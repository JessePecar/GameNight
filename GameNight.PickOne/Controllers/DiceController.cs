using GameNight.API.Utilities.Interfaces;
using GameNight.Models.Enums;
using GameNight.Models.Models.Dice;
using Microsoft.AspNetCore.Mvc;

namespace GameNight.API.Controllers
{
    [ApiController]
    [Route("api/Dice")]
    public class DiceController : Controller
    {
        private readonly IDiceRoller _diceRoller;

        public DiceController(IDiceRoller diceRoller)
        {
            _diceRoller = diceRoller;
        }


        [HttpGet]
        [Route("DndRoll")]
        public IActionResult DndRoll(int numberOfDice, DiceType diceType = DiceType.D20)
        {
            try
            {
                DiceResult result = new DiceResult
                {
                    Type = diceType,
                    RollCount = numberOfDice,
                    Rolls = _diceRoller.RoleDice(numberOfDice, diceType)
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("WrathAndGloryRoll")]
        public IActionResult WrathAndGloryRoll(int numberOfDice)
        {
            try
            {
                DiceResult result = new DiceResult
                {
                    Type = DiceType.D6,
                    RollCount = numberOfDice,
                    Rolls = _diceRoller.RoleDice(numberOfDice, DiceType.D6)
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
