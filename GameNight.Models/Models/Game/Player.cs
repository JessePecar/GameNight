namespace GameNight.Models.Models.Game
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public int Score { get; set; }
        public string ConnectionId { get; set; }
    }
}
