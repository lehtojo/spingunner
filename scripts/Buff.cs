using Godot;

public enum BuffType
{
    BurstFire,
    RapidFire
}

public partial class Buff : RigidBody2D
{
    public const string Group = "buffs";

    [Export]
    public BuffType Type { get; set; }

    [Export]
    public float Lifetime { get; set; }

    private Vector2 Velocity { get; set; }
    private float RotationSpeed { get; set; }

    private float LifetimeRemaining { get; set; }

    private void OnHit(Node body)
    {
        if (body is not Spaceship spaceship)
            return;

        spaceship.Buffs?.Add(Type);

        QueueFree();
    }

    public void Initialize(Vector2 position, Vector2 velocity, float rotation_speed)
    {
        GlobalPosition = position;
        Velocity = velocity;
        RotationSpeed = rotation_speed;
        AddToGroup(Group);
    }

    public override void _Ready()
    {
        LifetimeRemaining = Lifetime;
        BodyEntered += OnHit;
    }

    public override void _Process(double delta)
    {
        LifetimeRemaining -= (float)delta;

        if (LifetimeRemaining <= 0)
            QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        LinearVelocity = Velocity;
        AngularVelocity = RotationSpeed;
    }
}