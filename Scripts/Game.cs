using Godot;
using System;

public partial class Game : BaseGame
{
    public override void PipePassed()
    {
        Score++;
    }

	public override void FloorHit()
	{
		GameOver();
	}

	public override void PipeHit()
	{
		GameOver();
	}

}
