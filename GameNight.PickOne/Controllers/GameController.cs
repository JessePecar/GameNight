﻿using GameNight.API.Utilities.Interfaces;
using GameNight.Models.CacheUtils;
using GameNight.Models.Enums;
using GameNight.Models.Models.Game;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Reflection;

namespace GameNight.API.Controllers
{
    [Route("api/Game")]
    [ApiController]
    public class GameController : Controller
    {
        private readonly ILobbyKeyGenerator _lobbyKey;
        private readonly IMemoryCache _cache;
        public GameController(ILobbyKeyGenerator lobbyKey, IMemoryCache cache)
        {
            _lobbyKey = lobbyKey;
            _cache = cache;
        }

        /// <summary>
        /// Creates a new game of type passed with a password to prevent unwanted joiners.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("InitializeNewGame")]
        public IActionResult InitializeNewGame(string password = "", Games gameType = Games.ChooseOne)
        {
            try
            {
                GameManager gameManager = new GameManager
                {
                    LobbyKey = _lobbyKey.GenerateLobbyKey(),
                    AdminKey = Guid.NewGuid(),
                    GameType = gameType
                };

                if(_cache.TryGetValue(gameManager.LobbyKey, out var game))
                {
                    //Game exists, this should mean that you will need to get a new lobby key, but for now, I will end the current game and create a new one.
                    //Could think of the idea of having a lobby key tracker that will prevent new keys from being created when one is active.
                    _cache.Remove(gameManager.LobbyKey);
                }

                Models.Models.Game.Lobby lobby = new Models.Models.Game.Lobby
                {
                    AdminKey = gameManager.AdminKey,
                    LobbyKey = gameManager.LobbyKey,
                    Password = password,
                    GameType = gameType,
                    Players = new List<Player>()
                };

                _cache.Set(gameManager.LobbyKey, lobby, DateTimeOffset.Now.AddDays(1));

                return Ok(gameManager);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// This will be a locked down controller eventually that will only be accessible to admins of the GameNight suite
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("AllLobbies")]
        public IActionResult AllLobbies()
        {
            var lobbies = _cache.GetAllCacheItems<Models.Models.Game.Lobby>();
            return Ok(lobbies);
        }

        /// <summary>
        /// Close the game with the specific game code and admin key that was given when the game was created.
        /// </summary>
        /// <param name="lobbyKey"></param>
        /// <param name="adminKey"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CloseGame")]
        public IActionResult CloseGame([FromBody] GameManager gameManager)
        {
            try
            {
                if(_cache.TryGetValue(gameManager.LobbyKey, out Models.Models.Game.Lobby lobby))
                {
                    if(lobby.AdminKey == gameManager.AdminKey)
                    {
                        _cache.Remove(gameManager.LobbyKey);
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
