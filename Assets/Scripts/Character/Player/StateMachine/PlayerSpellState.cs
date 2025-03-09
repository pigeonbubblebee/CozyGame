using Godot;
using System;

public partial class PlayerSpellState : PlayerState
{
	public override void Initialize(StateMachine playerStateMachine) {
		base.Initialize(playerStateMachine);

		SpellController.FinishShootEvent += _EnterDefaultState;
	}
	
	public override void Enter(State previousState) {
		base.Enter(previousState);
		
		if(!(SpellController.CurrentMana >= Stats.ManaUsage)) {
			ParentPlayerStateMachine.EnterDefaultState();
			return;
		}
		
		CanFlip = false;
		
		SpellController.UseSpell();
		SpellController.StartShootCooldown();
		// GetTree().CreateTimer(Stats.ShootTime).Timeout += _EnterDefaultState;
		
		MovementController.Recoil(Stats.ShootMaxRecoil); // Link to Pstats
		// TODO: Reset Colliders
		
		MovementController.CanSwitchDirections = false;
	}

	public override void PlayStateAnimation() {
		// TODO: Start Idle Animation
		AnimationController.PlayAnimation(AnimationController.CounterAnimationClip, Stats.ShootTime);
	}

	public override void Process(double delta)
	{
		CheckStates();
		
		if(DeflectController.DeflectActuation) {
			DeflectController.StartDeflectBuffer();
		}
		if(AttackController.DesiredAttack) {
			AttackController.StartSlashBuffer();
		}
		if(MovementController.DesiredDash) {
			MovementController.StartDashBuffer();
		}
		if(MovementController.DesiredJump) {
			MovementController.StartJumpBuffer();
		}
		if(SpellController.DesiredShoot) {
			SpellController.StartShootBuffer();
		}
	}

	public override void PhysicsProcess(double delta) {
		base.PhysicsProcess(delta);
		
		MovementController.AddFriction(delta); // TODO: Add Recoil
		
	}

	protected override bool CheckStates()
	{
		return false;
	}
	
	private void _EnterDefaultState() {
		MovementController.CanSwitchDirections = true;
		if(!ActiveState)
			return;
		ParentPlayerStateMachine.EnterDefaultState();
	}
}
