using Microsoft.VisualStudio.TestTools.UnitTesting;
using TP.ConcurrentProgramming.BusinessLogic;
using TP.ConcurrentProgramming.Data;
using System.Collections.Generic;

namespace LogicTests
{
    internal class FakeBall : IBall
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Vx { get; set; }
        public double Vy { get; set; }
        public double Radius { get; set; }
        public double Mass { get; set; }
        public int Id { get; set; }
    }

    internal class FakeDataLayer : DataAbstractAPI
    {
        public List<IBall> Balls = new();

        public override List<IBall> GetBalls() => Balls;

        public override void UpdateBallVelocity(int id, double vx, double vy)
        {
            var b = (FakeBall)Balls.Find(b => b.Id == id);
            b.Vx = vx;
            b.Vy = vy;
        }

        public override void Start(int count, System.Action<IVector, IBall> callback) { }
        public override void Stop() { }
    }

    [TestClass]
    public class CollisionTests
    {
        [TestMethod]
        public void Balls_Collide_And_Exchange_Velocities()
        {
            var b1 = new FakeBall { X = 0, Y = 0, Vx = 1, Vy = 0, Radius = 5, Mass = 1, Id = 1 };
            var b2 = new FakeBall { X = 9, Y = 0, Vx = -1, Vy = 0, Radius = 5, Mass = 1, Id = 2 };

            var data = new FakeDataLayer();
            data.Balls.Add(b1);
            data.Balls.Add(b2);

            var logic = BusinessLogicAbstractAPI.CreateAPI(data);

            logic.Update();

            Assert.AreEqual(-1, b1.Vx);
            Assert.AreEqual(1, b2.Vx);
        }

        [TestMethod]
        public void Ball_Bounces_Off_Right_Wall()
        {
            var b = new FakeBall { X = 795, Y = 200, Vx = 5, Vy = 0, Radius = 5, Mass = 1, Id = 1 };

            var data = new FakeDataLayer();
            data.Balls.Add(b);

            var logic = BusinessLogicAbstractAPI.CreateAPI(data);

            logic.Update();

            Assert.AreEqual(-5, b.Vx);
        }
    }
}
