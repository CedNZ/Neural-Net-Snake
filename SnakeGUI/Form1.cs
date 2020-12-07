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
        const int MapHeight = Snake.Program.MapHeight;
        const int MapWidth = Snake.Program.MapWidth;
        public const int BlockSize = 4;
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
                var (brush, runId) = i switch
                {
                    1 => (Brushes.Green, Guid.Parse("588ead73-d85e-441c-9b8d-fb4d3e5db976")),
                    2 => (Brushes.Red, Guid.Parse("ba17c250-f6f0-4209-9b20-78984abe0925")),
                    3 => (Brushes.Blue, Guid.Parse("a72bce4b-b989-4f34-999c-9239aac20c4c")),
                    4 => (Brushes.Violet, Guid.Parse("60789889-2447-4250-92c0-279614b013b1")),
                    5 => (Brushes.Purple, Guid.Parse("1b642957-f0eb-41ff-a686-c94b80e53289")),
                    6 => (Brushes.Pink, Guid.Parse("52181c3b-3cea-4f95-b9df-7510c45dc383")),
                    7 => (Brushes.Orange, Guid.Parse("4bf030d5-7c4a-46ff-8613-dd4eb97f13c1")),
                    8 => (Brushes.DarkCyan, Guid.Parse("dc4cdbfc-d959-46a1-9161-789997f68164")),
                    9 => (Brushes.Magenta, Guid.Parse("b1b04b8d-6a87-48c0-a7c8-2add65db5fc2")),
                    10 => (Brushes.Indigo, Guid.Parse("29469f0f-2869-44b3-b499-9dd37e677f54")),
                    11 => (Brushes.YellowGreen, Guid.Parse("1d9bd106-5508-4696-b383-a26beb79254e")),
                    12 => (Brushes.Gold, Guid.Parse("6d8ca101-aadf-4c6a-924d-161126ff5143")),
                    13 => (Brushes.ForestGreen, Guid.Parse("31cde362-fbe0-4767-ad49-fa05d7ce85e4")),
                    14 => (Brushes.MediumAquamarine, Guid.Parse("2b5e06ef-7be9-4240-a061-5489475f863d")),
                    15 => (Brushes.SkyBlue, Guid.Parse("47bf4429-369e-4048-b78e-b731f37df3f7")),
                    16 => (Brushes.Silver, Guid.Parse("91915750-b850-4b0e-80cc-9a5f9286b8eb")),
                    17 => (Brushes.MidnightBlue, Guid.Parse("3c370aba-f136-412b-81dd-1c26c440c932")),
                    18 => (Brushes.Chocolate, Guid.Parse("80d5c462-7cb3-40d2-9235-43dc0c0e79fe")),
                    19 => (Brushes.Teal, Guid.Parse("0cc05e7d-48c5-4406-9ece-bed79ad75649")),
                    _ => (Brushes.Black, Guid.Parse("90d2380c-cff6-456d-8095-a67be615444e")),
                };

                games.Add((
                    new Snake.NeuralNet.Manager(layers: layers, populationSize: 100, loadPrevious: true, runId: runId),
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
                e.Graphics.DrawString($"G: {game.manager.Generation} C: {game.manager.Current} L: {snake.SnakeBody.Count() - 3} B:{game.manager.BestFitness}", DefaultFont, game.colour, 810, ((((MapHeight * BlockSize) / gameCount) - 1) * i));
                foreach(var snakePart in snake.SnakeBody)
                {
                    e.Graphics.FillRectangle(game.colour, snakePart.X * BlockSize, snakePart.Y * BlockSize, BlockSize, BlockSize);
                }
                e.Graphics.FillRectangle(game.colour, food.Location.X * BlockSize, food.Location.Y * BlockSize, BlockSize, BlockSize);
            }
        }

    }
}
