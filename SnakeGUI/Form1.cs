using Snake;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SnakeGUI
{
    public partial class Form1 : Form
    {
        private Timer timer;
        const int MapHeight = 125;
        const int MapWidth = 200;
        const int blockSize = 4;
        static int[] layers = new[] { 6, 5, 4, 4 };
        private int snakeCount;
        private int gameCount = 20;
        List<(Snake.NeuralNet.Manager manager, Snake.Snake snake, Food food, Brush colour)> games;

        public Form1()
        {
            InitializeComponent();
            games = new List<(Snake.NeuralNet.Manager manager, Snake.Snake snake, Food food, Brush colour)>(gameCount);

            for(int i = 0; i < gameCount; i++)
            {
                var brush = i switch
                {
                    1 => Brushes.Green,
                    2 => Brushes.Red,
                    3 => Brushes.Blue,
                    4 => Brushes.Violet,
                    5 => Brushes.Purple,
                    6 => Brushes.Pink,
                    7 => Brushes.Orange,
                    8 => Brushes.DarkCyan,
                    9 => Brushes.Magenta,
                    10 => Brushes.Indigo,
                    11 => Brushes.YellowGreen,
                    12 => Brushes.Gold,
                    13 => Brushes.ForestGreen,
                    14 => Brushes.MediumAquamarine,
                    15 => Brushes.SkyBlue,
                    16 => Brushes.Silver,
                    17 => Brushes.MidnightBlue,
                    18 => Brushes.Chocolate,
                    19 => Brushes.Teal,
                    _ => Brushes.Black,
                };

                games.Add((
                    new Snake.NeuralNet.Manager(layers, 100),
                    new Snake.Snake(MapWidth, MapHeight),
                    new Food(MapWidth, MapHeight),
                    brush));
            }

            snakeCount = 0;

            timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 1;

            timer.Start();
            //while(true)
            //{
            //    Timer_Tick(this, EventArgs.Empty);
            //}
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach(var game in games)
            {
                Snake.NeuralNet.Manager manager = game.manager;
                Snake.Snake snake = game.snake;
                Snake.Food food = game.food;
                Direction direction;

                List<double> neuralNetInputs = new List<double> { 1.0/snake.DistanceToFood,
                snake.LookingAtFood,
                1.0/snake.DistanceToNorthWall,
                1.0/snake.DistanceToSouthWall,
                1.0/snake.DistanceToEastWall,
                1.0/snake.DistanceToWestWall };

                var nOut = manager.RunCurrent(neuralNetInputs);

                Dictionary<Direction, double> nnOutputs = new();

                nnOutputs.Add(Direction.North, nOut[0]);
                nnOutputs.Add(Direction.South, nOut[1]);
                nnOutputs.Add(Direction.East, nOut[2]);
                nnOutputs.Add(Direction.West, nOut[3]);

                direction = nnOutputs.OrderBy(kv => kv.Value).First().Key;

                if(((int)direction & 2) == ((int)snake.SnakeDirection & 2))
                {
                    direction = nnOutputs.OrderBy(kv => kv.Value).ToList()[1].Key;
                }



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
                    manager.UpdateFitness(snake.Fitness);
                }
                else
                {
                    snake.Create();
                    manager.Next();
                    snakeCount++;
                }
            }
            this.Refresh();
        }

        private void Game_Paint(object sender, PaintEventArgs e)
        {
            int i = 0;
            foreach(var game in games.OrderByDescending(g => g.manager.BestFitness))
            {
                i++;
                var snake = game.snake;
                var food = game.food;
                e.Graphics.DrawString($"G: {game.manager.Generation} C: {game.manager.Current} L: {snake.SnakeBody.Count() - 3} B:{game.manager.BestFitness}", DefaultFont, game.colour, 810, ((((MapHeight * blockSize) / gameCount) - 1) * i));
                foreach(var snakePart in snake.SnakeBody)
                {
                    e.Graphics.FillRectangle(game.colour, snakePart.X * blockSize, snakePart.Y * blockSize, blockSize, blockSize);
                }
                e.Graphics.FillRectangle(game.colour, food.Location.X * blockSize, food.Location.Y * blockSize, blockSize, blockSize);
            }
        }

    }
}
