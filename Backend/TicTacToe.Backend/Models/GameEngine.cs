using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe.Backend.Models
{
    public enum GameMode { TwoPlayer, Computer }
    public enum GameStatus { InProgress, Won, Draw }

    public class MoveLog
    {
        public int MoveNumber { get; set; }
        public string Player { get; set; } = string.Empty;
        public int Row { get; set; }
        public int Column { get; set; }
    }

    public class GameState
    {
        public Guid GameId { get; set; } = Guid.NewGuid();
        public string[] Board { get; set; } = new string[9].Select(_ => "").ToArray(); 
        public string CurrentPlayer { get; set; } = "X";
        public GameMode Mode { get; set; } = GameMode.TwoPlayer;
        public GameStatus Status { get; set; } = GameStatus.InProgress;
        public string Winner { get; set; } = string.Empty;
        public List<int> WinningCells { get; set; } = new List<int>();
        public List<MoveLog> MoveHistory { get; set; } = new List<MoveLog>();
    }

    public class Scoreboard
    {
        public int XWins { get; set; }
        public int OWins { get; set; }
        public int Draws { get; set; }
    }

    public interface IGameService
    {
        GameState CreateGame(GameMode mode);
        GameState? GetGame(Guid id);
        GameState MakeMove(Guid id, string player, int index);
        GameState UndoMove(Guid id);
        GameState ResetGame(Guid id);
        Scoreboard GetScoreboard();
        void ResetScoreboard();
    }

    public class GameService : IGameService
    {
        private readonly Dictionary<Guid, GameState> _games = new();
        private readonly Scoreboard _scoreboard = new();

        private static readonly int[][] WinningCombinations = new int[][]
        {
            new[] {0, 1, 2}, new[] {3, 4, 5}, new[] {6, 7, 8}, // Rows
            new[] {0, 3, 6}, new[] {1, 4, 7}, new[] {2, 5, 8}, // Columns
            new[] {0, 4, 8}, new[] {2, 4, 6}                  // Diagonals
        };

        public GameState CreateGame(GameMode mode)
        {
            var game = new GameState { Mode = mode };
            _games[game.GameId] = game;
            return game;
        }

        public GameState? GetGame(Guid id) => _games.TryGetValue(id, out var game) ? game : null;

        public Scoreboard GetScoreboard() => _scoreboard;

        public void ResetScoreboard()
        {
            _scoreboard.XWins = 0;
            _scoreboard.OWins = 0;
            _scoreboard.Draws = 0;
        }

        public GameState ResetGame(Guid id)
        {
            if (!_games.TryGetValue(id, out var game)) 
                throw new ArgumentException("Session not found.");

            game.Board = new string[9].Select(_ => "").ToArray();
            game.CurrentPlayer = "X";
            game.Status = GameStatus.InProgress;
            game.Winner = string.Empty;
            game.WinningCells.Clear();
            game.MoveHistory.Clear();
            return game;
        }

        public GameState MakeMove(Guid id, string player, int index)
        {
            if (!_games.TryGetValue(id, out var game)) throw new ArgumentException("Invalid Game ID.");
            ValidateMove(game, player, index);

            ApplyMove(game, player, index);

            if (game.Status == GameStatus.InProgress && game.Mode == GameMode.Computer && game.CurrentPlayer == "O")
            {
                int cpuIndex = CalculateComputerMove(game);
                ApplyMove(game, "O", cpuIndex);
            }

            return game;
        }

        private void ValidateMove(GameState game, string player, int index)
        {
            if (game.Status != GameStatus.InProgress) throw new InvalidOperationException("Game is over.");
            if (index < 0 || index > 8) throw new ArgumentOutOfRangeException(nameof(index), "Out of bounds.");
            if (!string.IsNullOrEmpty(game.Board[index])) throw new InvalidOperationException("Cell occupied.");
            if (game.CurrentPlayer != player) throw new InvalidOperationException("Not your turn.");
        }

        private void ApplyMove(GameState game, string player, int index)
        {
            game.Board[index] = player;
            game.MoveHistory.Add(new MoveLog
            {
                MoveNumber = game.MoveHistory.Count + 1,
                Player = player,
                Row = (index / 3) + 1,
                Column = (index % 3) + 1
            });

            if (CheckWin(game.Board, player, out var winningLines))
            {
                game.Status = GameStatus.Won;
                game.Winner = player;
                game.WinningCells = winningLines;
                UpdateScoreboard(player);
                return;
            }

            if (game.Board.All(cell => !string.IsNullOrEmpty(cell)))
            {
                game.Status = GameStatus.Draw;
                _scoreboard.Draws++;
                return;
            }

            game.CurrentPlayer = (player == "X") ? "O" : "X";
        }

        public GameState UndoMove(Guid id)
        {
            if (!_games.TryGetValue(id, out var game)) throw new ArgumentException("Invalid Game ID.");
            if (game.Status != GameStatus.InProgress) throw new InvalidOperationException("Cannot undo finished matches.");
            if (!game.MoveHistory.Any()) return game;

            if (game.Mode == GameMode.TwoPlayer)
            {
                var lastMove = game.MoveHistory.Last();
                int idx = ((lastMove.Row - 1) * 3) + (lastMove.Column - 1);
                game.Board[idx] = "";
                game.CurrentPlayer = lastMove.Player;
                game.MoveHistory.RemoveAt(game.MoveHistory.Count - 1);
            }
            else // Computer Mode
            {
                if (game.MoveHistory.Count >= 2)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        var target = game.MoveHistory.Last();
                        int idx = ((target.Row - 1) * 3) + (target.Column - 1);
                        game.Board[idx] = "";
                        game.MoveHistory.RemoveAt(game.MoveHistory.Count - 1);
                    }
                    game.CurrentPlayer = "X";
                }
            }
            return game;
        }

        private bool CheckWin(string[] board, string player, out List<int> cells)
        {
            cells = new List<int>();
            foreach (var combo in WinningCombinations)
            {
                if (board[combo[0]] == player && board[combo[1]] == player && board[combo[2]] == player)
                {
                    cells = combo.ToList();
                    return true;
                }
            }
            return false;
        }

        private void UpdateScoreboard(string winner)
        {
            if (winner == "X") _scoreboard.XWins++;
            else if (winner == "O") _scoreboard.OWins++;
        }

        private int CalculateComputerMove(GameState game)
        {
            // Priority 1: Check if 'O' can win immediately
            for (int i = 0; i < 9; i++)
            {
                if (string.IsNullOrEmpty(game.Board[i]))
                {
                    var clone = (string[])game.Board.Clone();
                    clone[i] = "O";
                    if (CheckWin(clone, "O", out _)) return i;
                }
            }

            // Priority 2: Check if 'X' needs blocking
            for (int i = 0; i < 9; i++)
            {
                if (string.IsNullOrEmpty(game.Board[i]))
                {
                    var clone = (string[])game.Board.Clone();
                    clone[i] = "X";
                    if (CheckWin(clone, "X", out _)) return i;
                }
            }

            // Priority 3: Snag center
            if (string.IsNullOrEmpty(game.Board[4])) return 4;

            // Priority 4: Take open corners
            int[] corners = { 0, 2, 6, 8 };
            foreach (int c in corners)
            {
                if (string.IsNullOrEmpty(game.Board[c])) return c;
            }

            // Priority 5: Fallback to remaining spaces
            for (int i = 0; i < 9; i++)
            {
                if (string.IsNullOrEmpty(game.Board[i])) return i;
            }

            throw new InvalidOperationException("Grid structure evaluation error.");
        }
    }
}
