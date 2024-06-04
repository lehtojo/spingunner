using Godot;

public partial class ScorePopup : Popup
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

    }

    public override void _Ready()
    {
        if (Exit != null)
            Exit.Pressed += () => GetTree().Quit();

        if (Restart != null)
            Restart.Pressed += OnRestart;
    }
}