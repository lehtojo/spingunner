using System.IO;
using System;
using Godot;

public partial class Cannon : Node2D
{
    [Export]
    public float ShootCooldown { get; set; } = 0.25f;

    [Export]
    public PackedScene? Projectile { get; set; }

    [Export]
    public Node2D? ProjectileSpawnpoint { get; set; }

    [Export]
    public float ProjectileSpeed { get; set; } = 1024;

    [Export]
    public float RecoilForce { get; set; } = 200;

    [Export]
    public float MaxBodySpeed { get; set; } = 400;

    private void LaunchProjectile(Vector2 position, Vector2 direction)
    {
        if (Projectile?.Instantiate() is not RigidBody2D projectile)
            throw new InvalidDataException("Expected projectile to be a rigid body");

        projectile.GlobalPosition = position;
        projectile.LinearVelocity = direction * ProjectileSpeed;
        projectile.Rotation = GlobalRotation;

        GetTree().Root.AddChild(projectile);
    }

    private void ApplyRecoil(Vector2 direction)
    {
        var body = GetParent<RigidBody2D>();

        body.LinearVelocity += -direction * RecoilForce;
        body.LinearVelocity = body.LinearVelocity.Normalized() * MathF.Min(body.LinearVelocity.Length(), MaxBodySpeed);
    }

    public void Shoot()
    {
        if (ProjectileSpawnpoint == null)
            return;

        // Make the projectile go towards the direction we're facing
        var direction = new Vector2(MathF.Cos(GlobalRotation), MathF.Sin(GlobalRotation));

        LaunchProjectile(ProjectileSpawnpoint.GlobalPosition, direction);
        ApplyRecoil(direction);
    }
}