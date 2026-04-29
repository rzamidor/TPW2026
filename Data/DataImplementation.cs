//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System;
using System.Collections.Generic;
using System.Timers;

namespace TP.ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        private readonly List<Ball> balls = new();
        private readonly System.Timers.Timer timer;
        private readonly Random random = new();
        private Action<IVector, IBall> upperHandler;
        private bool disposed = false;

        private const double WIDTH = 800;
        private const double HEIGHT = 400;
        private const double RADIUS = 10;

        public DataImplementation()
        {
            timer = new System.Timers.Timer(20); // 50 FPS
            timer.Elapsed += OnTick;
        }

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));

            upperHandler = upperLayerHandler ?? throw new ArgumentNullException(nameof(upperLayerHandler));

            balls.Clear();

            for (int i = 0; i < numberOfBalls; i++)
            {
                double x = random.NextDouble() * (WIDTH - 2 * RADIUS) + RADIUS;
                double y = random.NextDouble() * (HEIGHT - 2 * RADIUS) + RADIUS;

                double vx = random.NextDouble() * 4 - 2; // -2..2
                double vy = random.NextDouble() * 4 - 2;

                var ball = new Ball(new Vector(x, y), new Vector(vx, vy));

                balls.Add(ball);

                // notify Logic layer about the new ball
                upperHandler(new Vector(x, y), ball);
            }

            timer.Start();
        }

        private void OnTick(object sender, ElapsedEventArgs e)
        {
            foreach (var ball in balls)
            {
                // use velocity as delta
                var delta = ball.Velocity;

                ball.Move(delta, WIDTH, HEIGHT, RADIUS);
            }
        }

        public override void Dispose()
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));

            timer.Stop();
            timer.Dispose();
            balls.Clear();
            disposed = true;
        }
    }
}
