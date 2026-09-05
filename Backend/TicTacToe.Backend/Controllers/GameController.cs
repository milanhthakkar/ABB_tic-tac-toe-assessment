using Microsoft.AspNetCore.Mvc;
using System;
using TicTacToe.Backend.Models;

namespace TicTacToe.Backend.Controllers
{
    [ApiController]
    [Route("api")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("games")]
        public ActionResult<GameState> CreateGame([FromQuery] GameMode mode)
        {
            return Ok(_gameService.CreateGame(mode));
        }

        [HttpGet("games/{id}")]
        public ActionResult<GameState> GetGame(Guid id)
        {
            var game = _gameService.GetGame(id);
            if (game == null) return NotFound();
            return Ok(game);
        }

        public class MoveRequest { public string Player { get; set; } = string.Empty; public int Index { get; set; } }

        [HttpPost("games/{id}/moves")]
        public ActionResult<GameState> SubmitMove(Guid id, [FromBody] MoveRequest req)
        {
            try
            {
                return Ok(_gameService.MakeMove(id, req.Player, req.Index));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("games/{id}/undo")]
        public ActionResult<GameState> Undo(Guid id)
        {
            try
            {
                return Ok(_gameService.UndoMove(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("games/{id}/reset")]
        public ActionResult<GameState> ResetGame(Guid id)
        {
            try
            {
                return Ok(_gameService.ResetGame(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("scoreboard")]
        public ActionResult<Scoreboard> GetScoreboard() => Ok(_gameService.GetScoreboard());

        [HttpPost("scoreboard/reset")]
        public ActionResult<Scoreboard> ResetScoreboard()
        {
            _gameService.ResetScoreboard();
            return Ok(_gameService.GetScoreboard());
        }
    }
}
