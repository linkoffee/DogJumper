using Godot;

public partial class ExitFromGameBtn : TextureButton
{
	public override void _Ready()
	{
		Pressed += OnButtonPressed;
	}

	private void OnButtonPressed()
	{
		GetTree().Quit();
	}
}
