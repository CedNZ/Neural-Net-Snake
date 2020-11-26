using System;
using System.Collections.Generic;
using System.Text;

namespace Snake
{
    public class Food
    {
        private readonly int _mapWidth, _mapHeight;
        private readonly Random _random;

        public Food(int mapWidth, int mapHeight)
        {
            _random = new Random();
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            Generate();
        }

        public void Generate()
        {
            var x = _random.Next(_mapWidth-4)+2;
            var y = _random.Next(_mapHeight-4)+2;

            Location = (x, y);
            Eaten = false;
        }

        public (int X, int Y) Location { get; set; }

        public bool Eaten;
    }
}
