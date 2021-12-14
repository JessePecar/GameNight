using GameNight.Models.Enums;

namespace GameNight.Lobby.Hubs
{
    public interface ILobbyHub
    {
        Task InvalidGameRequest();
        Task GameJoinedSuccessfully(Games gameType);
        Task GameStart();
        Task RoundStart();
        Task PlayersTurn();
        Task SendDetails(string user, object details);
        Task SubmitToJudge(object submission);
    }
}
