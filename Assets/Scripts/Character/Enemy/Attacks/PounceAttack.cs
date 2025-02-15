using Godot;
using System;

public partial class PounceAttack : SlashAttack
{
	[Export] public float VerticalLungeSpeed;
	private bool LungeStarted;
	
	[Export] private bool _checkLedge = false;
	[Export] private bool _stopAccelOnInterrupt = false;
	
	private bool _fired = false;

	public override void _Ready() {
		base._Ready();
		FinishAttackEvent += _EnableKnockback;
		FinishAttackEvent += _FinishLunge;
	}
	
	protected override void _checkAnimationEvent() {
		base._checkAnimationEvent();
		
		if(EnemyAI.Sprite.Frame == OnFrame && EnemyAI.Sprite.Animation == AnimationName && !_fired) {
			LungeStarted = true;
			EnemyAI.Velocity = new Vector2(EnemyAI.Velocity.X, -VerticalLungeSpeed);
			_fired = true;
		}
	}
	
	public override void Execute(Player p, Enemy e) {
		base.Execute(p, e);
		_fired = false;

		e.KnockbackEnabled = false;
	}
	
	public override void _Process(double delta) {
		if(!LungeStarted)
		  	return;
		if(_checkLedge) {
			if(!((EnemyPatrolAI)EnemyAI).GetRayCastLedge()) {
				((EnemyPatrolAI)EnemyAI).Decelerate();
				return;
			}
		}
		if(_accelerating) {
			((EnemyPatrolAI)EnemyAI).Accelerate(LungeSpeed);
		} else {
			((EnemyPatrolAI)EnemyAI).Decelerate();
		}
	}
	
	public override void Interrupt() {
		base.Interrupt();
		if(!_stopAccelOnInterrupt)
			_accelerating = true;
		// ((EnemyPatrolAI)EnemyAI).Decelerate();
		_EnableKnockback();
	}
	
	private void _EnableKnockback() {
		EnemyAI.KnockbackEnabled = true;
	}
	
	private void _FinishLunge() {
		_accelerating = false;
			// _canHit = false;
		_canHit = false;
			// _rightAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		_attackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		LungeStarted = false;
	}
}
