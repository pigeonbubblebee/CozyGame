using Godot;
using System;

public partial class PounceAttack : SlashAttack
{
	[Export] public float VerticalLungeSpeed;

	public override void _Ready() {
		base._Ready();
		FinishAttackEvent += _EnableKnockback;
	}
	
	protected override void _checkAnimationEvent() {
		base._checkAnimationEvent();
		
		if(EnemyAI.Sprite.Frame == OnFrame && EnemyAI.Sprite.Animation == AnimationName) {
			EnemyAI.Velocity = new Vector2(EnemyAI.Velocity.X, -VerticalLungeSpeed);
		}
	}
	
	public override void Execute(Player p, Enemy e) {
		base.Execute(p, e);

		e.KnockbackEnabled = false;
		
		
	}
	
	public override void _Process(double delta) {
		if(!Active)
			return;
		if(_accelerating) {
			((EnemyPatrolAI)EnemyAI).Accelerate(LungeSpeed);
		} else {
			((EnemyPatrolAI)EnemyAI).Decelerate();
		}
	}
	
	public override void Interrupt() {
		base.Interrupt();
		
		_EnableKnockback();
	}
	
	private void _EnableKnockback() {
		EnemyAI.KnockbackEnabled = true;
	}
}
