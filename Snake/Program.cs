using System;
using System.Text;
using System.Threading;
//using System.Timers;

namespace Snake
{
    class Program
    {
        static ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        const int MapWidth = 200;
        const int MapHeight = 50;
        static Snake snake;
        static Timer timer;
        static Food food;

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            snake = new Snake(MapWidth, MapHeight);
            food = new Food(MapWidth, MapHeight);

            timer = new Timer(Tick, _manualResetEvent, 100, 100);

            _manualResetEvent.WaitOne();
        }

        private static void Tick(object state)
        {
            Direction? direction = null;
            if(Console.KeyAvailable)
            {
                var cki = Console.ReadKey(true);

                if(cki.Key == ConsoleKey.Escape)
                {
                    _manualResetEvent.Set();
                }

                direction = cki.Key switch
                {
                    ConsoleKey.UpArrow => Direction.North,
                    ConsoleKey.DownArrow => Direction.South,
                    ConsoleKey.RightArrow => Direction.East,
                    ConsoleKey.LeftArrow => Direction.West,
                    _ => null
                    ,
                };

            }

            if (food.Eaten)
            {
                do
                {
                    food.Generate();
                }
                while(snake.OccupiesSquare(food.Location));
            }

            snake.Move(direction, food);
            Draw();
        }

        public static void Draw()
        {
            StringBuilder sb = new StringBuilder();

            for (int y = 0; y < MapHeight; y++)
            {
                for(int x = 0; x < MapWidth; x++)
                {
                    sb.Append(
                        snake.OccupiesSquare(x, y) 
                            ? "●" 
                            : food.Location == (x, y) 
                                ? "ᴥ"
                                : " ");
                }
                sb.AppendLine();
            }
            sb.AppendLine($"\nSnake Position: X: {snake.Head.X}, Y: {snake.Head.Y}, Length: {snake.Length}");

            Console.Clear();
            Console.Write(sb.ToString());
        }
    }
}
