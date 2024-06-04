using Godot;

public partial class Meteorite : RigidBody2D
{
    public const float Lifetime = 60.0f;

    private Vector2 Velocity { get; set; }
    private float RotationSpeed { get; set; }

    private float LifetimeRemaining { get; set; } = Lifetime;

    public void Initialize(Vector2 position, Vector2 velocity, float rotation_speed)
    {
        GlobalPosition = position;
        Velocity = velocity;
        RotationSpeed = rotation_speed;
    }

    public override void _Process(double delta)
    {
        LifetimeRemaining -= (float)delta;

        if (LifetimeRemaining <= 0.0f)
            QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        LinearVelocity = Velocity;
        AngularVelocity = RotationSpeed;
    }
}