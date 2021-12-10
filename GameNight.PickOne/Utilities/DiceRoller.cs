using GameNight.API.Utilities.Interfaces;
using GameNight.Models.Enums;

namespace GameNight.API.Utilities
{
    public class DiceRoller : IDiceRoller
    {
        private readonly Random _random;
        public DiceRoller()
        {
            _random = new Random();
        }

        public List<int> RoleDice(int numberOfDice, DiceType diceType)
        {
            List<int> rolledDice = new List<int>();

            for (int dice = 0; dice < numberOfDice; dice++)
            {
                rolledDice.Add(_random.Next(1, (int)diceType));
            }

            return rolledDice;
        }
    }
}
