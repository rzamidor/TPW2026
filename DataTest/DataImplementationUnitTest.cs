//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class DataImplementationUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            using (DataImplementation newInstance = new DataImplementation())
            {
                IReadOnlyList<IBall> ballsList = newInstance.GetBalls();
                Assert.IsNotNull(ballsList);
                Assert.AreEqual(0, ballsList.Count);
            }
        }

        [TestMethod]
        public void DisposeTestMethod()
        {
            DataImplementation newInstance = new DataImplementation();

            Assert.IsNotNull(newInstance.GetBalls());

            newInstance.Dispose();

            Assert.AreEqual(0, newInstance.GetBalls().Count);
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Start(0, (position, ball) => { }));
        }

        [TestMethod]
        public async Task Start_CreatesBalls_And_MovesThemConcurrently()
        {
            using (DataImplementation newInstance = new DataImplementation())
            {
                int numberOfCallbackInvoked = 0;
                int numberOfBalls2Create = 5;

                newInstance.Start(
                    numberOfBalls2Create,
                    (startingPosition, ball) =>
                    {
                        numberOfCallbackInvoked++;
                        Assert.IsNotNull(ball);
                    });

                Assert.AreEqual(numberOfBalls2Create, numberOfCallbackInvoked);

                var balls = newInstance.GetBalls();
                Assert.AreEqual(5, balls.Count);

                double initialX = balls[0].X;
                double initialY = balls[0].Y;

                await Task.Delay(200);

                bool hasMoved = (initialX != balls[0].X) || (initialY != balls[0].Y);
                Assert.IsTrue(hasMoved, "Kule nie poruszają się w tle!");
            }
        }
    }
}