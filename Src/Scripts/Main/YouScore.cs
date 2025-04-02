using Godot;

public partial class YouScore : Label
{
	private int score = 0;
	private float timeAccumulator = 0f;
	private float speedMultiplier = 0.1f;
	private float speedIncreaseInterval = 1f;
	private float speedIncreaseAmount = 0.01f;
	private float timeSinceLastSpeedIncrease = 0f;

	public override void _Process(double delta)
	{
		float deltaTime = (float)delta;
		timeAccumulator += deltaTime;
		timeSinceLastSpeedIncrease += deltaTime;

		if (timeSinceLastSpeedIncrease >= speedIncreaseInterval)
		{
			speedMultiplier += speedIncreaseAmount;
			timeSinceLastSpeedIncrease = 0f;
		}

		while (timeAccumulator >= 0.001f / speedMultiplier)
		{
			score += 1;
			Text = $"{score}";
			timeAccumulator -= 0.001f / speedMultiplier;
		}
	}

	public void AddPoints(int points)
	{
		score += points;
		Text = $"{score}";
	}
}
