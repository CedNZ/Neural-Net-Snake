using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Snake
{
    public class Program
    {
        static ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        public const int MapWidth = 200;
        public const int MapHeight = 200;
        static Snake snake;
        static Timer timer;
        static Food food;
        static List<double> nOut;
        static bool paused;
        static bool tick;

        static int[] layers = new[] { 6, 5, 4, 4 };

        static int tickInterval = 10;

        static double bestCurrentFitness;
        static double bestOverallFitness;
        static NeuralNet.Manager manager;

        static string folder = $@"C:\temp\SnakeAI\v3";
        static string foodLocInputBuffer;

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            snake = new Snake(MapWidth, MapHeight);
            food = new Food(MapWidth, MapHeight);
            manager = new NeuralNet.Manager(layers);

            //timer = new Timer(Tick, _manualResetEvent, 1000, tickInterval);
            while (true)
            {
                Tick(_manualResetEvent);
            }

            _manualResetEvent.WaitOne();
        }

        private static void Tick(object state)
        {
            Direction? direction = null;
            if (tick)
            {
                tick = false;
                paused = true;
            }
            if(Console.KeyAvailable)
            {
                var cki = Console.ReadKey(true);

                if(cki.Key == ConsoleKey.Escape)
                {
                    _manualResetEvent.Set();
                }
                if(cki.Key == ConsoleKey.R)
                {
                    snake.Create();
                }
                if(cki.Key == ConsoleKey.L)
                {
                    snake.Lengthen();
                }
                if(cki.Key == ConsoleKey.P)
                {
                    paused = !paused;
                }
                if(cki.Key == ConsoleKey.T)
                {
                    paused = false;
                    tick = true;
                }
                if(cki.Key == ConsoleKey.K)
                {
                    snake.Kill();
                    manager.Next();
                    return;
                }
                if(cki.Key == ConsoleKey.F)
                {
                    do
                    {
                        food.Generate();
                    }
                    while(snake.OccupiesSquare(food.Location));
                }


                direction = cki.Key switch
                {
                    ConsoleKey.UpArrow => Direction.North,
                    ConsoleKey.DownArrow => Direction.South,
                    ConsoleKey.RightArrow => Direction.East,
                    ConsoleKey.LeftArrow => Direction.West,
                    _ => null,
                };

                if (int.TryParse(cki.KeyChar.ToString(), out var num))
                {
                    foodLocInputBuffer += num;
                    if (foodLocInputBuffer.Length == 4)
                    {
                        var x = Math.Min(int.Parse(foodLocInputBuffer.Substring(0, 2)), MapWidth);
                        var y = Math.Min(int.Parse(foodLocInputBuffer.Substring(2, 2)), MapHeight);
                        food.Location = (x, y);
                        foodLocInputBuffer = "";
                    }
                }
            }

            if(paused) return;


            List<double> neuralNetInputs = new List<double> { 1.0/snake.DistanceToFoodX,
                1.0/snake.DistanceToFoodY,
                1.0/snake.DistanceToNorthWall,
                1.0/snake.DistanceToSouthWall,
                1.0/snake.DistanceToEastWall,
                1.0/snake.DistanceToWestWall };

            nOut = manager.RunCurrent(neuralNetInputs);

            Dictionary<Direction, double> nnOutputs = new();

            nnOutputs.Add(Direction.North, nOut[0]);
            nnOutputs.Add(Direction.South, nOut[1]);
            nnOutputs.Add(Direction.East, nOut[2]);
            nnOutputs.Add(Direction.West, nOut[3]);

            direction = nnOutputs.OrderBy(kv => kv.Value).First().Key;

            if (((int)direction & 2) == ((int)snake.SnakeDirection & 2))
            {
                direction = nnOutputs.OrderBy(kv => kv.Value).ToList()[1].Key;
            }

            if (food.Eaten)
            {
                do
                {
                    food.Generate();
                }
                while(snake.OccupiesSquare(food.Location));
            }

            if(snake.Alive)
            {
                snake.Move(direction, food);
                manager.UpdateFitness(snake.Fitness);
                if (snake.Fitness > bestCurrentFitness)
                {
                    bestCurrentFitness = snake.Fitness;
                    if (bestCurrentFitness > bestOverallFitness)
                    {
                        bestOverallFitness = bestCurrentFitness;
                    }
                }
            }
            else
            {
                snake.Create();
                manager.Next();
                if (manager.Current == 0)
                {
                    bestCurrentFitness = 0;
                }
            }

            //Draw();
        }

        public static void Draw()
        {
            StringBuilder sb = new StringBuilder(MapWidth * (MapHeight + 5));

            for(int y = 0; y < MapHeight; y++)
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
                sb.AppendLine("|");
            }

            sb.AppendLine($"\nSnake Distances to Walls: N:{snake.DistanceToNorthWall} S:{snake.DistanceToSouthWall} W:{snake.DistanceToWestWall} E:{snake.DistanceToEastWall} F:{snake.DistanceToFood:F2} S:{snake.LookingAtFood} Length: {snake.Length} Head:{snake.Head} Food:{food.Location}");
            sb.AppendLine($"Generation: {manager.Generation}, Current: {manager.Current}, Fitness: {manager.Fitness}");
            sb.AppendLine($"BestLastFitness: {manager.BestLastFitness}, BestCurrentFitness:{bestCurrentFitness}, BestOverallFitness:{bestOverallFitness}");            //sb.AppendLine($"Snake Head: {snake.Head}");
            sb.AppendLine($"NN Outputs: {nOut[0]} {nOut[1]} {nOut[2]} {nOut[3]}");
            sb.AppendLine($"Fresh:{manager.FreshCount} Mutant:{manager.MutantCount} Child:{manager.ChildCount} Clone:{manager.CloneCount}");

            Console.Clear();
            Console.Write(sb.ToString());
        }
    }
}
