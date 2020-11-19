﻿using System;
using System.Text;
using System.Threading;
//using System.Timers;

namespace Snake
{
    class Program
    {
        static ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        const int MapWidth = 20;
        const int MapHeight = 10;
        static Snake snake;
        static Timer timer;

        public static void Main(string[] args)
        {
            snake = new Snake(MapWidth, MapHeight);
            timer = new Timer(Tick, _manualResetEvent, 1000, 1000);

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

            snake.Move(direction);
            Draw();
        }

        public static void Draw()
        {
            StringBuilder sb = new StringBuilder();

            for (int y = 0; y < MapHeight; y++)
            {
                for(int x = 0; x < MapWidth; x++)
                {
                    sb.Append(snake.OccupiesSquare(x, y) ? "#" : " ");
                }
                sb.AppendLine();
            }
            sb.AppendLine($"\nSnake Position: Width: {snake.Head.X}, Height: {snake.Head.Y}, Length: {snake.Length}");

            Console.Clear();
            Console.Write(sb.ToString());
        }
    }
}
