using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.BusinessLogic;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BusinessLogicImplementationUnitTest
    {
        [TestMethod]
        public void Start_CallsDataLayer_And_MapsBalls()
        {
            FakeDataLayer fakeDataLayer = new FakeDataLayer();

            using (var logic = new BusinessLogicImplementation(fakeDataLayer))
            {
                int numberOfBallsRequested = 5;
                int callbacksReceived = 0;

                logic.Start(numberOfBallsRequested, (position, ball) =>
                {
                    callbacksReceived++;
                    Assert.IsNotNull(position);
                    Assert.IsNotNull(ball);
                });

                Assert.IsTrue(fakeDataLayer.StartCalled);
                Assert.AreEqual(numberOfBallsRequested, fakeDataLayer.BallsRequested);
                Assert.AreEqual(2, callbacksReceived);
            }
        }

        [TestMethod]
        public async Task Logic_DetectsCollisions_And_UpdatesVelocities()
        {
            FakeDataLayer fakeDataLayer = new FakeDataLayer();

            using (var logic = new BusinessLogicImplementation(fakeDataLayer))
            {
                logic.Start(2, (p, b) => { });

                await Task.Delay(100);

                Assert.IsTrue(fakeDataLayer.VelocityUpdates.Count > 0, "Logika nie wykryła kolizji!");
            }
        }
    }

    #region Fake Data Layer for Testing

    internal class FakeDataLayer : DataAbstractAPI
    {
        public bool StartCalled { get; private set; } = false;
        public int BallsRequested { get; private set; } = -1;

        public List<(int id, double vx, double vy)> VelocityUpdates = new();

        private FakeDataBall ball1 = new FakeDataBall { Id = 1, X = 10, Y = 10, Radius = 10 };
        private FakeDataBall ball2 = new FakeDataBall { Id = 2, X = 15, Y = 10, Radius = 10 };

        public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
        {
            StartCalled = true;
            BallsRequested = numberOfBalls;

            upperLayerHandler(new FakeVector { x = ball1.X, y = ball1.Y }, ball1);
            upperLayerHandler(new FakeVector { x = ball2.X, y = ball2.Y }, ball2);
        }

        public override IReadOnlyList<Data.IBall> GetBalls() => new List<Data.IBall> { ball1, ball2 };

        public override void UpdateBallVelocity(int id, double vx, double vy)
        {
            VelocityUpdates.Add((id, vx, vy));
        }

        public override void Dispose() { }

        private record FakeVector : IVector
        {
            public double x { get; init; } = 0;
            public double y { get; init; } = 0;
        }

        private class FakeDataBall : Data.IBall
        {
            public int Id { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public double Vx => 1;
            public double Vy => 1;
            public double Radius { get; set; }
            public double Mass => 1;
            public IVector Velocity { get; set; } = new FakeVector();
            public event EventHandler<IVector>? NewPositionNotification;
        }
    }

    #endregion
}