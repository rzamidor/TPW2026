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
            Vector testVector = new Vector(0.0, 0.0);
            Ball newInstance = new Ball(1, testVector, testVector, 10.0, 1.0);

            Assert.AreEqual(1, newInstance.Id);
            Assert.AreEqual(10.0, newInstance.Radius);
            Assert.AreEqual(1.0, newInstance.Mass);
            Assert.AreEqual(0.0, newInstance.X);
            Assert.AreEqual(0.0, newInstance.Y);
        }

        [TestMethod]
        public void MoveTestMethod()
        {
            Vector initialPosition = new Vector(10.0, 10.0);
            Ball newInstance = new Ball(1, initialPosition, new Vector(0.0, 0.0), 10.0, 1.0);
            IVector currentPosition = new Vector(0.0, 0.0);
            int numberOfCallBackCalled = 0;

            newInstance.NewPositionNotification += (sender, position) => {
                Assert.IsNotNull(sender);
                currentPosition = position;
                numberOfCallBackCalled++;
            };

            newInstance.Move(new Vector(0.0, 0.0), 800, 400, 10.0);

            Assert.AreEqual(1, numberOfCallBackCalled);
            Assert.AreEqual(initialPosition.x, currentPosition.x);
            Assert.AreEqual(initialPosition.y, currentPosition.y);
        }

        [TestMethod]
        public void BallDoesNotLeaveBoard()
        {
            double width = 100;
            double height = 100;
            double radius = 10;

            Ball ball = new Ball(1, new Vector(50, 50), new Vector(0, 0), radius, 1.0);

            // X tests
            ball.Move(new Vector(-100, 0), width, height, radius);
            Assert.IsTrue(ball.X >= radius);

            ball.Move(new Vector(1000, 0), width, height, radius);
            Assert.IsTrue(ball.X <= width - radius);

            // Y tests
            ball.Move(new Vector(0, -100), width, height, radius);
            Assert.IsTrue(ball.Y >= radius);

            ball.Move(new Vector(0, 1000), width, height, radius);
            Assert.IsTrue(ball.Y <= height - radius);
        }
    }
}
