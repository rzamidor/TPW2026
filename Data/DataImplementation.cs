using System;
using System.Collections.Generic;
using System.Linq;
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
        private const double MASS = 1.0;

        private readonly object _lock = new();

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

            lock (_lock)
            {
                balls.Clear();

                for (int i = 0; i < numberOfBalls; i++)
                {
                    double x = random.NextDouble() * (WIDTH - 2 * RADIUS) + RADIUS;
                    double y = random.NextDouble() * (HEIGHT - 2 * RADIUS) + RADIUS;

                    double vx = random.NextDouble() * 4 - 2; // -2..2
                    double vy = random.NextDouble() * 4 - 2;

                    var ball = new Ball(i, new Vector(x, y), new Vector(vx, vy), RADIUS, MASS);

                    balls.Add(ball);

                    upperHandler(new Vector(x, y), ball);
                }
            }

            timer.Start();
        }

        private void OnTick(object? sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                foreach (var ball in balls)
                {
                    var delta = ball.Velocity;
                    ball.Move(delta, WIDTH, HEIGHT, ball.Radius);
                }
            }
        }

        public override IReadOnlyList<IBall> GetBalls()
        {
            lock (_lock)
                return balls.Cast<IBall>().ToList();
        }

        public override void UpdateBallVelocity(int id, double vx, double vy)
        {
            lock (_lock)
            {
                var ball = balls.FirstOrDefault(b => b.Id == id);
                if (ball != null)
                    ball.Velocity = new Vector(vx, vy);
            }
        }

        public override void Dispose()
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));

            timer.Stop();
            timer.Dispose();
            lock (_lock)
            {
                balls.Clear();
            }
            disposed = true;
        }
    }
}
