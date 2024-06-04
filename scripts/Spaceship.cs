using System.IO;
using System;
using Godot;

public partial class Spaceship : RigidBody2D
{
    [Export]
    public Node2D? Root { get; set; }

    [Export]
    public float RotationSpeed { get; set; } = (float)Math.PI / 2.0f;

    [Export]
    public float RecoilForce { get; set; }

    [Export]
    public float MaxSpeed { get; set; }

    [Export]
    public float ShakeStrength { get; set; } = 32.0f;

    [Export]
    public float ShakeFade { get; set; } = 5.0f;

    [Export]
    public float ShootCooldown { get; set; }

    [Export]
    public PackedScene? Projectile { get; set; }

    [Export]
    public Node2D? ProjectileSpawnpoint { get; set; }

    [Export]
    public float ProjectileSpeed { get; set; }

    [Export]
    public PackedScene? Explosion { get; set; }

    private float ShootCooldownRemaining { get; set; }
    private float CurrentShakeStrength { get; set; }

    public void Shake(float factor = 1.0f)
    {
        CurrentShakeStrength = ShakeStrength * factor;
    }

    private void ShowScoreDialog()
    {
        // Fetch the current score and stop the counter
        var counter = GetTree().Root.GetNode<PointCounter>("Root/Gui/Points");
        counter.Enabled = false;

        // Show the score dialog
        var popup = GetTree().Root.GetNode<ScorePopup>("Root/Gui/Score");
        popup.Initialize((int)counter.Points);
        popup.Show();
    }

    private void OnHit(Node body)
    {
        if (body.HasMeta("type") && body.GetMeta("type").AsString() == "meteorite")
        {
            // Explode the ship
            if (Explosion?.Instantiate() is not CpuParticles2D explosion)
                throw new InvalidDataException("Failed to spawn an explosion");

            explosion.GlobalPosition = GlobalPosition;
            explosion.Emitting = true;
            GetTree().Root.AddChild(explosion);

            // Disable and hide the player
            SetProcess(false);
            CollisionLayer = 0;
            Visible = false;

            // Make a big blast shake
            Shake(10.0f);

            explosion.Finished += () =>
            {
                // Slow down the world, because the player has just lost
                Engine.TimeScale = 0.075;
                ShowScoreDialog();
            };
        }
    }

    private void Shoot()
    {
        if (Projectile?.Instantiate() is not RigidBody2D projectile)
            throw new InvalidDataException("Expected projectile to be a rigid body");

        // Make the projectile go towards the direction we're facing
        var direction = new Vector2(MathF.Cos(Rotation), MathF.Sin(Rotation));

        projectile.GlobalPosition = ProjectileSpawnpoint?.GlobalPosition ?? Vector2.Zero;
        projectile.LinearVelocity = direction * ProjectileSpeed;
        projectile.Rotation = Rotation;
        Root?.AddChild(projectile);

        // Start shaking the camera
        Shake();

        // Apply opposite recoil force to ourselves
        LinearVelocity += -direction * RecoilForce;
        LinearVelocity = LinearVelocity.Normalized() * MathF.Min(LinearVelocity.Length(), MaxSpeed);
    }

    public void Reset()
    {
        GlobalPosition = Vector2.Zero;
        LinearVelocity = Utils.RandomDirection() * RecoilForce;

        // Enable all the necessary components
        SetProcess(true);
        CollisionLayer = 1;
        Visible = true;
    }

    public override void _Ready()
    {
        Reset();
        BodyEntered += OnHit;
    }

    public override void _Process(double delta)
    {
        if (CurrentShakeStrength > 0)
        {
            CurrentShakeStrength = Mathf.Lerp(CurrentShakeStrength, 0, ShakeFade * (float)delta);
            GetTree().Root.GetNode<Camera2D>("Root/Camera").Offset = Utils.RandomDirection() * CurrentShakeStrength;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        AngularVelocity = RotationSpeed;

        // The player can steer the spaceship using the main cannon's recoil velocity
        ShootCooldownRemaining -= (float)delta;

        if (ShootCooldownRemaining <= 0.0f && Input.IsActionPressed("Shoot"))
        {
            ShootCooldownRemaining = ShootCooldown;
            GD.Print("Shooting");
            Shoot();
        }
    }
}