namespace GameNight.API.Utilities.Interfaces
{
    public interface ILobbyKeyGenerator
    {
        string GenerateLobbyKey(int keyLength = 4);
    }
}
