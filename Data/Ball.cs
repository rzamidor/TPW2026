namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        private Vector position;
        private Vector velocity;

        public event EventHandler<IVector>? NewPositionNotification;

        public int Id { get; }
        public double Radius { get; }
        public double Mass { get; }

        public double X => position.x;
        public double Y => position.y;
        public double Vx => velocity.x;
        public double Vy => velocity.y;

        private readonly object _ballLock = new();

        public IVector Velocity
        {
            get
            {
                lock (_ballLock) return velocity;
            }
            set
            {
                lock (_ballLock) velocity = new Vector(value.x, value.y);
            }
        }

        public Ball(int id, Vector position, Vector velocity, double radius, double mass)
        {
            Id = id;
            this.position = position;
            this.velocity = velocity;
            Radius = radius;
            Mass = mass;
        }

        public void Move(IVector delta)
        {
            Vector currentPosition;
            lock (_ballLock)
            {
                position = new Vector(position.x + delta.x, position.y + delta.y);
                currentPosition = position;
            }
            NewPositionNotification?.Invoke(this, currentPosition);
        }
    }
}
