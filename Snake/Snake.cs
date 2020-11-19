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

            Create();
        }

        public void Create()
        {
            _snake = new List<(int X, int Y)>
            {
                (_mapWidth / 2, _mapHeight / 2)
            };
            SnakeDirection = Direction.East;
            Length = 3;
            Alive = true;
        }

        public void Move(Direction? direction, Food food)
        {
            SnakeDirection = direction;
            
            var currentHeadX = _snake.First().X;
            var currentHeadY = _snake.First().Y;

            (int X, int Y) newHead = SnakeDirection switch
            {
                Direction.North => (currentHeadX, currentHeadY - 1),
                Direction.South => (currentHeadX, currentHeadY + 1),
                Direction.East => (currentHeadX + 1, currentHeadY),
                Direction.West => (currentHeadX - 1, currentHeadY),
                _ => throw new NotImplementedException()
            };

            if (newHead.X < 0 || newHead.X > _mapWidth || newHead.Y < 0 || newHead.Y > _mapHeight)
            if (newHead.X < 0 || newHead.X > _mapWidth 
                || newHead.Y < 0 || newHead.Y > _mapHeight
                || OccupiesSquare(newHead))
            {
                Alive = false;
                return;
            }

            if (newHead == food.Location)
            {
                food.Eaten = true;
                Length++;
            }

            _snake.Insert(0, newHead);
            _snake = _snake.Take(Length).ToList();
        }

        public bool OccupiesSquare((int X, int Y) location) 
        {
            return OccupiesSquare(location.X, location.Y);
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

        public bool Alive { get; private set; }
    }

    public enum Direction
    {
        North,
        South,
        East,
        West
    }
}
