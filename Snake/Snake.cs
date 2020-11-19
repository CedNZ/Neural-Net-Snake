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
            SnakeDirection = Direction.East;
            Length = 3;
        }

        public void Move(Direction? direction)
        {
            SnakeDirection = direction;
            
            var currentHeadX = _snake.First().X;
            var currentHeadY = _snake.First().Y;

            var newHead = SnakeDirection switch
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

        public Direction? SnakeDirection
        {
            get { return _direction; }
            set
            {
                if (value != null)
                {
                    if (((int)value & 2) != ((int)_direction & 2)) //Bitwise check if they lie on the same axis
                    {
                        _direction = value.Value;
                    }
                }
            }
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
