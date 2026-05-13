//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            Vector pos = new Vector(0.0, 0.0);
            Vector vel = new Vector(1.0, 1.0);

            Ball ball = new Ball(1, pos, vel, 10, 1);

            Assert.AreEqual(1, ball.Id);
            Assert.AreEqual(0.0, ball.X);
            Assert.AreEqual(0.0, ball.Y);
            Assert.AreEqual(1.0, ball.Vx);
            Assert.AreEqual(1.0, ball.Vy);
            Assert.AreEqual(10, ball.Radius);
            Assert.AreEqual(1, ball.Mass);
        }

        [TestMethod]
        public void MoveRaisesEvent()
        {
            Ball ball = new Ball(1, new Vector(10, 10), new Vector(0, 0), 10, 1);

            int calls = 0;
            IVector lastPos = new Vector(0, 0);

            ball.NewPositionNotification += (sender, pos) =>
            {
                calls++;
                lastPos = pos;
            };

            ball.Move(new Vector(5, 5), 200, 200, 10);

            Assert.AreEqual(1, calls);
            Assert.AreEqual(15, lastPos.x);
            Assert.AreEqual(15, lastPos.y);
        }

        [TestMethod]
        public void BallDoesNotLeaveBoard()
        {
            Ball ball = new Ball(1, new Vector(50, 50), new Vector(0, 0), 10, 1);

            double width = 100;
            double height = 100;
            double radius = 10;

            ball.Move(new Vector(-100, 0), width, height, radius);
            Assert.IsTrue(ball.X >= radius);

            ball.Move(new Vector(1000, 0), width, height, radius);
            Assert.IsTrue(ball.X <= width - radius);
        }
    }
}
