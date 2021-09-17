﻿using Snake;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Snake.NeuralNet;
using System.ComponentModel;

namespace SnakeGUI
{
    public partial class SnakeGUI : Form
    {
        private Timer timer;
        const int MapHeight = Snake.Program.MapHeight;
        const int MapWidth = Snake.Program.MapWidth;
        public const int BlockSize = 4;

        List<(Manager manager, Snake.Snake snake, Food food, Brush colour)> _games;

        private readonly BufferedGraphics bufferedGraphics;
        private readonly BufferedGraphicsContext context;
        private bool clearBuffer = true;

        public SnakeGUI(List<(Manager manager, Snake.Snake snake, Food food, Brush colour)> games)
        {
            InitializeComponent();

            _games = games;

            timer = new Timer();
            timer.Tick += Timer_Tick;
            timer.Interval = 10;

            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(Width + 1, Height + 1);
            bufferedGraphics = context.Allocate(CreateGraphics(), new Rectangle(0, 0, Width, Height));

            DrawToBuffer(bufferedGraphics.Graphics);

            timer.Start();
            //while (true)
            //{
            //    Timer_Tick(this, EventArgs.Empty);
            //}
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DrawToBuffer(bufferedGraphics.Graphics);
            this.Refresh();
        }

        private void DrawToBuffer(Graphics g)
        {
            if (clearBuffer)
            {
                g.FillRectangle(Brushes.AliceBlue, 0, 0, Width, Height);
            }

            int i = 0;
            int gameCount = _games.Count();
            foreach (var game in _games.OrderByDescending(g => g.manager.BestFitness))
            {
                var snake = game.snake;
                var food = game.food;
                var brushName = ((SolidBrush)game.colour).Color.Name;
                g.DrawString($"G: {game.manager.Generation} C: {game.manager.Current} L: {snake.SnakeBody.Count() - 3} B:{game.manager.BestFitness}  {brushName}", DefaultFont, game.colour, 810, ((((MapHeight * (BlockSize)) / gameCount) - 1) * i++));
                foreach (var snakePart in snake.SnakeBody)
                {
                    g.FillRectangle(game.colour, snakePart.X * BlockSize, snakePart.Y * BlockSize, BlockSize, BlockSize);
                }
                g.FillRectangle(game.colour, food.Location.X * BlockSize, food.Location.Y * BlockSize, BlockSize, BlockSize);
            }
        }

        private void Game_Paint(object sender, PaintEventArgs e)
        {
            bufferedGraphics.Render(e.Graphics);
        }

        private void KeyPress_Handler(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 't')
            {
                clearBuffer = !clearBuffer;
            }
        }
    }
}