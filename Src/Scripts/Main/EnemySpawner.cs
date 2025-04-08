using Godot;
using Godot.Collections;

public partial class EnemySpawner : Node
{
	[Export] public PackedScene[] EnemyPrefabs;
	[Export] public float SpawnInterval = 5f;
	[Export] public float MinSpacing = 70f;
	[Export] public float SpeedMultiplier = 4f;
	[Export] public NodePath TargetParallaxLayerPath;
	
	private Timer spawnTimer;
	private ParallaxLayer targetLayer;
	private ParallaxBg parallaxBg;
	private Array<Node> spawnedEnemies = new();

	[Export] public Dictionary<string, float> EnemyFixedHeights = new()
	{
		{"EagleEnemy", 300f},
		{"HedgehogEnemy", 550f}
	};

	public override void _Ready()
	{
		targetLayer = GetNode<ParallaxLayer>(TargetParallaxLayerPath);
		
		var parallaxBackground = targetLayer.GetParent<ParallaxBackground>();
		if (parallaxBackground != null)
		{
			parallaxBg = parallaxBackground.GetNode<ParallaxBg>(".");
		}
		else
		{
			GD.PrintErr("Не удалось найти ParallaxBackground");
			return;
		}

		spawnTimer = new Timer();
		spawnTimer.WaitTime = SpawnInterval;
		spawnTimer.OneShot = false;
		spawnTimer.Timeout += SpawnEnemy;
		AddChild(spawnTimer);
		spawnTimer.Start();
	}

	private void SpawnEnemy()
	{
		if (EnemyPrefabs.Length == 0) return;

		PackedScene randomPrefab = EnemyPrefabs[(int)GD.RandRange(0, EnemyPrefabs.Length - 1)];
		Node newEnemy = randomPrefab.Instantiate<Node>();

		if (newEnemy is not CharacterBody2D enemyCharacter)
		{
			GD.PrintErr($"Ошибка: враг {newEnemy.Name} не является CharacterBody2D!");
			return;
		}

		float screenWidth = GetViewport().GetVisibleRect().Size.X;

		if (spawnedEnemies.Count > 0)
		{
			Node lastEnemy = spawnedEnemies[spawnedEnemies.Count - 1];
			if (lastEnemy is CharacterBody2D lastCharacter && screenWidth - lastCharacter.Position.X < MinSpacing)
				return;
		}

		float spawnY = GetFixedHeightForEnemy(newEnemy.Name);

		enemyCharacter.Position = new Vector2(screenWidth, spawnY);
		GetParent().AddChild(enemyCharacter);
		spawnedEnemies.Add(enemyCharacter);
	}

	private float GetFixedHeightForEnemy(string enemyName)
	{
		if (EnemyFixedHeights.TryGetValue(enemyName, out float height))
		{
			return height;
		}
		return 400f;
	}

	public override void _Process(double delta)
	{
		if (targetLayer == null || parallaxBg == null) return;

		float layerSpeedMultiplier = targetLayer.GetMeta("scrollSpeed", 1.0f).AsSingle();
		float currentSpeed = parallaxBg.BaseSpeed * layerSpeedMultiplier * SpeedMultiplier;
		
		for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
		{
			if (spawnedEnemies[i] is not CharacterBody2D enemyObj || !IsInstanceValid(enemyObj))
			{
				spawnedEnemies.RemoveAt(i);
				continue;
			}

			enemyObj.Position -= new Vector2((float)(currentSpeed * delta), 0);

			if (enemyObj.Position.X < -100)
			{
				spawnedEnemies.RemoveAt(i);
				enemyObj.QueueFree();
			}
		}
	}
}
