using Snake;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Snake.NeuralNet;

namespace SnakeGUI
{
    public partial class Form1 : Form
    {
        private Timer timer;
        const int MapHeight = Snake.Program.MapHeight;
        const int MapWidth = Snake.Program.MapWidth;
        public const int BlockSize = 4;
        //static int[] layers = new[] { 6, 5, 4, 4 };
        //private int snakeCount;
        private int gameCount = 20;
        List<(Manager manager, Snake.Snake snake, Food food, Brush colour)> games;

        private readonly BufferedGraphics bufferedGraphics;
        private readonly BufferedGraphicsContext context;

        public Form1()
        {
            InitializeComponent();
            games = new List<(Manager manager, Snake.Snake snake, Food food, Brush colour)>(gameCount);

            for(int i = 0; i < gameCount; i++)
            {
                var (brush, runId, activationFunc, layers) = i switch
                {
                    1 => (Brushes.Green, Guid.Parse("588ead73-d85e-441c-9b8d-fb4d3e5db976"), ActivationFunctions.TanH, new[] { 6, 4 }),
                    2 => (Brushes.Red, Guid.Parse("ba17c250-f6f0-4209-9b20-78984abe0925"), ActivationFunctions.ReLU, new[] { 6, 4 }),
                    3 => (Brushes.Blue, Guid.Parse("a72bce4b-b989-4f34-999c-9239aac20c4c"), ActivationFunctions.Softplus, new[] { 6, 4 }),
                    4 => (Brushes.Violet, Guid.Parse("60789889-2447-4250-92c0-279614b013b1"), ActivationFunctions.TanH, new[] { 6, 3, 4 }),
                    5 => (Brushes.Purple, Guid.Parse("1b642957-f0eb-41ff-a686-c94b80e53289"), ActivationFunctions.ReLU, new[] { 6, 3, 4 }),
                    6 => (Brushes.Pink, Guid.Parse("52181c3b-3cea-4f95-b9df-7510c45dc383"), ActivationFunctions.Softplus, new[] { 6, 3, 4 }),
                    7 => (Brushes.Orange, Guid.Parse("4bf030d5-7c4a-46ff-8613-dd4eb97f13c1"), ActivationFunctions.TanH, new[] { 6, 4, 4 }),
                    8 => (Brushes.DarkCyan, Guid.Parse("dc4cdbfc-d959-46a1-9161-789997f68164"), ActivationFunctions.ReLU, new[] { 6, 4, 4 }),
                    9 => (Brushes.Magenta, Guid.Parse("b1b04b8d-6a87-48c0-a7c8-2add65db5fc2"), ActivationFunctions.Softplus, new[] { 6, 4, 4 }),
                    10 => (Brushes.Indigo, Guid.Parse("29469f0f-2869-44b3-b499-9dd37e677f54"), ActivationFunctions.TanH, new[] { 6, 5, 4 }),
                    11 => (Brushes.YellowGreen, Guid.Parse("1d9bd106-5508-4696-b383-a26beb79254e"), ActivationFunctions.ReLU, new[] { 6, 5, 4 }),
                    12 => (Brushes.Gold, Guid.Parse("6d8ca101-aadf-4c6a-924d-161126ff5143"), ActivationFunctions.Softplus, new[] { 6, 5, 4 }),
                    13 => (Brushes.ForestGreen, Guid.Parse("31cde362-fbe0-4767-ad49-fa05d7ce85e4"), ActivationFunctions.TanH, new[] { 6, 3, 3, 4 }),
                    14 => (Brushes.MediumAquamarine, Guid.Parse("2b5e06ef-7be9-4240-a061-5489475f863d"), ActivationFunctions.ReLU, new[] { 6, 3, 3, 4 }),
                    15 => (Brushes.SkyBlue, Guid.Parse("47bf4429-369e-4048-b78e-b731f37df3f7"), ActivationFunctions.Softplus, new[] { 6, 3, 3, 4 }),
                    16 => (Brushes.Silver, Guid.Parse("91915750-b850-4b0e-80cc-9a5f9286b8eb"), ActivationFunctions.TanH, new[] { 6, 4, 4, 4 }),
                    17 => (Brushes.MidnightBlue, Guid.Parse("3c370aba-f136-412b-81dd-1c26c440c932"), ActivationFunctions.ReLU, new[] { 6, 4, 4, 4 }),
                    18 => (Brushes.Chocolate, Guid.Parse("80d5c462-7cb3-40d2-9235-43dc0c0e79fe"), ActivationFunctions.Softplus, new[] { 6, 4, 4, 4 }),
                    19 => (Brushes.Teal, Guid.Parse("0cc05e7d-48c5-4406-9ece-bed79ad75649"), ActivationFunctions.TanH, new[] { 6, 6, 5, 4 }),
                    _ => (Brushes.Black, Guid.Parse("90d2380c-cff6-456d-8095-a67be615444e"), ActivationFunctions.ReLU, new[] { 6, 6, 5, 4 }),
                };

                if (gameCount == 1)
                {
                    runId = Guid.NewGuid();
                }

                games.Add((
                    new Manager(layers: layers, populationSize: 100, loadPrevious: true, runId: runId, loadFrom: @"C:\Temp\Snake2021-ActivationFunc"),
                    new Snake.Snake(MapWidth, MapHeight),
                    new Food(MapWidth, MapHeight),
                    brush));
            }

            timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 1;

            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(Width + 1, Height + 1);
            bufferedGraphics = context.Allocate(CreateGraphics(), new Rectangle(0, 0, Width, Height));

            DrawToBuffer(bufferedGraphics.Graphics);

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
                Manager manager = game.manager;
                Snake.Snake snake = game.snake;
                Food food = game.food;
                Direction direction;

                List<double> neuralNetInputs = new List<double> { 1.0/snake.DistanceToFoodX,
                snake.DistanceToFoodY,
                1.0/snake.DistanceToNorthWall,
                1.0/snake.DistanceToSouthWall,
                1.0/snake.DistanceToEastWall,
                1.0/snake.DistanceToWestWall };

                var nOut = manager.RunCurrent(neuralNetInputs).Select(Math.Abs).ToList();

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
                }
            }
            DrawToBuffer(bufferedGraphics.Graphics);
            this.Refresh();
        }

        private void DrawToBuffer(Graphics g)
        {
            g.FillRectangle(Brushes.AliceBlue, 0, 0, Width, Height);
            int i = 0;
            foreach (var game in games.OrderByDescending(g => g.manager.BestFitness))
            {
                var snake = game.snake;
                var food = game.food;
                var brushName = ((SolidBrush)game.colour).Color.Name;
                g.DrawString($"G: {game.manager.Generation} C: {game.manager.Current} L: {snake.SnakeBody.Count() - 3} B:{game.manager.BestFitness}  {brushName}", DefaultFont, game.colour, 810, ((((MapHeight * (BlockSize)) / gameCount) - 1) * i));
                foreach (var snakePart in snake.SnakeBody)
                {
                    g.FillRectangle(game.colour, snakePart.X * BlockSize, snakePart.Y * BlockSize, BlockSize, BlockSize);
                }
                g.FillRectangle(game.colour, food.Location.X * BlockSize, food.Location.Y * BlockSize, BlockSize, BlockSize);

                i++;
            }
        }

        private void Game_Paint(object sender, PaintEventArgs e)
        {
            bufferedGraphics.Render(e.Graphics);
        }

    }
}
