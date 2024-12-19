using Godot;
using System;

public partial class PlayerStaggerState : PlayerState
{
	public override void Initialize(StateMachine playerStateMachine) {
		base.Initialize(playerStateMachine);

		PostureController.PostureRecoverEvent += _EnterDefaultState;
	}
	// Called when the node enters the scene tree for the first time.
	public override void Enter(State previousState) {
		base.Enter(previousState);
		
		PostureController.StartPostureRecovery();
		CanFlip = false;

		// TODO: Reset Colliders
	}

	public override void PlayStateAnimation() {
		AnimationController.PlayAnimation(AnimationController.IdleAnimationClip);
		// TODO: Start Idle Animation
	}

	public override void PhysicsProcess(double delta) {
		base.PhysicsProcess(delta);
		
		MovementController.AddFriction(delta);
		if(!MovementController.Grounded) {
			MovementController.Fall(delta);
		}
	}
	
	private void _EnterDefaultState() {
		if(!ActiveState) {
			return;
		}
		ParentPlayerStateMachine.EnterDefaultState();
	}
}
