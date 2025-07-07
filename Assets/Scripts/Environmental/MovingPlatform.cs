using Godot;
using System;

public partial class MovingPlatform : PathFollow2D
{
	[Export]
	public float MoveSpeed { get; private set; } = 1f;
	[Export]
	public float IdleTimeBetweenFlips { get; private set; } = 0.4f;

	public int MoveDirection { get; private set; } = 1;
	public bool Idle { get; private set; }

    public override void _PhysicsProcess(double delta)
    {
		if(!Idle)
       		this.ProgressRatio += MoveSpeed * MoveDirection * (float)delta;

		_CheckPlatformFlip();
    }

	private void _CheckPlatformFlip() {
		if(this.ProgressRatio <= 0f || this.ProgressRatio >= 1f) {
			this.MoveDirection *= -1;

			Idle = true;
			GetTree().CreateTimer(IdleTimeBetweenFlips).Timeout += delegate { Idle = false; };
		}
	}
}
