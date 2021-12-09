using GameNight.Models.Enums;

namespace GameNight.Models.Models.Game
{
    public class Lobby : GameManager
    {
        public string Password { get; set; }
        public Games GameType { get; set; } 
        public int TurnNumber { get; set; }
        public List<Player> Players { get; set; }
    }
}
