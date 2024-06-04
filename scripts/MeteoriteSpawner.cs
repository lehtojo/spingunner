using System;
using System.Security.Principal;
using Godot;

public partial class MeteoriteSpawner : Node2D
{
    [Export]
    public PackedScene? Meteorite { get; set; }

    [Export]
    public Node2D? Root { get; set; }

    [Export]
    public Vector2 SpawnIntervalRange { get; set; }

    [Export]
    public Vector2 VelocityRange { get; set; }

    [Export]
    public Vector2 RotationSpeedRange { get; set; }

    private float SpawnIntervalRemaining { get; set; }

    public void Spawn()
    {
        // Configure the meteorite "parameters"
        var target = Vector2.Zero;
        var distance = MathF.Max(GetViewportRect().Size.X, GetViewportRect().Size.Y);
        var spawnpoint = target + Utils.RandomDirection() * distance;
        var direction = (target - spawnpoint).Normalized();
        var speed = Utils.Random(VelocityRange);
        var velocity = direction * speed;
        var rotation_speed = Utils.Random(RotationSpeedRange);

        var meteorite = Meteorite?.Instantiate<Meteorite>();

        if (meteorite == null)
            return;

        meteorite.Initialize(spawnpoint, velocity, rotation_speed);

        Root?.AddChild(meteorite);
    }

    public override void _Process(double delta)
    {
        SpawnIntervalRemaining -= (float)delta;

        if (SpawnIntervalRemaining <= 0)
        {
            SpawnIntervalRemaining = Utils.Random(SpawnIntervalRange);
            GD.Print("Spawning meteorite");
            Spawn();
        }
    }
}