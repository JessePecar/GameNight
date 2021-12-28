namespace GameNight.Lobby.Hubs.InterfaceHubs
{
    public interface IChooseOneHub
    {
        Task RecievedPrompt(Guid deviceKey, string user, string answer);
        Task ShowAnswers();
        Task RevealJudge();
        Task RemoveAnswer(Guid deviceKey);
        Task ToggleJudge();
        Task StartTurn(object turnDetails);
    }
}
