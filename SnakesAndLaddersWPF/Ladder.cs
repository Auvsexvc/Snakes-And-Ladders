namespace SnakesAndLaddersWPF
{
    public class Ladder
    {
        public int Bottom { get; }
        public int Top { get; }

        public Ladder(int bottom, int top)
        {
            Top = top;
            Bottom = bottom;
        }
    }
}