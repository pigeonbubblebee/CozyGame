using Godot;
using System;

public partial class JumpAttack : SlashAttack
{
	[Export] public float VerticalSpeed;
	private bool JumpStarted;
	
	// [Export] private bool _checkLedge = false;
	[Export] private bool _stopAccelOnInterrupt = false;
	
	private bool _fired = false;

	[Export] public int JumpFrame;
	[Export] public int StopJumpFrame;
	[Export] public int FlySpeed;


	public override void _Ready()
	{
		base._Ready();
		FinishAttackEvent += _EnableKnockback;
		//		FinishAttackEvent += _FinishLunge;
	}

	protected override void _checkAnimationEvent()
	{
		base._checkAnimationEvent();
		if (EnemyAI.Sprite.Frame == JumpFrame && EnemyAI.Sprite.Animation == AnimationName && !_fired)
		{
			JumpStarted = true;
			EnemyAI.Velocity = new Vector2(EnemyAI.Velocity.X, -VerticalSpeed);
			_fired = true;
		}
		if (EnemyAI.Sprite.Frame == StopJumpFrame && EnemyAI.Sprite.Animation == AnimationName)
		{
			JumpStarted = false;
			
		}
	}
	
	public override void Execute(Player p, Enemy e) {
		base.Execute(p, e);
		_fired = false;
		e.KnockbackEnabled = false;
	}
	
	public override void _Process(double delta) {
		if (!Active)
		{
			return;
		}
		if (!JumpStarted)
		{
			((EnemyPatrolAI)EnemyAI).Decelerate();
			return;
		}
		if (!EnemyAI.IsOnFloor())
			{
				((EnemyPatrolAI)EnemyAI).Accelerate(FlySpeed);
			}

		
	}
	
	public override void Interrupt() {
		base.Interrupt();
		if(!_stopAccelOnInterrupt)
			JumpStarted = false;
		// ((EnemyPatrolAI)EnemyAI).Decelerate();
		_EnableKnockback();
	}
	
	private void _EnableKnockback() {
		EnemyAI.KnockbackEnabled = true;
	}

}
