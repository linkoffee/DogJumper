using Godot;

public partial class BgMsc : AudioStreamPlayer
{
	[Export] public AudioStream[] MusicTracks = new AudioStream[7];
	private int currentTrackIndex = 0;

	public override void _Ready()
	{
		if (MusicTracks.Length > 0 && MusicTracks[0] != null)
		{
			PlayTrack(currentTrackIndex);
		}
	}

	private void PlayTrack(int index)
	{
		if (index >= 0 && index < MusicTracks.Length && MusicTracks[index] != null)
		{
			Stream = MusicTracks[index];
			Play();
		}
	}

	public override void _Process(double delta)
	{
		if (!Playing)
		{
			currentTrackIndex = (currentTrackIndex + 1) % MusicTracks.Length;
			PlayTrack(currentTrackIndex);
		}
	}
}
