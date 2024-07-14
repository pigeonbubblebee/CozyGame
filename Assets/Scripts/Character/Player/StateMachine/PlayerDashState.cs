using Godot;
using System;
using System.Threading.Tasks;

public partial class PlayerDashState : PlayerState
{
	public override void Initialize(StateMachine playerStateMachine) {
		base.Initialize(playerStateMachine);

		MovementController.FinishDashEvent += _EnterDefaultState;
	}

	public override void Enter(State previousState) {
		base.Enter(previousState);

		// TODO: Reset Colliders
		
		// MovementController.DashAccelerate();

		MovementController.Dash();
		MovementController.StartDashCooldown();
	}

	public override void PlayStateAnimation() {
		// TODO: Start Idle Animation
	}

	public override void Process(double delta)
    {
		if(AttackController.DesiredAttack) {
			AttackController.StartSlashBuffer();
		}
		if(MovementController.DesiredDash) {
			MovementController.StartDashBuffer();
		}
    }

	public override void PhysicsProcess(double delta) {
		base.PhysicsProcess(delta);

		// MovementController.AddDashFriction();
		MovementController.DashAccelerate();

		// Vector2 inputDir = MovementController.InputVector;
		
		// if(inputDir != Vector2.Zero) {
		// 	if((inputDir.X > 0 && MovementController.Velocity.X < 0) || (inputDir.X < 0 && MovementController.Velocity.X > 0)) {
		// 		MovementController.SwitchDirection(inputDir);
		// 	} else {
				
		// 	}
		// } else {
		// 	ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.IdleState);
		// }
	}

	private void _EnterDefaultState() {
        ParentPlayerStateMachine.EnterDefaultState();
    }
}
