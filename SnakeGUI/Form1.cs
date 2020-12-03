using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Snake;

namespace SnakeGUI
{
    public partial class Form1 : Form
    {
        private Timer timer;
        private Snake.NeuralNet.Manager manager;
        private Snake.Snake snake;
        private Snake.Food food;
        const int MapHeight = 250;
        const int MapWidth = 400;
        static int[] layers = new[] { 6, 5, 4, 4 };
        private int snakeCount;

        public Form1()
        {
            InitializeComponent();
            manager = new Snake.NeuralNet.Manager(layers);
            snake = new Snake.Snake(MapWidth, MapHeight);
            food = new Food(MapWidth, MapHeight);

            snakeCount = 0;

            timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 10;

            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
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
                snakeCount++;
            }

            this.Refresh();
        }

        private void Game_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawString($"{snakeCount} Length: {snake.SnakeBody.Count() - 3}", DefaultFont, Brushes.Black, 10, 10);
            foreach (var snakePart in snake.SnakeBody)
            {
                e.Graphics.FillRectangle(Brushes.Red, snakePart.X*2, snakePart.Y*2, 2, 2);
            }
            e.Graphics.FillRectangle(Brushes.Green, food.Location.X*2, food.Location.Y*2, 2, 2);
        }

    }
}
