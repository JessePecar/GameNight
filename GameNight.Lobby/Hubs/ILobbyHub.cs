namespace GameNight.Lobby.Hubs
{
    public interface ILobbyHub
    {
        Task InvalidGameRequest();
        Task GameJoinedSuccessfully();
        Task GameStart();
        Task RoundStart();
        Task PlayersTurn();
        Task SendDetails(object details);
        Task SubmitToJudge(object submission);
    }
}
