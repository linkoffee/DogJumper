using Godot;

public partial class BonusObj : Area2D
{
	[Export] public int ScoreValue = 2500;
	[Export] public AudioStream SoundEffect;
	
	private AudioStreamPlayer2D audioPlayer;
	private YouScore scoreLabel;

	public override void _Ready()
	{
		audioPlayer = new AudioStreamPlayer2D();
		AddChild(audioPlayer);

		if (SoundEffect != null)
		{
			audioPlayer.Stream = SoundEffect;
		}

		scoreLabel = GetTree().GetFirstNodeInGroup("score") as YouScore;

		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
	}

	private void OnBodyEntered(Node body)
	{
		if (body.IsInGroup("player"))
		{
			if (SoundEffect != null)
			{
				audioPlayer.Play();
			}

			if (scoreLabel != null)
			{
				scoreLabel.AddPoints(ScoreValue);
			}

			Hide();
		}
	}
}
