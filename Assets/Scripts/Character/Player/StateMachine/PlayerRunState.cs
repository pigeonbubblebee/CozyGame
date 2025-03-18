using Godot;
using System;

public partial class PlayerRunState : PlayerState
{
	public override void Enter(State previousState) {
		base.Enter(previousState);
		MovementController.RunningSFXPlaying = true;

		// TODO: Reset Colliders
	}

	public override void PlayStateAnimation() {
		AnimationController.PlayAnimation(AnimationController.RunAnimationClip);
		// TODO: Start Idle Animation
	}

	public override void PhysicsProcess(double delta) {
		base.PhysicsProcess(delta);

		Vector2 inputDir = MovementController.InputVector;
		
		if(inputDir != Vector2.Zero) {
			if((inputDir.X > 0 && MovementController.Velocity.X < 0) || (inputDir.X < 0 && MovementController.Velocity.X > 0)) {
				MovementController.SwitchDirection(inputDir, delta);
			} else {
				MovementController.Accelerate(inputDir, delta);
			}
		} else {
			if(!ActiveState)
				return;
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.IdleState);
		}
	}

	protected override bool CheckStates() {
		if(DeflectController.DeflectActuation || (!DeflectController.GetDeflectBufferStop() && DeflectController.CanBlock)) {
			DeflectController.StartBlock();
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.BlockState);
			return true;
		}
		if(HealController.DesiredHeal && HealController.HealCooldownOff) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.HealState);
			return true;
		}
		if(MovementController.DesiredDashRaw && DeflectController.CanCleaveCounter) {
			DeflectController.CounterCleave();
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.GrabState);
			// ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.DashState);
			return true;
		}
		if(MovementController.DesiredDash || (!MovementController.GetDashBufferStop() && MovementController.CanDash)) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.DashState);
			return true;
		}
		if(SpellController.DesiredShoot || (!SpellController.GetShootBufferStop() && SpellController.CanShoot)) {
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

		return false;
	}
	
	public override void Exit() {
		base.Exit();
		MovementController.RunningSFXPlaying = false;
	}
}
