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
    public float ShootCooldown { get; set; }

    [Export]
    public PackedScene? Projectile { get; set; }

    [Export]
    public Node2D? ProjectileSpawnpoint { get; set; }

    [Export]
    public float ProjectileSpeed { get; set; }

    private float ShootCooldownRemaining { get; set; }

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

        // Apply opposite recoil force to ourselves
        LinearVelocity += -direction * RecoilForce;
    }

    public override void _PhysicsProcess(double delta)
    {
        AngularVelocity = RotationSpeed;

        // The player can steer the spaceship using the main cannon's recoil velocity
        ShootCooldownRemaining -= (float)delta;

        if (ShootCooldownRemaining <= 0.0f && Input.IsActionJustPressed("Shoot"))
        {
            ShootCooldownRemaining = ShootCooldown;
            GD.Print("Shooting");
            Shoot();
        }
    }
}