using Godot;

public partial class Enemy : CharacterBody2D
{
	[Export] public int Damage = 1;
	[Export] public AudioStreamPlayer2D HitSound;
	[Export] public Area2D DetectionArea;
	
	public override void _Ready()
	{
		if (HitSound == null)
		{
			HitSound = GetNodeOrNull<AudioStreamPlayer2D>("HitSound");
			GD.Print("Hit sound not assigned!");
		}
		
		if (DetectionArea == null)
		{
			DetectionArea = GetNodeOrNull<Area2D>("Area2D");
		}
		
		if (DetectionArea != null)
		{
			DetectionArea.BodyEntered += OnBodyEntered;
		}
		else
		{
			GD.PrintErr("DetectionArea not found!");
		}
	}
	
	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			DealDamage();
			PlayHitSound();
		}
	}
	
	private void DealDamage()
	{
		GameManager.Instance.LoseLife();
		PlayHitSound();
	}
	
	private void PlayHitSound()
	{
		if (HitSound != null)
		{
			HitSound.Play();
		}
		else
		{
			GD.Print("Hit sound not assigned!");
		}
	}
}
