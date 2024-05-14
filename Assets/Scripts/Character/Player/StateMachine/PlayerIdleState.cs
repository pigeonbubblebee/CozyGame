using Godot;
using System;

public partial class PlayerIdleState : PlayerState
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
        CheckStates();
    }

    public override void PhysicsProcess(double delta) {
		base.PhysicsProcess(delta);
		
		MovementController.AddFriction();
	}

    protected override bool CheckStates()
    {
        if(!MovementController.Grounded) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.FallState);
			return true;
		}
		if(MovementController.DesiredJump) {
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