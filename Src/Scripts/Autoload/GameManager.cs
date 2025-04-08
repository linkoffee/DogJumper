using Godot;
using System.Security.Cryptography;
using System.Text;
using System;

public partial class GameManager : Node
{
	public bool IsGameOver => _gameOver;
	public static GameManager Instance;
	
	public int CurrentLives { get; private set; } = 3;
	public int CurrentScore { get; set; }
	public int BestScore { get; private set; }
	
	private const string SAVE_PATH = "user://game_data.dat";
	private bool _gameOver = false;

	public override void _Ready()
	{
		Instance = this;
		LoadGameData();
	}

	private string GetSecretSalt()
	{
		return $"{OS.GetName()}{OS.GetUniqueId()}{DisplayServer.WindowGetSize().X}";
	}

	public void LoseLife()
	{
		if (_gameOver) return;
		
		CurrentLives--;
		UpdateHeartsUI();
		
		if (CurrentLives <= 0)
		{
			GameOver();
		}
	}

	private void UpdateHeartsUI()
	{
		var heartPanel = GetTree().GetFirstNodeInGroup("heart_panel");
		if (heartPanel == null) return;
		
		for (int i = 1; i <= 3; i++)
		{
			var heart = heartPanel.GetNodeOrNull<Sprite2D>($"Heart{i}");
			if (heart != null)
			{
				heart.Visible = (i <= CurrentLives);
			}
		}
	}

	private void GameOver()
	{
		if (_gameOver) return;
		_gameOver = true;
		
		if (CurrentScore > BestScore)
		{
			BestScore = CurrentScore;
			SaveGameData();
		}
		
		ShowRestartPanel();
		GetTree().Paused = true;
	}

	private void ShowRestartPanel()
	{
		var restartPanel = GetTree().CurrentScene.GetNodeOrNull<Sprite2D>("RestartPannel") ?? GetTree().Root.GetNodeOrNull<Sprite2D>("RestartPannel");
		
		if (restartPanel == null)
		{
			GD.PrintErr("RestartPannel (Sprite2D) not found in scene tree!");
			return;
		}

		var youScoreValue = restartPanel.GetNodeOrNull<Label>("YouScore/YouScoreValue");
		var bestScoreValue = restartPanel.GetNodeOrNull<Label>("BestScore/BestScoreValue");
		
		if (youScoreValue == null || bestScoreValue == null)
		{
			GD.PrintErr($"Labels not found! YouScoreValue: {youScoreValue != null}, BestScoreValue: {bestScoreValue != null}");
			return;
		}

		youScoreValue.Text = CurrentScore.ToString();
		bestScoreValue.Text = BestScore.ToString();
		
		restartPanel.Show();
	}

	private void LoadGameData()
	{
		if (!FileAccess.FileExists(SAVE_PATH)) return;
		
		try
		{
			using var file = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Read);
			var encryptedData = file.GetAsText();
			var json = Json.ParseString(DecryptData(encryptedData));
			
			var data = json.AsGodotDictionary();
			int score = (int)data["score"];
			string savedHash = (string)data["hash"];
			
			if (VerifyData(score, savedHash))
			{
				BestScore = score;
			}
		}
		catch (Exception e)
		{
			GD.PrintErr($"Ошибка загрузки: {e.Message}");
		}
	}

	private void SaveGameData()
	{
		try
		{
			var data = new Godot.Collections.Dictionary
			{
				["score"] = BestScore,
				["hash"] = ComputeDataHash(BestScore)
			};
			
			using var file = FileAccess.Open(SAVE_PATH, FileAccess.ModeFlags.Write);
			file.StoreString(EncryptData(Json.Stringify(data)));
		}
		catch (Exception e)
		{
			GD.PrintErr($"Ошибка сохранения: {e.Message}");
		}
	}

	private string ComputeDataHash(int score)
	{
		using var sha256 = SHA256.Create();
		byte[] bytes = sha256.ComputeHash(
			Encoding.UTF8.GetBytes($"{score}{GetSecretSalt()}")
		);
		return BitConverter.ToString(bytes).Replace("-", "").ToLower();
	}

	private bool VerifyData(int score, string savedHash)
	{
		string computedHash = ComputeDataHash(score);
		return savedHash == computedHash;
	}

	private string EncryptData(string data)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(data);
		byte[] key = Encoding.UTF8.GetBytes(GetSecretSalt());
		
		for (int i = 0; i < bytes.Length; i++)
		{
			bytes[i] ^= key[i % key.Length];
		}
		
		return Convert.ToBase64String(bytes);
	}

	private string DecryptData(string encryptedData)
	{
		byte[] bytes = Convert.FromBase64String(encryptedData);
		byte[] key = Encoding.UTF8.GetBytes(GetSecretSalt());
		
		for (int i = 0; i < bytes.Length; i++)
		{
			bytes[i] ^= key[i % key.Length];
		}
		
		return Encoding.UTF8.GetString(bytes);
	}

	public void ResetGame()
	{
		var youScore = GetTree().Root.GetNodeOrNull<YouScore>("YouScore");
		CurrentLives = 3;
		CurrentScore = 0;
		_gameOver = false;
		GetTree().Paused = false;
		youScore?.ResetScore();
		UpdateHeartsUI();
	}
}
