using Godot;

public partial class Player : CharacterBody2D
{
	[Export] public AudioStreamPlayer2D TimedSound;
	private Timer soundTimer;
	private AnimatedSprite2D playerAnimation;

	[Export] public float JumpPower = 1000f;
	[Export] public float JumpDeceleration = 3f;
	[Export] public float FallSpeed = 450f;
	private bool isJumping = false;
	private Vector2 startPosition;
	private float jumpProgress = 0f;

	public override void _Ready()
	{
		playerAnimation = GetNode<AnimatedSprite2D>("PlayerAnimation");
		startPosition = Position;

		if (TimedSound != null)
			TimedSound.Play();

		soundTimer = new Timer();
		AddChild(soundTimer);
		soundTimer.WaitTime = 45f;
		soundTimer.OneShot = false;
		soundTimer.Timeout += () => { if (TimedSound != null) TimedSound.Play(); };
		soundTimer.Start();
	}

	public override void _PhysicsProcess(double delta)
	{
		if ((Input.IsActionJustPressed("ui_accept") || Input.IsActionJustPressed("touch")) && !isJumping)
		{
			TryJump();
		}

		if (isJumping)
		{
			jumpProgress += (float)delta * JumpDeceleration;

			if (jumpProgress < 1f)
			{
				Position += new Vector2(0, -JumpPower * (1f - jumpProgress) * (float)delta);
			}
			else
			{
				Position += new Vector2(0, FallSpeed * (jumpProgress - 1f) * (float)delta);
			}

			if (Position.Y >= startPosition.Y)
			{
				Position = startPosition;
				isJumping = false;
				jumpProgress = 0f;
				playerAnimation.Play("Run");
			}
		}
	}

	private void TryJump()
	{
		isJumping = true;
		jumpProgress = 0f;
		playerAnimation.Play("Jump");
	}
}
