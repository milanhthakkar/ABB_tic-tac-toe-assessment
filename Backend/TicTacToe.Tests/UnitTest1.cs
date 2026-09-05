using Xunit;
using System;
using TicTacToe.Backend.Models;

namespace TicTacToe.Backend.Tests
{
    public class GameEngineTests
    {
        // 1. Test: Valid Move & 2. Test: Turn Switching
        [Fact]
        public void MakeMove_ValidMove_ShouldUpdateBoardAndAlternateTurn()
        {
            var service = new GameService();
            var game = service.CreateGame(GameMode.TwoPlayer);

            var updatedGame = service.MakeMove(game.GameId, "X", 0); 

            Assert.Equal("X", updatedGame.Board[0]);
            Assert.Equal("O", updatedGame.CurrentPlayer); // Turn switched
            Assert.Single(updatedGame.MoveHistory);
        }

        // 3. Test: Invalid Move
        [Fact]
        public void MakeMove_OccupiedCell_ShouldThrowInvalidOperationException()
        {
            var service = new GameService();
            var game = service.CreateGame(GameMode.TwoPlayer);

            service.MakeMove(game.GameId, "X", 4); 

            // Invalid move: trying to take an already occupied cell
            Assert.Throws<InvalidOperationException>(() => service.MakeMove(game.GameId, "O", 4)); 
        }

        // 4. Test: Row Win & 5. Test: Scoreboard Update
        [Fact]
        public void MakeMove_RowWinCondition_ShouldTriggerVictoryAndScoreboardAddition()
        {
            var service = new GameService();
            var game = service.CreateGame(GameMode.TwoPlayer);

            service.MakeMove(game.GameId, "X", 0); // Row 1
            service.MakeMove(game.GameId, "O", 3); 
            service.MakeMove(game.GameId, "X", 1); // Row 1
            service.MakeMove(game.GameId, "O", 4); 
            var finalState = service.MakeMove(game.GameId, "X", 2); // Row 1 Win!

            Assert.Equal(GameStatus.Won, finalState.Status);
            Assert.Equal("X", finalState.Winner);
            Assert.Equal(1, service.GetScoreboard().XWins); // Scoreboard updated
        }

        // 6. Test: Column Win
        [Fact]
        public void MakeMove_ColumnWinCondition_ShouldTriggerVictory()
        {
            var service = new GameService();
            var game = service.CreateGame(GameMode.TwoPlayer);

            service.MakeMove(game.GameId, "X", 0); // Col 1
            service.MakeMove(game.GameId, "O", 1); 
            service.MakeMove(game.GameId, "X", 3); // Col 1
            service.MakeMove(game.GameId, "O", 2); 
            var finalState = service.MakeMove(game.GameId, "X", 6); // Col 1 Win!

            Assert.Equal(GameStatus.Won, finalState.Status);
            Assert.Equal("X", finalState.Winner);
        }

        // 7. Test: Diagonal Win
        [Fact]
        public void MakeMove_DiagonalWinCondition_ShouldTriggerVictory()
        {
            var service = new GameService();
            var game = service.CreateGame(GameMode.TwoPlayer);

            service.MakeMove(game.GameId, "X", 0); // Diagonal top-left to bottom-right
            service.MakeMove(game.GameId, "O", 1); 
            service.MakeMove(game.GameId, "X", 4); // Center
            service.MakeMove(game.GameId, "O", 2); 
            var finalState = service.MakeMove(game.GameId, "X", 8); // Bottom-right Win!

            Assert.Equal(GameStatus.Won, finalState.Status);
            Assert.Equal("X", finalState.Winner);
        }

