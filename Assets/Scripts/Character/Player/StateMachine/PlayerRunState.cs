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
				MovementController.SwitchDirection(inputDir);
			} else {
				MovementController.Accelerate(inputDir);
			}
		} else {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.IdleState);
		}
	}

	protected override bool CheckStates() {
		if(!MovementController.Grounded) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.FallState);
			return true;
		} else if(MovementController.DesiredJump) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.JumpState);
			return true;
		}

		return false;
	}
}
