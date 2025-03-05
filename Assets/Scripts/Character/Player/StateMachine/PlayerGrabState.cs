using Godot;
using System;

public partial class PlayerGrabState : PlayerState
{
	[Export]
	public float CameraZoom = 1.2f;
	
	private Vector2 _originalZoom;
	
	public override void Initialize(StateMachine playerStateMachine) {
		base.Initialize(playerStateMachine);
		

		_player.ExitGrabEvent += _EnterDefaultState;
	}
	// Called when the node enters the scene tree for the first time.
	public override void Enter(State previousState) {
		base.Enter(previousState);
		CanFlip = false;
		_originalZoom = new Vector2(Camera.Zoom.X, Camera.Zoom.Y);
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(Camera, "zoom", _originalZoom * CameraZoom, 0.075f);
		// Camera.Zoom = new Vector2(Camera.Zoom.X * CameraZoom, Camera.Zoom.Y * CameraZoom);

		// TODO: Reset Colliders
	}

	public override void PlayStateAnimation() {
		AnimationController.PlayAnimation(AnimationController.GrabAnimationClip);
		// TODO: Start Idle Animation
	}

	public override void PhysicsProcess(double delta) {
		base.PhysicsProcess(delta);
		MovementController.AddFriction(delta);
	}
	
	private void _EnterDefaultState() {
		if(!ActiveState) {
			return;
		}
		ParentPlayerStateMachine.EnterDefaultState();
	}
	
	public override void Exit() {
		base.Exit();
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(Camera, "zoom", _originalZoom, 0.15f);
	}
}
