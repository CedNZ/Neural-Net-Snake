using Snake;
using Snake.NeuralNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGUI
{
    public class GameRunner : ApplicationContext
    {
        private int _gameCount = 60;
        private List<GameWrapper> _games;

        public List<GameWrapper> Games => _games;

        const int MapHeight = Snake.Program.MapHeight;
        const int MapWidth = Snake.Program.MapWidth;

        public GameRunner()
        {
            Application.ApplicationExit += new EventHandler(OnApplicationExit);

            _games = new List<GameWrapper>(_gameCount);

            for (int i = 0; i < _gameCount; i++)
            {
                var (brush, runId, activationFunc, layers) = (i % 20) switch
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

                if (_gameCount == 1)
                {
                    runId = Guid.NewGuid();
                }

                Games.Add(new GameWrapper(
                    new Manager(layers: layers, populationSize: 100, loadPrevious: false, runId: null, loadFrom: @"C:\Temp\SnakeResults", activationFunction: activationFunc),
                    new Snake.Snake(MapWidth, MapHeight),
                    new Food(MapWidth, MapHeight),
                    brush));
            }

            ResultsForm.Show();

            Timer timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 1;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Run();
            if (!SnakeForm.Visible && !ResultsForm.Visible)
            {
                Application.Exit();
            }
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {

        }

        public void Run()
        {
            foreach (var game in Games)
            {
                Manager manager = game.Manager;
                Snake.Snake snake = game.Snake;
                Food food = game.Food;
                Direction direction;

                List<double> neuralNetInputs = new List<double>
                {
                    1.0/snake.DistanceToFoodX,
                    1.0/snake.DistanceToFoodY,
                    1.0/snake.DistanceToNorthWall,
                    1.0/snake.DistanceToSouthWall,
                    1.0/snake.DistanceToEastWall,
                    1.0/snake.DistanceToWestWall
                };

                var nOut = manager.RunCurrent(neuralNetInputs).Select(Math.Abs).ToList();

                Dictionary<Direction, double> nnOutputs = new();

                nnOutputs.Add(Direction.North, nOut[0]);
                nnOutputs.Add(Direction.South, nOut[1]);
                nnOutputs.Add(Direction.East, nOut[2]);
                nnOutputs.Add(Direction.West, nOut[3]);

                direction = nnOutputs.OrderBy(kv => kv.Value).First().Key;

                //if(((int)direction & 2) == ((int)snake.SnakeDirection & 2))
                //{
                //    direction = nnOutputs.OrderBy(kv => kv.Value).ToList()[1].Key;
                //}

                if (food.Eaten)
                {
                    do
                    {
                        food.Generate();
                    }
                    while (snake.OccupiesSquare(food.Location));
                }

                if (snake.Alive)
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

            if (Games.Any(x => x.DrawGame))
            {
                SnakeForm.Show();
            }
            else
            {
                SnakeForm.Hide();
            }
        }

        private ResultsForm _resultsForm;

        public ResultsForm ResultsForm
        {
            get
            {
                return _resultsForm ??= new ResultsForm(Games);
            }
            set
            {
                _resultsForm = value;
            }
        }

        private SnakeGUI _snakeForm;

        public SnakeGUI SnakeForm
        {
            get
            {
                return _snakeForm ??= new SnakeGUI(Games, () => ResultsForm.Show());
            }
            set
            {
                _snakeForm = value;
            }
        }
    }

    public class GameWrapper
    {
        public GameWrapper(Manager manager, Snake.Snake snake, Food food, Brush colour)
        {
            Manager = manager;
            Snake = snake;
            Food = food;
            Colour = colour;
            DrawGame = false;
        }

        private Manager _manager;
        public Manager Manager { get => _manager; set => _manager = value; }

        private Snake.Snake _snake;
        public Snake.Snake Snake { get => _snake; set => _snake = value; }

        private Food _food;
        public Food Food { get => _food; set => _food = value; }

        private Brush _colour;
        public Brush Colour { get => _colour; set => _colour = value; }


        private bool _drawGame;
        public bool DrawGame { get => _drawGame; set => _drawGame = value; }
    }
}
