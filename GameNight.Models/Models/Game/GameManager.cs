using GameNight.Models.Enums;

namespace GameNight.Models.Models.Game
{
    public class GameManager
    {
        public Guid AdminKey { get; set; }
        public string LobbyKey { get; set; }
        public Games GameType { get; set; }
    }
}
