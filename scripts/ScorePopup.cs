using Godot;

public partial class ScorePopup : Window
{
    [Export]
    public Label? HighscoreLabel { get; set; }

    [Export]
    public Label? Text { get; set; }

    [Export]
    public Button? Exit { get; set; }

    [Export]
    public Button? Restart { get; set; }

    private int Highscore { get; set; } = -1;

    public bool CheckHighscore(int score)
    {
        if (score > Highscore)
        {
            Highscore = score;
            return true;
        }

        return false;
    }

    public void Initialize(int score)
    {
        if (HighscoreLabel != null)
            HighscoreLabel.Visible = CheckHighscore(score);

        if (Text != null)
            Text.Text = $"Score: {score}\nHighscore: {Highscore}";
    }

    private void OnRestart()
    {
        // Destroy all meteorites
        foreach (var meteorite in GetTree().GetNodesInGroup(Meteorite.Group))
            meteorite.QueueFree();

        // Reset the player to the center
        var spaceship = GetTree().Root.GetNode<Spaceship>("Root/Spaceship");
        spaceship.Reset();

        // Remove slow down
        Engine.TimeScale = 1.0f;

        // Restart the score counter
        var counter = GetTree().Root.GetNode<PointCounter>("Root/Gui/Points");
        counter.Points = 0;
        counter.Enabled = true;

        // Hide the popup
        Hide();
    }

    public override void _Ready()
    {
        if (Exit != null)
            Exit.Pressed += () => GetTree().Quit();

        if (Restart != null)
            Restart.Pressed += OnRestart;
    }
}