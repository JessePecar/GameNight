using GameNight.API.Utilities.Interfaces;

namespace GameNight.API.Utilities
{
    public class LobbyKeyGenerator : ILobbyKeyGenerator
    {
        private readonly Random _random;
        private readonly string _keyChars;
        public LobbyKeyGenerator()
        {
            _random = new Random();
            //Possible TODO: Make configurable
            _keyChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        }

        public string GenerateLobbyKey(int keyLength = 4)
        {
            string lobbyKey = new string(Enumerable.Repeat(_keyChars, keyLength)
                .Select(r => r[_random.Next(r.Length)]).ToArray());

            return lobbyKey;
        }
    }
}
