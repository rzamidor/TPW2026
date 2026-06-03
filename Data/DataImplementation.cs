using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        private readonly ConcurrentDictionary<int, Ball> balls = new();
        private readonly Random random = new();
        private Action<IVector, IBall>? upperHandler;
        private CancellationTokenSource? cts;
        private bool disposed = false;

        private const double WIDTH = 800;
        private const double HEIGHT = 400;
        private const double RADIUS = 10;
        private const double MASS = 1.0;

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (disposed) throw new ObjectDisposedException(nameof(DataImplementation));

            upperHandler = upperLayerHandler ?? throw new ArgumentNullException(nameof(upperLayerHandler));
            cts = new CancellationTokenSource();
            balls.Clear();

            for (int i = 0; i < numberOfBalls; i++)
            {
                double x = random.NextDouble() * (WIDTH - 2 * RADIUS) + RADIUS;
                double y = random.NextDouble() * (HEIGHT - 2 * RADIUS) + RADIUS;
                double vx = random.NextDouble() * 4 - 2; // -2..2
                double vy = random.NextDouble() * 4 - 2;

                var ball = new Ball(i, new Vector(x, y), new Vector(vx, vy), RADIUS, MASS);
                balls.TryAdd(i, ball);

                upperHandler(new Vector(x, y), ball);

                Task.Run(() => MoveBallLoopAsync(ball, cts.Token));
            }
        }

        private async Task MoveBallLoopAsync(Ball ball, CancellationToken token)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            const double speedMultiplier = 60.0;

            while (!token.IsCancellationRequested)
            {
                double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                stopwatch.Restart();

                double deltaX = ball.Velocity.x * elapsedSeconds * speedMultiplier;
                double deltaY = ball.Velocity.y * elapsedSeconds * speedMultiplier;

                ball.Move(new Vector(deltaX, deltaY));

                await Task.Delay(15, token);
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
            if (disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));

            cts?.Cancel();
            cts?.Dispose();

            balls.Clear();

            disposed = true;
        }
    }
}