using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data;

[assembly: InternalsVisibleTo("BusinessLogicTest")]

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        private readonly DataAbstractAPI _dataLayer;
        private readonly List<Data.IBall> _balls = new();
        private readonly object _logicLock = new();
        private CancellationTokenSource? _cts;

        public BusinessLogicImplementation(DataAbstractAPI? dataLayer = null)
        {
            _dataLayer = dataLayer ?? DataAbstractAPI.GetDataLayer();
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            _cts = new CancellationTokenSource();
            _balls.Clear();

            _dataLayer.Start(numberOfBalls, (vector, dataBall) =>
            {
                Ball businessBall = new Ball(dataBall);
                Position pos = new Position(vector.x, vector.y);

                lock (_logicLock)
                {
                    _balls.Add(dataBall);
                }

                upperLayerHandler(pos, businessBall);
            });

            Task.Run(() => DetectCollisionsLoop(_cts.Token));
        }

        private async Task DetectCollisionsLoop(CancellationToken token)
        {
            double width = BusinessLogicAbstractAPI.GetDimensions.TableWidth;
            double height = BusinessLogicAbstractAPI.GetDimensions.TableHeight;

            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMilliseconds(10));

            try
            {
                while (await timer.WaitForNextTickAsync(token))
                {
                    lock (_logicLock)
                    {
                        for (int i = 0; i < _balls.Count; i++)
                        {
                            var ball1 = _balls[i];

                            if ((ball1.X - ball1.Radius <= 0 && ball1.Vx < 0) ||
                                (ball1.X + ball1.Radius >= width && ball1.Vx > 0))
                            {
                                _dataLayer.UpdateBallVelocity(ball1.Id, -ball1.Vx, ball1.Vy);
                            }

                            if ((ball1.Y - ball1.Radius <= 0 && ball1.Vy < 0) ||
                                (ball1.Y + ball1.Radius >= height && ball1.Vy > 0))
                            {
                                _dataLayer.UpdateBallVelocity(ball1.Id, ball1.Vx, -ball1.Vy);
                            }

                            for (int j = i + 1; j < _balls.Count; j++)
                            {
                                var ball2 = _balls[j];
                                double dx = ball1.X - ball2.X;
                                double dy = ball1.Y - ball2.Y;
                                double distance = Math.Sqrt((dx * dx) + (dy * dy));

                                if (distance <= (ball1.Radius + ball2.Radius))
                                {

                                    double dvx = ball1.Vx - ball2.Vx;
                                    double dvy = ball1.Vy - ball2.Vy;

                                    if ((dx * dvx + dy * dvy) < 0)
                                    {
                                        double tempVx = ball1.Vx;
                                        double tempVy = ball1.Vy;
                                        _dataLayer.UpdateBallVelocity(ball1.Id, ball2.Vx, ball2.Vy);
                                        _dataLayer.UpdateBallVelocity(ball2.Id, tempVx, tempVy);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        public override void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _dataLayer.Dispose();
        }
    }
}