        // 8. Test: Draw
        [Fact]
        public void MakeMove_FullBoardNoWinner_ShouldResultInDraw()
        {
            var service = new GameService();
            var game = service.CreateGame(GameMode.TwoPlayer);

            // Simulating a standard cat's game / draw sequence
            service.MakeMove(game.GameId, "X", 0); service.MakeMove(game.GameId, "O", 1);
            service.MakeMove(game.GameId, "X", 2); service.MakeMove(game.GameId, "O", 4);
            service.MakeMove(game.GameId, "X", 3); service.MakeMove(game.GameId, "O", 5);
            service.MakeMove(game.GameId, "X", 7); service.MakeMove(game.GameId, "O", 6);
            var finalState = service.MakeMove(game.GameId, "X", 8); // Board filled

            Assert.Equal(GameStatus.Draw, finalState.Status);
            Assert.Equal(1, service.GetScoreboard().Draws);
        }

        // 9. Test: Reset Game
        [Fact]
        public void ResetGame_ShouldClearBoardAndHistoryButPreserveScoreboard()
        {
            var service = new GameService();
            var game = service.CreateGame(GameMode.TwoPlayer);

            service.MakeMove(game.GameId, "X", 0);
            service.MakeMove(game.GameId, "O", 3);
            
            var resetGame = service.ResetGame(game.GameId);

            Assert.Equal(GameStatus.InProgress, resetGame.Status);
            Assert.Equal("X", resetGame.CurrentPlayer);
            Assert.Empty(resetGame.MoveHistory);
            Assert.All(resetGame.Board, cell => Assert.True(string.IsNullOrEmpty(cell)));
        }

        // 10. Test: Undo in Two-Player Mode
        [Fact]
        public void UndoMove_InTwoPlayerMode_ShouldRevertOnlySingleLastMove()
        {
            var service = new GameService();
            var game = service.CreateGame(GameMode.TwoPlayer);

            service.MakeMove(game.GameId, "X", 0);
            service.MakeMove(game.GameId, "O", 4);
            var stateAfterUndo = service.UndoMove(game.GameId);

            Assert.Equal("", stateAfterUndo.Board[4]); // O's move removed
            Assert.Equal("X", stateAfterUndo.Board[0]); // X's move stays
            Assert.Equal("O", stateAfterUndo.CurrentPlayer); // Reverts back to O's turn
        }

        // 11. Test: Undo in Computer Mode
        [Fact]
        public void UndoMove_InComputerMode_ShouldRevertBothUserAndComputerMoves()
        {
            var service = new GameService();
            var game = service.CreateGame(GameMode.Computer);

            // Human plays X at 0; Computer plays O automatically (takes center 4)
            game = service.MakeMove(game.GameId, "X", 0);
            Assert.Equal("X", game.Board[0]);
            Assert.Equal("O", game.Board[4]);

            var stateAfterUndo = service.UndoMove(game.GameId);

            // Both moves must be completely erased
            Assert.Equal("", stateAfterUndo.Board[0]);
            Assert.Equal("", stateAfterUndo.Board[4]);
            Assert.Equal("X", stateAfterUndo.CurrentPlayer); // Ready for player X again
        }

        // 12. Test: Computer Move Selection (Blocking Logic)
        [Fact]
        public void ComputerMoveSelection_XThreateningImmediateWin_ShouldBlockX()
        {
            var service = new GameService();
            var game = service.CreateGame(GameMode.Computer);

            game = service.MakeMove(game.GameId, "X", 0); // Computer takes center 4 automatically
            var stateAfterCpuIntercept = service.MakeMove(game.GameId, "X", 1); // Threatens win at index 2

            Assert.Equal("O", stateAfterCpuIntercept.Board[2]); // Computer blocks row win
        }

        // 13. Test: Move After Game Completion
        [Fact]
        public void MakeMove_AfterGameIsWon_ShouldThrowInvalidOperationException()
        {
            var service = new GameService();
            var game = service.CreateGame(GameMode.TwoPlayer);

            // X wins row 1
            service.MakeMove(game.GameId, "X", 0); service.MakeMove(game.GameId, "O", 3);
            service.MakeMove(game.GameId, "X", 1); service.MakeMove(game.GameId, "O", 4);
            service.MakeMove(game.GameId, "X", 2); // Game Over!

            // Attempting to move after match completion should be rejected immediately
            Assert.Throws<InvalidOperationException>(() => service.MakeMove(game.GameId, "O", 8));
        }
    }
}
