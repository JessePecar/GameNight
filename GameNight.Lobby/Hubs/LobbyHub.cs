using GameNight.Models.CacheUtils;
using GameNight.Models.Enums;
using GameNight.Models.Models.Game;
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

        public Task JoinGame(string lobbyKey, string userName, Guid deviceKey)
        {
            Games gameType;
            Player player = new Player();
            if (CanJoinLobby(lobbyKey, out Models.Models.Game.Lobby lobby))
            {
                player = new Player
                {
                    Name = userName,
                    Id = deviceKey,
                    IsAdmin = lobby.AdminKey == deviceKey,
                    ConnectionId = Context.ConnectionId
                };

                if (lobby.Players.Any(p => p.Id == deviceKey))
                {
                    //Remove the devices previous connections, change the players name to the sent in name
                    Parallel.ForEach(lobby.Players.Where(p => p.Id == deviceKey), async (ply) =>
                    {
                        await Groups.RemoveFromGroupAsync(ply.ConnectionId, lobbyKey);
                    });

                    lobby.Players.FirstOrDefault(p => p.Id == deviceKey).Name = userName;
                }
                else
                {
                    lobby.Players.Add(player);
                }
                gameType = lobby.GameType;

                _cache.Set(lobbyKey, lobby);
            }
            else
            {
                return Clients.Caller.InvalidGameRequest();
            }

            Groups.AddToGroupAsync(Context.ConnectionId, lobbyKey);
            Clients.OthersInGroup(lobbyKey).PlayerJoined(player);
            return Clients.Caller.GameJoinedSuccessfully((int)gameType);
        }

        public Task LeaveGame(string lobbyKey, Guid deviceKey)
        {
            var lobby = _cache.Get<Models.Models.Game.Lobby>(lobbyKey);
            lobby.Players.RemoveAll(p => p.Id == deviceKey);
            _cache.Set(lobbyKey, lobby);

            return Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyKey);
        }

        public Task ReadyUpToggle(string lobbyKey, Guid deviceKey, bool isReady)
        {
            return Clients.Group(lobbyKey).PlayerToggleReadyUp(deviceKey, isReady);
        }

        public Task StartGame(string lobbyKey)
        {
            return Clients.Group(lobbyKey).GameStart();
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
            if (_cache.TryGetValue(lobbyKey, out Models.Models.Game.Lobby lobby))
            {
                if (lobby.Players.Count == lobby.TurnNumber + 1) lobby.TurnNumber = 0;
                else lobby.TurnNumber += 1;

                _cache.Set(lobbyKey, lobby);
                return SendPlayerTurnStart(lobby.Players.ElementAt(lobby.TurnNumber).ConnectionId);
            }
            return Clients.Caller.InvalidGameRequest();
        }

        public Task SendPlayersDetails(string lobbyKey, string user, Guid deviceKey, object details)
        {
            return Clients.OthersInGroup(lobbyKey).SendDetails(user, deviceKey, details);
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

        public void LeaveAllGames()
        {
            string connId = Context.ConnectionId;
            IEnumerable<Models.Models.Game.Lobby>? lobbies = _cache.GetAllCacheItems<Models.Models.Game.Lobby>();
            List<Models.Models.Game.Lobby>? playerLobbies = lobbies.Where(l => l.Players.Any(p => p.ConnectionId == connId)).ToList();

            Parallel.ForEach(playerLobbies, (pl) =>
            {
                pl.Players.RemoveAll(p => p.ConnectionId == connId);

                _cache.Set(pl.LobbyKey, pl);
            });

        }

        private bool CanJoinLobby(string lobbyKey, out Models.Models.Game.Lobby lobby)
        {
            return _cache.TryGetValue(lobbyKey, out lobby) && lobby != null;
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
