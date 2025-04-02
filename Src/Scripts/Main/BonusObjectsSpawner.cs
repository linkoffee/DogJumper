using Godot;
using Godot.Collections;

public partial class BonusObjectsSpawner : Node
{
	[Export] public PackedScene[] BonusPrefabs;
	[Export] public float SpawnInterval = 10f;
	[Export] public float MinSpacing = 50f;
	[Export] public float MaxJumpHeight = 200f;
	[Export] public NodePath TargetParallaxLayerPath;
	
	private Timer spawnTimer;
	private ParallaxLayer targetLayer;
	private ParallaxBg parallaxBg;
	private float playerStartY;
	private Array<Node> spawnedObjects = new();

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
			GD.PrintErr("Не удалось найти ParallaxBackground как родителя целевого слоя");
			return;
		}
		
		Node2D player = GetTree().GetFirstNodeInGroup("player") as Node2D;
		playerStartY = player != null ? player.Position.Y : 500;

		spawnTimer = new Timer();
		spawnTimer.WaitTime = SpawnInterval;
		spawnTimer.OneShot = false;
		spawnTimer.Timeout += SpawnObject;
		AddChild(spawnTimer);
		spawnTimer.Start();
	}

	private void SpawnObject()
	{
		if (BonusPrefabs.Length == 0) return;

		PackedScene randomPrefab = BonusPrefabs[(int)GD.RandRange(0, BonusPrefabs.Length - 1)];
		Node newObject = randomPrefab.Instantiate<Node>(); 

		if (newObject is not Area2D area2DObject)
		{
			GD.PrintErr($"Ошибка: объект {newObject.Name} не является Area2D!");
			return;
		}

		float minY = playerStartY - MaxJumpHeight;
		float maxY = playerStartY;
		float spawnY = (float)GD.RandRange(minY, maxY);

		float screenWidth = GetViewport().GetVisibleRect().Size.X;

		if (spawnedObjects.Count > 0)
		{
			Node lastObj = spawnedObjects[spawnedObjects.Count - 1];
			if (lastObj is Area2D lastArea2D && screenWidth - lastArea2D.Position.X < MinSpacing)
				return;
		}

		area2DObject.Position = new Vector2(screenWidth, spawnY);
		GetParent().AddChild(area2DObject);
		spawnedObjects.Add(area2DObject);
	}

	public override void _Process(double delta)
	{
		if (targetLayer == null || parallaxBg == null) return;

		float layerSpeedMultiplier = targetLayer.GetMeta("scrollSpeed", 1.0f).AsSingle();
		float currentSpeed = parallaxBg.BaseSpeed * layerSpeedMultiplier;
		
		for (int i = spawnedObjects.Count - 1; i >= 0; i--)
		{
			if (spawnedObjects[i] is not Area2D area2DObj || !IsInstanceValid(area2DObj))
			{
				spawnedObjects.RemoveAt(i);
				continue;
			}

			area2DObj.Position -= new Vector2((float)(currentSpeed * delta), 0);

			if (area2DObj.Position.X < -100)
			{
				spawnedObjects.RemoveAt(i);
				area2DObj.QueueFree();
			}
		}
	}
}
