//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data
{
  internal class Ball : IBall
  {
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity)
    {
      Position = initialPosition;
      Velocity = initialVelocity;
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity { get; set; }

        #endregion IBall

        #region private

   public Vector Position { get; private set; }


        private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }

        internal void Move(IVector delta, double width, double height, double radius)
        {
            // aktualizacja pozycji na podstawie delta
            double newX = Position.x + delta.x;
            double newY = Position.y + delta.y;

            if (newX <= radius || newX >= width - radius)
                Velocity = new Vector(-Velocity.x, Velocity.y);

            if (newY <= radius || newY >= height - radius)
                Velocity = new Vector(Velocity.x, -Velocity.y);

            // ograniczenie pozycji do obszaru stołu
            newX = Math.Max(radius, Math.Min(width - radius, newX));
            newY = Math.Max(radius, Math.Min(height - radius, newY));

            Position = new Vector(newX, newY);

            RaiseNewPositionChangeNotification();
        }




        #endregion private
    }
}