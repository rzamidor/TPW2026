using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TP.ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        private readonly ConcurrentDictionary<int, Ball> balls = new();

        private readonly ConcurrentDictionary<int, Timer> ballTimers = new();

        private readonly Random random = new();
        private readonly DiagnosticLogger logger = new();
        private Action<IVector, IBall>? upperHandler;
        private bool disposed = false;

        private const double WIDTH = 800;
        private const double HEIGHT = 400;
        private const double RADIUS = 10;
        private const double MASS = 1.0;

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (disposed) throw new ObjectDisposedException(nameof(DataImplementation));

            upperHandler = upperLayerHandler ?? throw new ArgumentNullException(nameof(upperLayerHandler));
            balls.Clear();

            for (int i = 0; i < numberOfBalls; i++)
            {
                double x = random.NextDouble() * (WIDTH - 2 * RADIUS) + RADIUS;
                double y = random.NextDouble() * (HEIGHT - 2 * RADIUS) + RADIUS;
                double vx = random.NextDouble() * 4 - 2;
                double vy = random.NextDouble() * 4 - 2;

                var ball = new Ball(i, new Vector(x, y), new Vector(vx, vy), RADIUS, MASS);
                balls.TryAdd(i, ball);

                ball.NewPositionNotification += (sender, newPos) =>
                {
                    logger.LogBallData(ball.Id, newPos.x, newPos.y, ball.Velocity.x, ball.Velocity.y);
                };

                upperHandler(new Vector(x, y), ball);

                Timer timer = new Timer(MoveBallCallback, ball, 0, 15);
                ballTimers.TryAdd(i, timer);
            }
        }

        private void MoveBallCallback(object? state)
        {
            if (state is Ball ball)
            {
                const double elapsedSeconds = 0.015;
                const double speedMultiplier = 60.0;

                double deltaX = ball.Velocity.x * elapsedSeconds * speedMultiplier;
                double deltaY = ball.Velocity.y * elapsedSeconds * speedMultiplier;

                ball.Move(new Vector(deltaX, deltaY));
            }
        }

        public override IReadOnlyList<IBall> GetBalls()
        {
            return balls.Values.Cast<IBall>().ToList();
        }

        public override void UpdateBallVelocity(int id, double vx, double vy)
        {
            if (balls.TryGetValue(id, out var ball))
            {
                ball.Velocity = new Vector(vx, vy);
            }
        }

        public override void Dispose()
        {
            if (disposed) throw new ObjectDisposedException(nameof(DataImplementation));

            // Zatrzymujemy i niszczymy wszystkie Timery kul
            foreach (var timer in ballTimers.Values)
            {
                timer.Dispose();
            }
            ballTimers.Clear();
            balls.Clear();

            logger.Dispose();
            disposed = true;
        }
    }
}