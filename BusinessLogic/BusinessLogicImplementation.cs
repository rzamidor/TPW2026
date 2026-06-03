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
        private readonly DiagnosticLogger _logger;

        private readonly List<Data.IBall> _balls = new();
        private readonly object _logicLock = new();
        private CancellationTokenSource? _cts;

        public BusinessLogicImplementation(DataAbstractAPI? dataLayer = null)
        {
            _dataLayer = dataLayer ?? DataAbstractAPI.GetDataLayer();
            _logger = new DiagnosticLogger();
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

                dataBall.NewPositionNotification += (sender, newPos) =>
                {
                    _logger.LogBallData(dataBall.Id, newPos.x, newPos.y, dataBall.Velocity.x, dataBall.Velocity.y);
                };

                upperLayerHandler(pos, businessBall);
            });

            Task.Run(() => DetectCollisionsLoop(_cts.Token));
        }

        private async Task DetectCollisionsLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                lock (_logicLock)
                {
                    for (int i = 0; i < _balls.Count; i++)
                    {
                        for (int j = i + 1; j < _balls.Count; j++)
                        {
                            var ball1 = _balls[i];
                            var ball2 = _balls[j];

                            double dx = ball1.X - ball2.X;
                            double dy = ball1.Y - ball2.Y;
                            double distance = Math.Sqrt((dx * dx) + (dy * dy));

                            if (distance <= (ball1.Radius + ball2.Radius))
                            {
                                double tempVx = ball1.Vx;
                                double tempVy = ball1.Vy;

                                _dataLayer.UpdateBallVelocity(ball1.Id, ball2.Vx, ball2.Vy);
                                _dataLayer.UpdateBallVelocity(ball2.Id, tempVx, tempVy);
                            }
                        }
                    }
                }

                await Task.Delay(10, token);
            }
        }

        public override void Dispose()
        {
            _cts?.Cancel();
            _logger.Dispose();
            _dataLayer.Dispose();
        }
    }
}