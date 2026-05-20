using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
                Assert.AreEqual(1, callbacksReceived); 
            }
        }
    }

    #region Fake Data Layer for Testing

    internal class FakeDataLayer : DataAbstractAPI
    {
        public bool StartCalled { get; private set; } = false;
        public int BallsRequested { get; private set; } = -1;

        public override void Start(int numberOfBalls, Action<IVector, Data.IBall> upperLayerHandler)
        {
            StartCalled = true;
            BallsRequested = numberOfBalls;

            upperLayerHandler(new FakeVector(), new FakeDataBall());
        }

        public override IReadOnlyList<Data.IBall> GetBalls() => new List<Data.IBall>();

        public override void UpdateBallVelocity(int id, double vx, double vy) { }

        public override void Dispose() { }

        private record FakeVector : IVector
        {
            public double x { get; init; } = 0;
            public double y { get; init; } = 0;
        }

        private class FakeDataBall : Data.IBall
        {
            public int Id => 1;
            public double X => 0;
            public double Y => 0;
            public double Vx => 0;
            public double Vy => 0;
            public double Radius => 10;
            public double Mass => 1;
            public IVector Velocity { get; set; } = new FakeVector();
            public event EventHandler<IVector>? NewPositionNotification;
        }
    }

    #endregion
}