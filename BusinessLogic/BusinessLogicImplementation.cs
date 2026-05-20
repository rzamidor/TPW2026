using System;
using System.Collections.Generic;
using TP.ConcurrentProgramming.Data;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BusinessLogicTest")]

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        private readonly DataAbstractAPI _dataLayer;

        public BusinessLogicImplementation(DataAbstractAPI? dataLayer = null)
        {
            _dataLayer = dataLayer ?? DataAbstractAPI.GetDataLayer();
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            _dataLayer.Start(numberOfBalls, (vector, dataBall) =>
            {
                Ball businessBall = new Ball(dataBall);

                Position pos = new Position(vector.x, vector.y);

                upperLayerHandler(pos, businessBall);
            });
        }

        public override void Dispose()
        {
            _dataLayer.Dispose();
        }
    }
}