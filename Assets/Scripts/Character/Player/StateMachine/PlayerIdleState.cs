using Godot;
using System;

public partial class PlayerIdleState : PlayerState
{
	public override void Enter(State previousState) {
		base.Enter(previousState);

		// TODO: Reset Colliders
	}

	public override void PlayStateAnimation() {
		AnimationController.PlayAnimation(AnimationController.IdleAnimationClip);
		// TODO: Start Idle Animation
	}

	public override void PhysicsProcess(double delta) {
		base.PhysicsProcess(delta);
		
		MovementController.AddFriction(delta);
	}

	protected override bool CheckStates()
	{
		if(PostureController.CurrentPosture <= 0) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.StaggerState);
			return true;
		}
		if(PostureController.CurrentPosture <= 0) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.StaggerState);
			return true;
		}
		if(DeflectController.DeflectActuation || (!DeflectController.GetDeflectBufferStop() && DeflectController.CanBlock)) {
			DeflectController.StartBlock();
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.BlockState);
			return true;
		}
		if(HealController.DesiredHeal && HealController.HealCooldownOff) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.HealState);
			return true;
		}
		if(MovementController.DesiredDash || (!MovementController.GetDashBufferStop() && MovementController.CanDash)) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.DashState);
			return true;
		}
		if(SpellController.DesiredShoot || (!SpellController.GetShootBufferStop()  && SpellController.CanShoot)) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.SpellState);
			return true;
		}
		if(AttackController.DesiredAttack || (!AttackController.GetSlashBufferStop() && AttackController.CanSlash)) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.AttackState);
			return true;
		}
		if(!MovementController.Grounded) {
			MovementController.StartCoyoteTimer();
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.FallState);
			return true;
		}
		if(MovementController.DesiredJump || !MovementController.GetJumpBufferStop()) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.JumpState);
			return true;
		}
		if(MovementController.InputVector != Vector2.Zero) {
		 	ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.RunState);
			return true;
		}

		return false;
	}
}
