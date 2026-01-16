using Godot;
using System;

public partial class BaseGame : Node2D
{
	[Export] public PackedScene PipePrefab;
	[Export] public PackedScene GameOverScene;
	[Export] public Node2D PipeSpawnPoint;
	[Export] public int TargetPipeCount = 2;
	[Export] public Drone Drone;
	[Export] public Label ScoreLabel;
	[Export] public AudioStreamPlayer ScoreSound;
	public int Score
	{
		get => _score;
		set
		{
			if (value > _score) ScoreSound.Play();
			_score = value;
			ScoreLabel.Text = _score.ToString();
		}
	}
	private int _targetPipeCount = 2;
	private int _currentPipeCount = 0;
	private float _screenWidth = 0;
	private int _score = 0;

	public override void _Ready()
	{
		_targetPipeCount = TargetPipeCount;
		_screenWidth = GetViewport().GetVisibleRect().Size.X * GetViewport().GetCamera2D().Zoom.X;
		SpawnPipes();
		Drone.OnFloorHit += OnFloorHit;
	}

	private void SpawnPipes()
	{
		if (_currentPipeCount >= TargetPipeCount) return;
		if (PipeSpawnPoint == null || PipePrefab == null) return;
		var pipesToSpawn = TargetPipeCount - _currentPipeCount;
		var disp = new Vector2(_screenWidth / TargetPipeCount, 0);
		for (int i = 0; i < pipesToSpawn; i++)
		{
			var pipe = PipePrefab.Instantiate() as Pipe;
			pipe.Player = Drone;
			pipe.Position = PipeSpawnPoint.Position + disp * i;
			pipe.OnHit += OnPipeHit;
			pipe.OnPassed += OnPipePassed;
			pipe.OnScreenExited += () => 
			{
				_currentPipeCount--;
				pipe.QueueFree();
				SpawnPipes();
			};
			AddChild(pipe);
		}
		_currentPipeCount = TargetPipeCount;
	}

	public void GameOver()
	{
		GameOver gameOver = GameOverScene.Instantiate() as GameOver;
		gameOver.Score = Score;
		GetTree().Root.AddChild(gameOver);
		QueueFree();
		GetTree().CurrentScene = gameOver;
	}

	private void OnPipePassed()
	{
		PipePassed();
	}

	private void OnFloorHit()
	{
        FloorHit();
	}

	private void OnPipeHit()
	{
        PipeHit();
	}

    public virtual void PipePassed() { }

    public virtual void FloorHit() { }

    public virtual void PipeHit() { }
}
