using Godot;

public partial class ParallaxBg : ParallaxBackground
{
	[Export] public float BaseSpeed = 150f;
	private Timer speedTimer;

	public override void _Ready()
	{
		speedTimer = new Timer();
		AddChild(speedTimer);
		speedTimer.WaitTime = 1f;
		speedTimer.OneShot = false;
		speedTimer.Timeout += () => BaseSpeed += 3f;
		speedTimer.Start();
	}

	public override void _Process(double delta)
	{
		foreach (var node in GetChildren())
		{
			if (node is ParallaxLayer layer)
			{
				float speedMultiplier = layer.GetMeta("scrollSpeed", 1.0f).AsSingle();
				layer.MotionOffset -= new Vector2((float)(BaseSpeed * speedMultiplier * delta), 0);
			}
		}
	}
}
