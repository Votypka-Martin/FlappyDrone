using Godot;
using System;

public partial class GameOver : Node2D
{
	[Export] public Label ScoreLabel;
	public int Score
	{
		get => _score;
		set
		{
			_score = value;
			ScoreLabel.Text = _score.ToString();
		}
	}
	private int _score;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("_fly"))
		{
			Restart();
		}
	}

	private void Restart()
	{
		GetTree().ChangeSceneToFile("res://Scenes/Game.tscn");
	}
}
