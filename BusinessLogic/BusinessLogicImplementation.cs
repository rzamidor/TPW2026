using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using TP.ConcurrentProgramming.Data;

[assembly: InternalsVisibleTo("BusinessLogicTest")]

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        private readonly DataAbstractAPI _dataLayer;
        private readonly List<Data.IBall> _balls = new();
        private readonly object _logicLock = new();

        // Timer wyliczający fizykę
        private Timer? _collisionTimer;

        private bool _disposed = false;

        public BusinessLogicImplementation(DataAbstractAPI? dataLayer = null)
        {
            _dataLayer = dataLayer ?? DataAbstractAPI.GetDataLayer();
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(BusinessLogicImplementation));

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

            _collisionTimer = new Timer(DetectCollisionsCallback, null, 0, 10);
        }

        private void DetectCollisionsCallback(object? state)
        {
            double width = BusinessLogicAbstractAPI.GetDimensions.TableWidth;
            double height = BusinessLogicAbstractAPI.GetDimensions.TableHeight;

            lock (_logicLock)
            {
                for (int i = 0; i < _balls.Count; i++)
                {
                    var ball1 = _balls[i];

                    // Krawędzie
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

                    // Zderzenia
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

        public override void Dispose()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(BusinessLogicImplementation));

            _collisionTimer?.Dispose();
            _dataLayer.Dispose();
        }
    }
}