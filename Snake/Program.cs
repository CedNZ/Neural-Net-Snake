﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Snake
{
    class Program
    {
        static ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        const int MapWidth = 100;
        const int MapHeight = 50;
        static Snake snake;
        static Timer timer;
        static Food food;
        static List<double> nOut;
        static bool paused;
        static bool tick;

        static double learningRate = 0.5;
        static int[] layers = new[] { 5, 4, 4 };

        static int tickInterval = 10;

        static NeuralNet.NeuralNetwork neuralNet;
        static NeuralNet.NeuralNetwork[] neuralNetworks;
        static int generation;
        static int current;
        const float MutateChance = 0.01f;
        const float MutationStrength = 0.5f;
        const int population = 40;
        static double bestCurrentFitness;

        static Guid runId;
        static string outputFile;

        static char[] map;
        static string folder = $@"C:\temp\SnakeAI";

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            snake = new Snake(MapWidth, MapHeight);
            food = new Food(MapWidth, MapHeight);

            generation = 0;
            current = 0;

            neuralNetworks = new NeuralNet.NeuralNetwork[population];
            for(int i = 0; i < population; i++)
            {
                neuralNetworks[i] = new NeuralNet.NeuralNetwork(learningRate, layers);
                neuralNetworks[i].Load(folder, i);
            }

            runId = Guid.NewGuid();

            outputFile = $@"{folder}\{runId}";

            map = new char[(MapWidth + 2) * (MapHeight)];
            Array.Fill(map, ' ');
            for (int y = 0; y < MapHeight; y++)
            {
                map[(y * MapWidth) + (MapWidth)] = '|';
                map[(y * MapWidth) + (MapWidth + 1)] = '\n';
            }

            timer = new Timer(Tick, _manualResetEvent, 100, tickInterval);
            //while(true)
            //{
            //    Tick(_manualResetEvent);
            //}

            _manualResetEvent.WaitOne();
        }

        private static void Tick(object state)
        {
            if (current == population)
            {
                current = 0;
            }
            neuralNet = neuralNetworks[current];

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
                    neuralNet = new NeuralNet.NeuralNetwork(learningRate, layers);
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
                    Next();
                    return;
                }


                direction = cki.Key switch
                {
                    ConsoleKey.UpArrow => Direction.North,
                    ConsoleKey.DownArrow => Direction.South,
                    ConsoleKey.RightArrow => Direction.East,
                    ConsoleKey.LeftArrow => Direction.West,
                    _ => null,
                };
            }

            if(paused) return;


            List<double> neuralNetInputs = new List<double> { 1.0/snake.DistanceToFood,
                1.0/snake.DistanceToNorthWall,
                1.0/snake.DistanceToSouthWall,
                1.0/snake.DistanceToEastWall,
                1.0/snake.DistanceToWestWall };

            var outputs = neuralNet.Run(neuralNetInputs).ToList();

            nOut = outputs;            

            var highest = outputs.IndexOf(outputs.Max());

            direction = highest switch
            {
                0 => Direction.North,
                1 => Direction.South,
                2 => Direction.East,
                3 => Direction.West,
                _ => null,
            };



            if(food.Eaten)
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
                neuralNet.Fitness = snake.Fitness;
                if (snake.Fitness > bestCurrentFitness)
                {
                    bestCurrentFitness = snake.Fitness;
                }
            }
            else
            {
                foreach(var segment in snake.SnakeBody)
                {
                    map[segment.Y * MapWidth + segment.X] = ' ';
                }
                
                Next();
            }

            Draw();
        }

        public static void Next()
        {
            snake.Create();

            current++;
            if(current == population)
            {
                Array.Sort(neuralNetworks);
                neuralNetworks.Last().Save(outputFile, generation);

                current = 0;
                generation++;
                for(int i = 0; i < population - 1; i++)
                {
                    if(i < population / 10)
                    {
                        neuralNetworks[i] = new NeuralNet.NeuralNetwork(learningRate, layers);
                    }
                    else if(i >= (population - 3))
                    {
                        neuralNetworks[i].Mutate(10, 0.2f);
                    }
                    else
                    {
                        neuralNetworks[i] = neuralNetworks[population - (i % 2) - 1].Clone();
                        neuralNetworks[i].Mutate((int)(1 / MutateChance), MutationStrength);
                    }
                }
            }
        }

        public static void Draw()
        {
            map[food.Location.Y * MapWidth + food.Location.X] = 'ᴥ';
            map[snake.Last.Y * MapWidth + snake.Last.X] = ' ';

            for(int y = 0; y < MapHeight; y++)
            {
                map[(y * MapWidth) + (MapWidth)] = '|';
                map[(y * MapWidth) + (MapWidth + 1)] = '\n';
            }
            foreach(var segment in snake.SnakeBody)
            {
                map[segment.Y * MapWidth + segment.X] = '●';
            }


            StringBuilder sb = new StringBuilder((MapWidth + 2) * (MapHeight + 5));

            sb.Append(new string(map));

            sb.AppendLine($"\nSnake Distances to Walls: N:{snake.DistanceToNorthWall} S:{snake.DistanceToSouthWall} W:{snake.DistanceToWestWall} E:{snake.DistanceToEastWall} F:{snake.DistanceToFood:F2} Length: {snake.Length}");
            sb.AppendLine($"Generation: {generation}, Current: {current}, Fitness: {neuralNet.Fitness}.\t BestLastFitness: {neuralNetworks.Last().Fitness}, BestCurrentFitness:{bestCurrentFitness}");            //sb.AppendLine($"Snake Head: {snake.Head}");
            sb.AppendLine($"NN Outputs: {nOut[0]} {nOut[1]} {nOut[2]} {nOut[3]}");

            Console.Clear();
            Console.Write(sb.ToString());
        }
    }
}
