using System.Linq;
using Godot;

public partial class Meteorite : RigidBody2D
{
    public const string Group = "meteorites";
    public const float Lifetime = 60.0f;

    private Vector2 Velocity { get; set; }
    private float RotationSpeed { get; set; }

    private float LifetimeRemaining { get; set; } = Lifetime;

    public void Initialize(Vector2 position, Vector2 velocity, float rotation_speed, float scale)
    {
        GlobalPosition = position;
        Velocity = velocity;
        RotationSpeed = rotation_speed;
        AddToGroup(Group);

        foreach (var child in GetChildren().Cast<Node2D>())
            child.GlobalScale = new Vector2(scale, scale);
    }

    // public override void _Draw()
    // {
    //     const float Epsilon = 0.001f;

    //     if (MathF.Abs(Velocity.X) < Epsilon)
    //         return;

    //     var spaceship = GetTree().Root.GetNode<Spaceship>("Root/Spaceship");
    //     var k1 = MathF.Tan(spaceship.GlobalRotation);
    //     var k2 = Velocity.Y / Velocity.X;

    //     if (MathF.Abs(k1 - k2) < Epsilon)
    //         return;

    //     // Based: y - y0 = k(x - x0) => y = k(x - x0) + y0
    //     // y = k1(x - x0) + y0
    //     // y' = k2(x - x1) + y1
    //     // y = y'
    //     // k1(x - x0) + y0 = k2(x - x1) + y1
    //     // k1x - k1x0 + y0 = k2x - k2x1 + y1
    //     // k1x - k2x = k1x0 - k2x1 - y0 + y1
    //     // x(k1 - k2) = k1x0 - k2x1 - y0 + y1
    //     // x = (k1x0 - k2x1 - y0 + y1) / (k1 - k2)
    //     var x0 = GlobalPosition.X;
    //     var y0 = GlobalPosition.Y;
    //     var x1 = spaceship.GlobalPosition.X;
    //     var y1 = spaceship.GlobalPosition.Y;

    //     var x = (k1 * x0 - k2 * x1 - y0 + y1) / (k1 - k2);
    //     var y = k1 * (x - x0) + y0;

    //     // Compute how many seconds it takes for the projectile to reach the point from the ship based on distance
    //     var distance = spaceship.ProjectileSpawnpoint!.GlobalPosition.DistanceTo(new Vector2(x, y));
    //     var seconds = distance / spaceship.ProjectileSpeed;

    //     // Compute where the meteorite is going to be in that many seconds
    //     var future_position = GlobalPosition + Velocity * seconds;

    //     DrawSetTransformMatrix(Transform.AffineInverse());
    //     DrawCircle(future_position, 4.0f, Colors.Red);
    // }

    public override void _Process(double delta)
    {
        LifetimeRemaining -= (float)delta;

        if (LifetimeRemaining <= 0.0f)
            QueueFree();

        // QueueRedraw();
    }

    public override void _PhysicsProcess(double delta)
    {
        LinearVelocity = Velocity;
        AngularVelocity = RotationSpeed;
    }
}