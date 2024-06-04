using Godot;

public partial class PointCounter : Label
{
    public const int MeteoriteHitPoints = 10000;

    [Export]
    public int PointsPerSecond { get; set; }

    public float Points { get; set; }
    public bool Enabled { get; set; } = true;

    public override void _Ready()
    {
        Points = 0.0f;
    }

    public override void _Process(double delta)
    {
        if (Enabled)
            Points += PointsPerSecond * (float)delta;

        Text = ((int)Points).ToString();
    }

    public void OnHitMeteorite()
    {
        Points += MeteoriteHitPoints;
    }
}