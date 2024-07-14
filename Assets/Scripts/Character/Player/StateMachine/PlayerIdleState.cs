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
		if(MovementController.DesiredDash || (!MovementController.GetDashBufferStop() && MovementController.CanDash)) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.DashState);
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