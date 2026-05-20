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
        public void StartTestMethod()
        {
            using (DataImplementation newInstance = new DataImplementation())
            {
                int numberOfCallbackInvoked = 0;
                int numberOfBalls2Create = 10;

                newInstance.Start(
                    numberOfBalls2Create,
                    (startingPosition, ball) =>
                    {
                        numberOfCallbackInvoked++;
                        Assert.IsTrue(startingPosition.x >= 0);
                        Assert.IsTrue(startingPosition.y >= 0);
                        Assert.IsNotNull(ball);
                    });

                Assert.AreEqual(numberOfBalls2Create, numberOfCallbackInvoked);
                Assert.AreEqual(10, newInstance.GetBalls().Count);
            }
        }
    }
}
