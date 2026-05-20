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
using System;
using TP.ConcurrentProgramming.BusinessLogic;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BusinessBallUnitTest
    {
        private class StubVector : TP.ConcurrentProgramming.Data.IVector
        {
            public double x { get; init; }
            public double y { get; init; }
        }

        private class StubDataBall : TP.ConcurrentProgramming.Data.IBall
        {
            public event EventHandler<TP.ConcurrentProgramming.Data.IVector>? NewPositionNotification;

            public TP.ConcurrentProgramming.Data.IVector Velocity { get; set; } = new StubVector();
            public double X => 0;
            public double Y => 0;
            public double Vx => 0;
            public double Vy => 0;
            public double Radius => 10;
            public double Mass => 1;
            public int Id => 1;

            public void SimulateMove(double x, double y)
            {
                NewPositionNotification?.Invoke(this, new StubVector { x = x, y = y });
            }
        }

        [TestMethod]
        public void Ball_EventMapping_TestMethod()
        {
            StubDataBall stubDataBall = new StubDataBall();

            Ball businessBall = new Ball(stubDataBall);

            int eventTriggerCount = 0;
            IPosition? receivedPosition = null;

            businessBall.NewPositionNotification += (sender, position) =>
            {
                eventTriggerCount++;
                receivedPosition = position;
            };

            double testX = 15.5;
            double testY = 42.1;
            stubDataBall.SimulateMove(testX, testY);

            Assert.AreEqual(1, eventTriggerCount);
            Assert.IsNotNull(receivedPosition);
            Assert.AreEqual(testX, receivedPosition.x);
            Assert.AreEqual(testY, receivedPosition.y);
        }
    }
}