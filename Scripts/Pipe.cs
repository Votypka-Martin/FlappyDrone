using Godot;
using System;

public partial class Pipe : Node2D
{
	[Export] public int MinSpace = 20;
	[Export] public int MaxSpace = 50;
	[Export] public float Speed = 100;
	[Export] public Node2D UpperPipe;
	[Export] public Node2D LowerPipe;
	[Export] public Node2D Player;
	[Export] public AudioStreamPlayer Audio;

	public Action OnPassed { get; set; }

	public Action OnHit { get; set; }

	public Action OnScreenExited { get; set; }

	private bool _passed = false;
	
	public override void _Ready()
	{
		var spaceY = RandomSpace(MinSpace, MaxSpace);
		UpperPipe.Position = new Vector2(UpperPipe.Position.X, UpperPipe.Position.Y - spaceY);
		LowerPipe.Position = new Vector2(LowerPipe.Position.X, LowerPipe.Position.Y + spaceY);

		var upperPipeArea = UpperPipe.GetNode<Area2D>("Area2D");
		upperPipeArea.BodyEntered += (body) => OnHit?.Invoke();
		var lowerPipeArea = LowerPipe.GetNode<Area2D>("Area2D");
		lowerPipeArea.BodyEntered += (body) => OnHit?.Invoke();

		var notifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
        notifier.ScreenExited += ScreenExited;

		Position = new Vector2(
			Position.X + RandomSpace(0, 30),
			Position.Y + RandomSpace(-200, 200)
		);
    }

    private void ScreenExited()
    {
		OnScreenExited?.Invoke();
    }

	public override void _Process(double delta)
	{
		var position = Position;
		position.X -= Speed * (float)delta;
		Position = position;
		
		if (Player != null)
		{
			var distance = GlobalPosition.DistanceTo(Player.GlobalPosition);
			if (Audio != null)
			{
				Audio.VolumeDb = -distance / 100;
			}
			if(position.X < Player.Position.X)
			{
				if (!_passed)
				{
					OnPassed?.Invoke();
					_passed = true;
				}
			}
		}
		
	}

	private int RandomSpace(int min, int max)
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		rng.Randomize();
		return rng.RandiRange(min, max);
	}
}
