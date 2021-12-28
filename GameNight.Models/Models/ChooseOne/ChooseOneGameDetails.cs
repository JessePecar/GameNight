using GameNight.Models.Models.Game;

namespace GameNight.Models.Models.ChooseOne
{
    public class ChooseOneGameDetails : GameDetails
    {
        public string Prompt { get; set; }
        public List<ChooseOneAnswer> ChooseOneAnswers { get; set; }
    }
}
