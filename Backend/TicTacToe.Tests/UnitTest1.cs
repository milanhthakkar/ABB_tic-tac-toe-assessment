using Xunit;
using System;
using TicTacToe.Backend.Models; // 👈 Ensures GameMode, GameStatus, etc. are instantly found

namespace TicTacToe.Backend.Tests
{
    public class GameEngineTests
    {
        [Fact]
        public void CreateGame_ShouldInitializeEmptyBoardAndSetCurrentPlayerToX()
        {
            var service = new GameService();
            var game = service.CreateGame(GameMode.TwoPlayer);

            Assert.NotNull(game);
            Assert.Equal("X", game.CurrentPlayer);
            Assert.Equal(GameStatus.InProgress, game.Status);
            foreach (var cell in game.Board)
            {
                Assert.True(string.IsNullOrEmpty(cell));
            }
        }

        [Fact]
        public void MakeMove_ValidMove_ShouldUpdateBoardAndAlternateTurn()
        {
            var service = new GameService();
            var game = service.CreateGame(GameMode.TwoPlayer);

            var updatedGame = service.MakeMove(game.GameId, "X", 0); 

            Assert.Equal("X", updatedGame.Board[0]);
            Assert.Equal("O", updatedGame.CurrentPlayer); 
            Assert.Single(updatedGame.MoveHistory);
        }

        [Fact]
        public void MakeMove_OccupiedCell_ShouldThrowInvalidOperationException()
        {
            var service = new GameService();
            var game = service.CreateGame(GameMode.TwoPlayer);

            service.MakeMove(game.GameId, "X", 4); 

            Assert.Throws<InvalidOperationException>(() => service.MakeMove(game.GameId, "O", 4)); 
        }

        [Fact]
        public void MakeMove_RowWinCondition_ShouldTriggerVictoryAndScoreboardAddition()
        {
            var service = new GameService();
            var game = service.CreateGame(GameMode.TwoPlayer);

            service.MakeMove(game.GameId, "X", 0); 
            service.MakeMove(game.GameId, "O", 3); 
            service.MakeMove(game.GameId, "X", 1); 
            service.MakeMove(game.GameId, "O", 4); 
            var finalState = service.MakeMove(game.GameId, "X", 2); 

            Assert.Equal(GameStatus.Won, finalState.Status);
            Assert.Equal("X", finalState.Winner);
            Assert.Equal(3, finalState.WinningCells.Count);
            Assert.Equal(1, service.GetScoreboard().XWins); 
        }

        [Fact]
        public void UndoMove_InTwoPlayerMode_ShouldRevertOnlySingleLastMove()
        {
            var service = new GameService();
            var game = service.CreateGame(GameMode.TwoPlayer);

            service.MakeMove(game.GameId, "X", 0);
            service.MakeMove(game.GameId, "O", 4);
            var stateAfterUndo = service.UndoMove(game.GameId);

            Assert.Equal("", stateAfterUndo.Board[4]); 
            Assert.Equal("X", stateAfterUndo.Board[0]); 
            Assert.Equal("O", stateAfterUndo.CurrentPlayer); 
        }

        [Fact]
        public void ComputerMoveSelection_XThreateningImmediateWin_ShouldBlockX()
        {
            var service = new GameService();
            var game = service.CreateGame(GameMode.Computer);

            // Human takes index 0 -> Computer automatically answers (takes center 4)
            game = service.MakeMove(game.GameId, "X", 0); 

            // Human takes index 1 -> Threatens immediate row win at index 2.
            // Computer must defensively intercept and mark index 2 as "O"
            var stateAfterCpuIntercept = service.MakeMove(game.GameId, "X", 1); 

            Assert.Equal("O", stateAfterCpuIntercept.Board[2]); 
        }
    }
}
