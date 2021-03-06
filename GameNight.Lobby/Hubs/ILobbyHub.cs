using GameNight.Lobby.Hubs.InterfaceHubs;
using GameNight.Models.Enums;

namespace GameNight.Lobby.Hubs
{
    public interface ILobbyHub : IChooseOneHub, IPregameLobby
    {
        Task InvalidGameRequest();
        Task GameJoinedSuccessfully(int gameType);
        Task GameStart();
        Task RoundStart();
        Task PlayersTurn();
        Task SendDetails(string user, Guid deviceKey, object details);
        Task SubmitToJudge(object submission);
        
    }
}
