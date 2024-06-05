using System.Collections.Generic;
using System.Linq;
using Godot;

public class SpaceshipProperties
{
    public bool HasSideCannons { get; set; } = false;
    public float ShootCooldown { get; set; }
}

public partial class BuffSystem : Node2D
{
    [Export]
    public float BuffDuration { get; set; } = 30.0f;

    [Export]
    public float NormalShootCooldown { get; set; } = 0.25f;

    [Export]
    public float RapidFireCooldown { get; set; } = 0.05f;

    [Export]
    public float SlowMotionTimeScale { get; set; } = 0.5f;

    private Dictionary<BuffType, float> Buffs { get; } = new();

    public void Add(BuffType buff)
    {
        Buffs[buff] = BuffDuration;
    }

    private void DecreaseDuration(float delta)
    {
        foreach (var buff in Buffs.Keys)
            Buffs[buff] -= delta;

        foreach (var buff in Buffs.Keys.ToList())
        {
            if (Buffs[buff] <= 0)
                Buffs.Remove(buff);
        }
    }

    private void ApplyBuff(SpaceshipProperties properties, BuffType buff)
    {
        switch (buff)
        {
            case BuffType.BurstFire:
                properties.HasSideCannons = true;
                break;
            case BuffType.RapidFire:
                properties.ShootCooldown = RapidFireCooldown;
                break;
        }
    }

    private void AddSideCannons(Spaceship spaceship)
    {
        foreach (var cannon in spaceship.Cannons)
            cannon.Show();
    }

    private void RemoveSideCannons(Spaceship spaceship)
    {
        var middle = spaceship.Cannons.Length / 2;

        for (var index = 0; index < spaceship.Cannons.Length; index++)
        {
            var cannon = spaceship.Cannons[index];

            if (index != middle)
                cannon.Hide();
            else
                cannon.Show();
        }
    }

    private void ApplyProperties(SpaceshipProperties properties)
    {
        var spaceship = GetParent<Spaceship>();
        spaceship.ShootCooldown = properties.ShootCooldown;

        if (properties.HasSideCannons)
            AddSideCannons(spaceship);
        else
            RemoveSideCannons(spaceship);
    }

    private SpaceshipProperties GetDefaultProperties()
        => new() { ShootCooldown = NormalShootCooldown };

    public override void _Process(double delta)
    {
        DecreaseDuration((float)delta);

        var properties = GetDefaultProperties();

        foreach (var buff in Buffs.Keys)
            ApplyBuff(properties, buff);

        ApplyProperties(properties);
    }

    public void Reset()
    {
        Buffs.Clear();
        ApplyProperties(GetDefaultProperties());
    }
}