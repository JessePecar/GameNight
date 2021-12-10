using GameNight.Models.Enums;

namespace GameNight.API.Utilities.Interfaces
{
    public interface IDiceRoller
    {
        List<int> RoleDice(int numberOfDice, DiceType diceType);
    }
}
