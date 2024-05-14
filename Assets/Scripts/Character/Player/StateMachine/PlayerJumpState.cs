using Godot;
using System;

public partial class PlayerJumpState : PlayerState
{
	public override void Enter(State previousState) {
		base.Enter(previousState);
		
		// TODO: Reset Colliders

		MovementController.Jump();
	}

    public override void Process(double delta)
    {
		
    }

    public override void PhysicsProcess(double delta) {
		base.PhysicsProcess(delta);

		_HandleHorizontalMovement();

		MovementController.Fall();
	}

	private void _HandleHorizontalMovement() {
		Vector2 inputDir = MovementController.InputVector;
		
		if(inputDir != Vector2.Zero) {
			if((inputDir.X > 0 && MovementController.Velocity.X < 0) || (inputDir.X < 0 && MovementController.Velocity.X > 0)) {
				MovementController.SwitchDirection(inputDir);
			} else {
				MovementController.Accelerate(inputDir);
			}
		} else {
			MovementController.AddFriction();
		}
	}

	protected override bool CheckStates() {
		if(MovementController.DesiredFall) { // Add if vel is neg
			MovementController.JumpFallTransition();
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.FallState);
			return true;
		}

		if(MovementController.Velocity.Y > 0) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.FallState);
		}

		return false;
	}
}
