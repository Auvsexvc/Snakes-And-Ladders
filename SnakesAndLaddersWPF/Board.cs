using System;

namespace SnakesAndLaddersWPF
{
    public class Board
    {
        public readonly Snake[] snakes;
        public readonly Ladder[] ladders;

        public int Size { get; }
        public int Start { get; }
        public int Finish { get; }
        public int Side { get; }

        public Board(int size)
        {
            Size = size;
            Start = 0;
            Finish = size;
            Side = (int)Math.Sqrt(Size);
            snakes = new Snake[]
            {
                    new Snake(16, 6),
                    new Snake(49, 11),
                    new Snake(46, 25),
                    new Snake(62, 19),
                    new Snake(64, 60),
                    new Snake(74, 53),
                    new Snake(89, 68),
                    new Snake(92, 88),
                    new Snake(95, 75),
                    new Snake(99, 80)
            };
            ladders = new Ladder[]
            {
                    new Ladder(2, 38),
                    new Ladder(7, 14),
                    new Ladder(8, 31),
                    new Ladder(15, 26),
                    new Ladder(28, 84),
                    new Ladder(21, 42),
                    new Ladder(36, 44),
                    new Ladder(51, 67),
                    new Ladder(71, 91),
                    new Ladder(78, 98),
                    new Ladder(87, 94)
            };
        }
    }
}