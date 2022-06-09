using System;
using System.Linq;

namespace SnakesAndLaddersWPF
{
    public class Player
    {
        public string Name { get; }
        public int Position { get; set; }

        public Player(string name)
        {
            Name = name;
            Position = 1;
        }

        public void ClimbUp(Ladder[] ladders)
        {
            if (ladders.Select(l => l.Bottom).Contains(Position))
            {
                Position = Array
                    .Find(ladders, l => l.Bottom == Position)
                    .Top;
            }
        }

        public void SlideDown(Snake[] snakes)
        {
            if (snakes.Select(s => s.Head).Contains(Position))
            {
                Position = Array
                    .Find(snakes, s => s.Head == Position)
                    .Tail;
            }
        }
    }
}