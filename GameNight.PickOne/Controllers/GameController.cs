using GameNight.API.Utilities.Interfaces;
using GameNight.Models.Enums;
using GameNight.Models.Models.Game;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

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
                    AdminKey = Guid.NewGuid()
                };

                if(_cache.TryGetValue(gameManager.LobbyKey, out var game))
                {
                    //Game exists, this should mean that you will need to get a new lobby key, but for now, I will end the current game and create a new one.
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
        /// 
        /// </summary>
        /// <param name="lobbyKey"></param>
        /// <param name="adminKey"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CloseGame")]
        public IActionResult CloseGame(string lobbyKey, Guid adminKey)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
