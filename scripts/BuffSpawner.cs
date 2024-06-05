using System;
using Godot;

public partial class BuffSpawner : Node2D
{
    [Export]
    public PackedScene[] Buffs { get; set; } = Array.Empty<PackedScene>();

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
        // Choose a random point within the viewport
        var rect = GetViewportRect();
        var left = -rect.Size.X / 2;
        var right = rect.Size.X / 2;
        var top = -rect.Size.Y / 2;
        var bottom = rect.Size.Y / 2;
        var target = new Vector2(Utils.Random(left, right), Utils.Random(top, bottom));
        var distance = MathF.Max(GetViewportRect().Size.X, GetViewportRect().Size.Y);
        var spawnpoint = target + Utils.RandomDirection() * distance;
        var direction = (target - spawnpoint).Normalized();
        var speed = Utils.Random(VelocityRange);
        var velocity = direction * speed;
        var rotation_speed = Utils.Random(RotationSpeedRange);

        var buff = Utils.Random(Buffs).Instantiate<Buff>();

        if (buff == null)
            return;

        buff.Initialize(spawnpoint, velocity, rotation_speed);

        GetTree().Root.AddChild(buff);
    }

    public override void _Process(double delta)
    {
        SpawnIntervalRemaining -= (float)delta;

        if (SpawnIntervalRemaining <= 0)
        {
            SpawnIntervalRemaining = Utils.Random(SpawnIntervalRange);
            GD.Print("Spawning buff");
            Spawn();
        }
    }
}