using Godot;
using System;

public partial class PlayerFallState : PlayerState
{
	public override void PlayStateAnimation() {
		AnimationController.PlayAnimation(AnimationController.FallAnimationClip);
		// TODO: Start Idle Animation
	}
	
	public override void Enter(State previousState) {
		base.Enter(previousState);
		// TODO: Reset Colliders
	}

	public override void Process(double delta)
	{
		if(MovementController.DesiredJump) {
			MovementController.StartJumpBuffer();
		}
	}

	 public override void PhysicsProcess(double delta) {
		base.PhysicsProcess(delta);
		
		_HandleHorizontalMovement(delta);

		MovementController.Fall(delta);
	}

	private void _HandleHorizontalMovement(double delta) {
		Vector2 inputDir = MovementController.InputVector;
		
		if(inputDir != Vector2.Zero) {
			if((inputDir.X > 0 && MovementController.Velocity.X < 0) || (inputDir.X < 0 && MovementController.Velocity.X > 0)) {
				MovementController.SwitchDirection(inputDir, delta);
			} else {
				MovementController.Accelerate(inputDir, delta);
			}
		} else {
			MovementController.AddFriction(delta);
		}
	}

	protected override bool CheckStates() {
		/*if(DeflectController.DesiredDeflect) {
			DeflectController.StartBlock();
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.BlockState);
			return true;
		}*/
		if((MovementController.DesiredDash || (!MovementController.GetDashBufferStop() && MovementController.CanDash)) 
			&& Stats.CanAirDash && MovementController.UseAirDash()) {
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
		if(MovementController.Grounded) {
			ParentPlayerStateMachine.EnterDefaultState();
			return true;
		} else if(MovementController.DesiredJump && !MovementController.GetCoyoteStop()) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.JumpState);
			return true;
		}

		return false;
	}
}
