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
        private int _steps;
        private int _stepsToDieWithoutFood;
        private int _defaultStepsWithoutFood = 1000;
        private int _loopCount;
        private (int X, int Y) _lastTail;

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
            _steps = 0;
            _lastTail = (0, 0);
            _stepsToDieWithoutFood = _defaultStepsWithoutFood;
        }

        public void Lengthen()
        {
            Length++;
        }

        public void Kill()
        {
            Alive = false;
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

            if (newHead.X <= 0 || newHead.X >= _mapWidth
                || newHead.Y <= 0 || newHead.Y >= _mapHeight
                || OccupiesSquare(newHead)
                || _stepsToDieWithoutFood == 0)
            {
                Alive = false;
                return;
            }
            
            if (newHead == food.Location)
            {
                food.Eaten = true;
                Length++;
                _stepsToDieWithoutFood = _defaultStepsWithoutFood;
            }

            if (newHead == _lastTail)
            {
                _loopCount++;
                if (_loopCount > 5)
                {
                    Alive = false;
                    return;
                }
            }
            _lastTail = _snake.Last();

            _snake.Insert(0, newHead);
            _snake = _snake.Take(Length).ToList();

            DistanceToFood = -CartesianDistance(newHead, food.Location);

            LookingAtFood = 0;
            if (newHead.X == food.Location.X || newHead.Y == food.Location.Y)
            {
                if (SnakeDirection == Direction.North && newHead.Y > food.Location.Y
                    || SnakeDirection == Direction.South && newHead.Y < food.Location.Y
                    || SnakeDirection == Direction.East && newHead.X < food.Location.X
                    || SnakeDirection == Direction.West && newHead.X > food.Location.X)
                {
                    LookingAtFood = -1.0;
                }
                if(SnakeDirection == Direction.North && newHead.Y == food.Location.Y
                    || SnakeDirection == Direction.South && newHead.Y == food.Location.Y
                    || SnakeDirection == Direction.East && newHead.X == food.Location.X
                    || SnakeDirection == Direction.West && newHead.X == food.Location.X)
                {
                    LookingAtFood = -0.5;
                }
            }

            DistanceToNorthWall = newHead.Y - _snake.Where(s => s.X == newHead.X && s.Y < newHead.Y).OrderByDescending(s => s.Y).FirstOrDefault().Y ;
            DistanceToSouthWall = FirstOrWall(_snake.Where(s => s.X == newHead.X && s.Y > newHead.Y).OrderBy(s => s.Y)).Y - newHead.Y;

            DistanceToWestWall = newHead.X - _snake.Where(s => s.Y == newHead.Y && s.X < newHead.X).OrderByDescending(s => s.X).FirstOrDefault().X;
            DistanceToEastWall = FirstOrWall(_snake.Where(s => s.Y == newHead.Y && s.X > newHead.X).OrderBy(s => s.X)).X - newHead.X;

            _steps++;
            _stepsToDieWithoutFood--;
        }

        public double CartesianDistance((int X, int Y) p, (int X, int Y) q)
        {
            return Math.Sqrt(Math.Pow((p.X - q.X), 2) + Math.Pow((p.Y - q.Y), 2));
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

        public (int X, int Y) FirstOrWall(IEnumerable<(int X, int Y)> ps)
        {
            var p = ps.FirstOrDefault();
            if (p == (0, 0))
            {
                p = (_mapWidth, _mapHeight);
            }
            return p;
        }


        public (int X, int Y) Head => _snake.First();
        public (int X, int Y) Last => _lastTail;

        public IEnumerable<(int X, int Y)> SnakeBody => _snake;

        public int Length { get; private set; }
        public bool Alive { get; private set; }
        public double Fitness => (Length * 1000) + ((double)_steps / 1000);

        public double DistanceToNorthWall { get; private set; }
        public double DistanceToSouthWall { get; private set; }
        public double DistanceToWestWall { get; private set; }
        public double DistanceToEastWall { get; private set; }
        public double DistanceToFood { get; private set; }
        public double LookingAtFood { get; private set; }
    }

    public enum Direction
    {
        North,
        South,
        East,
        West
    }
}
