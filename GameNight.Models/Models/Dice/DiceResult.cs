using GameNight.Models.Enums;

namespace GameNight.Models.Models.Dice
{
    public class DiceResult
    {
        public DiceType Type { get; set; }
        public List<int> Rolls { get; set; }
        public int RollCount { get; set; }
    }
}
