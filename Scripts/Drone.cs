using Godot;
using System;

public partial class Drone : CharacterBody2D
{
	public float FlySpeed = 700.0f;
	public float Gravity = 500.0f;
	public float MaxFlySpeed = 700.0f;
	public Action OnFloorHit;
	[Export] public float MaxAnimationSpeed = 7.0f;
	[Export] public float AnimationAcceleration = 10.0f;
	[Export] public float AnimationDeceleration = 5.0f;
	[Export] public AnimatedSprite2D Animation;
	[Export] public Node2D Top;
	[Export] public Node2D Bottom;
	[Export] public AudioStreamPlayer Audio;
	private float _animationSpeed = 0.0f;
	private bool _fly = false;

	public override void _Process(double delta)
	{
		_fly = Input.IsActionPressed("_fly");
		if (_fly)
		{
			if (!Audio.Playing) Audio.Play();
			Audio.VolumeDb = Mathf.Lerp(Audio.VolumeDb, 0.0f, 5.0f * (float)delta);
			_animationSpeed = Mathf.Lerp(
				_animationSpeed,
				MaxAnimationSpeed,
				AnimationAcceleration * (float)delta
			);
		}
		else
		{
			Audio.VolumeDb = Mathf.Lerp(Audio.VolumeDb, -20.0f, 3.0f * (float)delta);
			if (Audio.Playing && Audio.VolumeDb < -15) Audio.Stop();
			_animationSpeed = Mathf.Lerp(
				_animationSpeed,
				0.0f,
				AnimationDeceleration * (float)delta
			);
		}
		Animation.SpeedScale = _animationSpeed;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_fly)
		{
			var velocity = Velocity;
			velocity.Y -= FlySpeed * (float)delta;
			velocity.Y = Mathf.Max(velocity.Y, -MaxFlySpeed);
			Velocity = velocity;
		} else
		{
			var velocity = Velocity;
			velocity.Y += Gravity * (float)delta;
			Velocity = velocity;
		}
		MoveAndSlide();
		if (Top.GlobalPosition.Y < 0)
		{
			GlobalPosition = new Vector2(GlobalPosition.X, -Top.Position.Y);
			Velocity = Vector2.Zero;
		}
		if (Bottom.GlobalPosition.Y > GetViewportRect().Size.Y)
		{			
			GlobalPosition = new Vector2(GlobalPosition.X, - Top.Position.Y);
			Velocity = Vector2.Zero;
			OnFloorHit?.Invoke();
		}
	}
}
