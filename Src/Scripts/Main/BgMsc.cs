using Godot;
using System;

public partial class BgMsc : AudioStreamPlayer
{
	[Export] public AudioStream[] MusicTracks = new AudioStream[7];
	private Random _random = new Random();
	private int _previousTrackIndex = -1;

	public override void _Ready()
	{
		if (MusicTracks.Length > 0)
		{
			PlayRandomTrack();
		}
		
		Finished += OnTrackFinished;
	}

	private void PlayRandomTrack()
	{
		if (MusicTracks.Length == 0) return;
		
		int newTrackIndex;
		
		do
		{
			newTrackIndex = _random.Next(MusicTracks.Length);
		} 
		while (MusicTracks.Length > 1 && newTrackIndex == _previousTrackIndex);
		
		_previousTrackIndex = newTrackIndex;
		
		if (MusicTracks[newTrackIndex] != null)
		{
			Stream = MusicTracks[newTrackIndex];
			Play();
		}
	}

	private void OnTrackFinished()
	{
		PlayRandomTrack();
	}

	public override void _ExitTree()
	{
		Finished -= OnTrackFinished;
	}
}
