using Godot;

public partial class Projectile : RigidBody2D
{
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
            QueueFree();
        }
    }

    public override void _Ready()
    {
        LifetimeRemaining = Lifetime;
        BodyEntered += OnHit;
    }

    public override void _Process(double delta)
    {
        LifetimeRemaining -= (float)delta;

        if (LifetimeRemaining <= 0.0f)
            QueueFree();
    }
}