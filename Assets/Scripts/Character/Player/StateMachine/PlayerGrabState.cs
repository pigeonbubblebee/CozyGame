using Godot;
using System;

public partial class PlayerGrabState : PlayerState
{
	public override void Initialize(StateMachine playerStateMachine) {
		base.Initialize(playerStateMachine);

		_player.ExitGrabEvent += _EnterDefaultState;
	}
	// Called when the node enters the scene tree for the first time.
	public override void Enter(State previousState) {
		base.Enter(previousState);
		CanFlip = false;

		// TODO: Reset Colliders
	}

	public override void PlayStateAnimation() {
		AnimationController.PlayAnimation(AnimationController.GrabAnimationClip);
		// TODO: Start Idle Animation
	}

	public override void PhysicsProcess(double delta) {
		base.PhysicsProcess(delta);
	}
	
	private void _EnterDefaultState() {
		if(!ActiveState) {
			return;
		}
		ParentPlayerStateMachine.EnterDefaultState();
	}
}
