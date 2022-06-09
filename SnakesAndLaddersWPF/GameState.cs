using System;

namespace SnakesAndLaddersWPF
{
    public class GameState
    {
        private readonly Player player1, player2;
        private readonly Board board;
        private readonly GameGrid gameGrid;

        public bool GameOver { get; private set; }
        public Player CurrentPlayer { get; private set; }
        public Player[] Players { get; }
        public int TurnsPassed { get; private set; }

        public GameGrid GameGrid => gameGrid;
        public Board Board => board;

        public event Action<int>? MoveMade;

        public event Action<GameResult>? GameEnded;

        public event Action? GameRestarted;

        public GameState()
        {
            GameOver = false;
            TurnsPassed = 1;
            player1 = new Player("Player 1");
            player2 = new Player("Player 2");
            Players = new[] { player1, player2 };
            CurrentPlayer = player1;
            board = new Board(100);
            gameGrid = new(board);
        }

        public void MakeMove(int die1, int die2)
        {
            if (!CanMakeMove())
            {
                return;
            }

            if (CurrentPlayer.Position + die1 + die2 > board.Size)
            {
                CurrentPlayer.Position = board.Size - (CurrentPlayer.Position + die1 + die2 - board.Size);
            }
            else
            {
                CurrentPlayer.Position += die1 + die2;
            }

            CurrentPlayer.ClimbUp(board.ladders);
            CurrentPlayer.SlideDown(board.snakes);

            MoveMade?.Invoke(CurrentPlayer.Position);
            if (DidMoveEndGame(out GameResult gameResult))
            {
                GameOver = true;
                GameEnded?.Invoke(gameResult);
            }
            else if (die1 != die2)
            {
                SwitchPlayer();
                EndTurn();
            }
        }

        private bool DidMoveEndGame(out GameResult gameResult)
        {
            if (CurrentPlayer.Position == board.Finish)
            {
                gameResult = new GameResult { Winner = CurrentPlayer, NumberOfTurns = TurnsPassed };
                return true;
            }

            gameResult = null!;
            return false;
        }

        private void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == player1 ? player2 : player1;
        }

        private bool CanMakeMove()
        {
            return !GameOver;
        }

        private void EndTurn()
        {
            if (CurrentPlayer == player1)
            {
                TurnsPassed++;
            }
        }

        public void Reset()
        {
            CurrentPlayer = player1;
            foreach (Player player in Players)
            {
                player.Position = 1;
            }
            TurnsPassed = 1;
            GameOver = false;
            GameRestarted?.Invoke();
        }
    }
}