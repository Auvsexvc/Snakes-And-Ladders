namespace SnakesAndLaddersWPF
{
    public class GameGrid
    {
        public int[,] BoardFieldValues { get; } = new int[10, 10];

        public GameGrid(Board board)
        {
            SetupGrid(board);
        }

        private void SetupGrid(Board board)
        {
            for (int r = 0; r < board.Side; r++)
            {
                for (int c = 0; c < board.Side; c++)
                {
                    if (r % 2 == 0)
                    {
                        BoardFieldValues[r, c] = board.Size - (c + (r * board.Side));
                    }
                    else
                    {
                        BoardFieldValues[r, c] = board.Size - ((r * board.Side) + board.Side - c) + 1;
                    }
                }
            }
        }
    }
}