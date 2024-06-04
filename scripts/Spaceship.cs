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

    private float ShootCooldownRemaining { get; set; }
    private float CurrentShakeStrength { get; set; }

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
        CurrentShakeStrength = ShakeStrength;

        // Apply opposite recoil force to ourselves
        LinearVelocity += -direction * RecoilForce;
        LinearVelocity = LinearVelocity.Normalized() * MathF.Min(LinearVelocity.Length(), MaxSpeed);
    }

    public override void _Ready()
    {
        LinearVelocity = Utils.RandomDirection() * RecoilForce;
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