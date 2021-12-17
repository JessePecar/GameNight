using System.ComponentModel;

namespace GameNight.Models.Enums
{
    public enum Games
    {
        [Description("Choose 1")]
        ChooseOne = 0,
        [Description("Table Top RPG")]
        TableTopRPG = 1,
        [Description("Wrath and Glory")]
        WrathAndGlory = 2,
        [Description("Dungeons and Dragons")]
        DnD = 3
    }
}
