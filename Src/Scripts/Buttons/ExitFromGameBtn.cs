using Godot;

public partial class ExitFromGameBtn : TextureButton
{
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		Pressed += OnButtonPressed;
	}

	private void OnButtonPressed()
	{
		GetTree().Quit();
	}
}
