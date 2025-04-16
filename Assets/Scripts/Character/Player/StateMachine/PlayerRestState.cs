using Godot;
using System;

public partial class PlayerRestState : PlayerState
{
	public override void Initialize(StateMachine playerStateMachine) {
		base.Initialize(playerStateMachine);
		

		// _player.E += _EnterDefaultState;
	}
	// Called when the node enters the scene tree for the first time.
	public override void Enter(State previousState) {
		base.Enter(previousState);
		CanFlip = false;
		// Camera.Zoom = new Vector2(Camera.Zoom.X * CameraZoom, Camera.Zoom.Y * CameraZoom);

		// TODO: Reset Colliders
	}

	public override void PlayStateAnimation() {
		AnimationController.PlayAnimation(AnimationController.IdleAnimationClip);
		// TODO: Start Idle Animation
	}

	public override void PhysicsProcess(double delta) {
		base.PhysicsProcess(delta);
		MovementController.AddFriction(delta);
	}
	
	// private void _EnterDefaultState() {
	// 	if(!ActiveState) {
	// 		return;
	// 	}
	// 	ParentPlayerStateMachine.EnterDefaultState();
	// }
}
