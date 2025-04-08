using Godot;

public partial class RestartGameBtn : TextureButton
{
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		Pressed += OnButtonPressed;
	}

	private void OnButtonPressed()
	{
		GameManager.Instance.ResetGame();
		GetTree().ReloadCurrentScene();
	}
}
