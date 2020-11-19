using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snake
{
    public class Snake
    {
        private List<(int X, int Y)> _snake;
        private Direction _direction;
        private readonly int _mapWidth, _mapHeight;

        public Snake(int mapWidth, int mapHeight)
        {
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;

            _snake = new List<(int X, int Y)>
            {
                (mapWidth / 2, mapHeight / 2)
            };
            _direction = Direction.East;
            Length = 3;
        }

        public void Move()
        {
            var currentHeadX = _snake.First().X;
            var currentHeadY = _snake.First().Y;

            var newHead = _direction switch
            {
                Direction.North => (currentHeadX, currentHeadY - 1),
                Direction.South => (currentHeadX, currentHeadY + 1),
                Direction.East => (currentHeadX + 1, currentHeadY),
                Direction.West => (currentHeadX - 1, currentHeadY),
                _ => throw new NotImplementedException()
            };

            _snake.Insert(0, newHead);
            _snake = _snake.Take(Length).ToList();
        }

        public bool OccupiesSquare(int x, int y)
        {
            return _snake.Contains((x, y));
        }

        public int Length { get; private set; }

        public (int X, int Y) Head => _snake.First();

    }

    public enum Direction
    {
        North,
        South,
        East,
        West
    }
}
