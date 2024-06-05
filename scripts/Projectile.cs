using Godot;

public partial class Projectile : RigidBody2D
{
    public const string Group = "projectiles";

    [Export]
    public float Lifetime { get; set; }

    [Export]
    public PackedScene? Explosion { get; set; }

    private float LifetimeRemaining { get; set; }

    private void OnHit(Node body)
    {
        if (body.HasMeta("type") && body.GetMeta("type").AsString() == "meteorite")
        {
            if (Explosion?.Instantiate() is CpuParticles2D explosion)
            {
                explosion.Emitting = true;
                explosion.GlobalPosition = GlobalPosition;
                GetTree().Root.AddChild(explosion);
            }

            body.QueueFree();
            // QueueFree(); // We don't despawn the projectile as we want piercing

            GetTree().Root.GetNode<PointCounter>("Root/Gui/Points").OnHitMeteorite();
            GetTree().Root.GetNode<Spaceship>("Root/Spaceship").Shake(4.0f);
        }
    }

    public override void _Ready()
    {
        LifetimeRemaining = Lifetime;
        BodyEntered += OnHit;
        AddToGroup(Group);
    }

    public override void _Process(double delta)
    {
        LifetimeRemaining -= (float)delta;

        if (LifetimeRemaining <= 0.0f)
            QueueFree();
    }
}