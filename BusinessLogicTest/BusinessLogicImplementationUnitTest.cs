using Microsoft.VisualStudio.TestTools.UnitTesting;
using TP.ConcurrentProgramming.BusinessLogic;
using TP.ConcurrentProgramming.Data;
using System;
using System.Collections.Generic;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BusinessLogicImplementationUnitTest
    {
        [TestMethod]
        public void StartCallsDataLayer()
        {
            FakeDataLayer data = new FakeDataLayer();
            BusinessLogicAPI logic = BusinessLogicAPI.Create(data);

            logic.Start(5);

            Assert.IsTrue(data.StartCalled);
            Assert.AreEqual(5, data.BallsRequested);
        }

        [TestMethod]
        public void UpdateVelocityCallsDataLayer()
        {
            FakeDataLayer data = new FakeDataLayer();
            BusinessLogicAPI logic = BusinessLogicAPI.Create(data);

            logic.UpdateBallVelocity(1, 2, 3);

            Assert.AreEqual(1, data.VelocityUpdates.Count);
            Assert.AreEqual((1, 2.0, 3.0), data.VelocityUpdates[0]);
        }
    }

    #region testing instrumentation

    internal class FakeDataLayer : DataAbstractAPI
    {
        public bool StartCalled { get; private set; } = false;
        public int BallsRequested { get; private set; } = -1;

        public List<IBall> Balls { get; } = new();
        public List<(int id, double vx, double vy)> VelocityUpdates { get; } = new();

        public override void Start(int numberOfBalls, Action<IVector, IBall> handler)
        {
            StartCalled = true;
            BallsRequested = numberOfBalls;

            handler(new VectorFixture(), new BallFixture());
        }

        public override IReadOnlyList<IBall> GetBalls() => Balls;

        public override void UpdateBallVelocity(int id, double vx, double vy)
        {
            VelocityUpdates.Add((id, vx, vy));
        }

        public override void Dispose() { }

        private record VectorFixture : IVector
        {
            public double x { get; init; }
            public double y { get; init; }
        }

        private class BallFixture : IBall
        {
            public int Id { get; set; } = 1;
            public double X { get; set; } = 0;
            public double Y { get; set; } = 0;
            public double Vx { get; set; } = 0;
            public double Vy { get; set; } = 0;
            public double Radius { get; set; } = 10;
            public double Mass { get; set; } = 1;

            public IVector Velocity
            {
                get => new VectorFixture { x = Vx, y = Vy };
                set { Vx = value.x; Vy = value.y; }
            }

            public event EventHandler<IVector>? NewPositionNotification;
        }
    }

    #endregion
}
