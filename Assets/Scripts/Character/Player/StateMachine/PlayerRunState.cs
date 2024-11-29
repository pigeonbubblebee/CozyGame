using Godot;
using System;

public partial class PlayerRunState : PlayerState
{
	public override void Enter(State previousState) {
		base.Enter(previousState);

		// TODO: Reset Colliders
	}

	public override void PlayStateAnimation() {
		// TODO: Start Idle Animation
	}

	public override void Process(double delta)
	{
		
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
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.IdleState);
		}
	}

	protected override bool CheckStates() {
		if(HealController.DesiredHeal && HealController.HealCooldownOff) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.HealState);
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
}
