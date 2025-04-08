using Godot;

public partial class YouScore : Label
{
	public int Score { get; private set; } = 0;

	private float _timeAccumulator = 0f;
	private float _speedMultiplier = 0.1f;
	private float _speedIncreaseInterval = 1f;
	private float _speedIncreaseAmount = 0.01f;
	private float _timeSinceLastSpeedIncrease = 0f;

	public override void _Process(double delta)
	{
		if (GameManager.Instance == null || GameManager.Instance.IsGameOver) return;
		
		float deltaTime = (float)delta;
		_timeAccumulator += deltaTime;

		while (_timeAccumulator >= 0.001f / _speedMultiplier)
		{
			Score += 1;
			GameManager.Instance.CurrentScore = Score;
			Text = $"{Score}";
			_timeAccumulator -= 0.001f / _speedMultiplier;
		}
	}

	public void AddPoints(int points)
	{
		Score += points;
		GameManager.Instance.CurrentScore = Score;
		Text = $"{Score}";
	}

	public void ResetScore()
	{
		Score = 0;
		Text = "0";
		_timeAccumulator = 0f;
		_speedMultiplier = 0.1f;
	}
}
