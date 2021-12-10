﻿using GameNight.Models.Models.Game;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace GameNight.Lobby.Hubs
{
    public class LobbyHub : Hub<ILobbyHub>
    {
        private readonly IMemoryCache _cache;
        public LobbyHub(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task JoinGame(string lobbyKey, string password, string userName, Guid? adminKey = null)
        {
            if (CanJoinLobby(lobbyKey, password, out Models.Models.Game.Lobby lobby))
            {
                Player player = new Player
                {
                    Name = userName,
                    Id = Guid.NewGuid(),
                    IsAdmin = adminKey != null && lobby.AdminKey == adminKey,
                    ConnectionId = Context.ConnectionId
                };
                lobby.Players.Add(player);

                _cache.Set(lobbyKey, lobby);
            }
            else
            {
                return Clients.Caller.InvalidGameRequest();
            }

            Groups.AddToGroupAsync(Context.ConnectionId, lobbyKey);
            return Clients.Caller.GameJoinedSuccessfully();
        }
        
        public Task LeaveGame(string lobbyKey)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyKey);
        }

        
        public Task StartGame(string lobbyKey, Guid adminKey)
        {
            if(IsAdminOfLobby(lobbyKey, adminKey, out var lobby))
            {
                return Clients.Group(lobbyKey).GameStart();
            }
            return Clients.Caller.InvalidGameRequest();
        }

        public Task StartRound(string lobbyKey, Guid adminKey)
        {
            if (IsAdminOfLobby(lobbyKey, adminKey, out var lobby))
            {
                lobby.TurnNumber = 0;
                _cache.Set(lobbyKey, lobby);
                Clients.All.RoundStart();
                return SendPlayerTurnStart(lobby.Players.ElementAt(lobby.TurnNumber).ConnectionId);
            }
            return Clients.Caller.InvalidGameRequest();
        }

        public Task NextTurn(string lobbyKey)
        {
            if(_cache.TryGetValue(lobbyKey, out Models.Models.Game.Lobby lobby))
            {
                if (lobby.Players.Count == lobby.TurnNumber + 1) lobby.TurnNumber = 0;
                else lobby.TurnNumber += 1;
                
                _cache.Set(lobbyKey, lobby);
                return SendPlayerTurnStart(lobby.Players.ElementAt(lobby.TurnNumber).ConnectionId);
            }
            return Clients.Caller.InvalidGameRequest();
        }

        public Task SendPlayersDetails(string lobbyKey, object details)
        {
            return Clients.OthersInGroup(lobbyKey).SendDetails(details);
        }

        public Task UpdateScore(string lobbyKey, string userName)
        {
            if (_cache.TryGetValue(lobbyKey, out Models.Models.Game.Lobby lobby))
            {
                lobby.Players.FirstOrDefault(p => p.Name == userName).Score++;
                _cache.Set(lobbyKey, lobby);
                return NextTurn(lobbyKey);
            }
            return Clients.Caller.InvalidGameRequest();
        }

        public Task SubmitToJudge(string lobbyKey, object submission)
        {
            if (_cache.TryGetValue(lobbyKey, out Models.Models.Game.Lobby lobby))
            {
                string connection = lobby.Players.ElementAt(lobby.TurnNumber).ConnectionId;
                
                return Clients.Client(connection).SubmitToJudge(lobbyKey);
            }
            return Clients.Caller.InvalidGameRequest();
        }

        private bool CanJoinLobby(string lobbyKey, string password, out Models.Models.Game.Lobby lobby)
        {
            return _cache.TryGetValue(lobbyKey, out lobby) && lobby != null && lobby.Password.Equals(password);
        }

        private bool IsAdminOfLobby(string lobbyKey, Guid adminKey, out Models.Models.Game.Lobby lobby)
        {
            return _cache.TryGetValue(lobbyKey, out lobby) && lobby != null && lobby.AdminKey.Equals(adminKey);
        }

        private Task SendPlayerTurnStart(string connectionId)
        {
            return Clients.Client(connectionId).PlayersTurn();
        }
        
    }
}