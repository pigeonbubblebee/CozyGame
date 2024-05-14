using Godot;
using System;

// Handles Run/Jump, will add Dash and stuff

public partial class PlayerMovementController : Node
{
	[Export] public float Speed = 500.0f;
	[Export] public float Acceleration = 50.0f;
	[Export] public float DirectionSwitchSpeed = 100.0f;
	[Export] public float Friction = 70.0f;
	
	[Export] public float JumpVelocity = 4500.0f;
	[Export] public int MaxJumps = 2;

	public Vector2 Velocity => _playerBody.Velocity;
	// [Export] public float JumpReleaseVelocity = 100f;
	private int _CurrentJump = 1;
	public bool DesiredJump => _CheckDesiredJump();
	private bool _Jumping = false;
	public bool DesiredFall => _CheckDesiredFall();
	public bool Grounded => _playerBody.IsOnFloor();

	private CharacterBody2D _playerBody;

	public Vector2 InputVector => _GetInputVector();

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private IInputManager _inputManager;

	public override void _Ready()
	{
		_inputManager = GetNode<IInputManager>("/root/InputManager");
		GD.Print(_inputManager);
	}

	public void Initialize(CharacterBody2D body) {
		_playerBody = body;
	}

	public override void _PhysicsProcess(double delta)
	{
		_PlayerMovement();
		_ResetJump(); // Move
	}

	private void _ResetJump() {
		if(Grounded) {
			_CurrentJump = 1;
		}
	}

	private Vector2 _GetInputVector() {
		return _inputManager.GetMovementDirection();
	}

	public void Accelerate(Vector2 direction) {
		Vector2 velocity = _playerBody.Velocity.MoveToward(Speed * direction, Acceleration);
		velocity.Y = Velocity.Y;
		_playerBody.Velocity = velocity;
	}

	public void SwitchDirection(Vector2 direction) {
		Vector2 velocity = _playerBody.Velocity.MoveToward(Speed * direction, DirectionSwitchSpeed);
		velocity.Y = Velocity.Y;
		_playerBody.Velocity = velocity;
	}

	public void AddFriction() {
		Vector2 zero = Vector2.Zero;
		zero.Y = Velocity.Y;
		_playerBody.Velocity = _playerBody.Velocity.MoveToward(zero, Friction);
	}

	private void _PlayerMovement() {
		_playerBody.MoveAndSlide();
	}
	
	private bool _CheckDesiredJump() {
		if(_inputManager.GetJumpActuation()) {
			if(_CurrentJump < MaxJumps) { // Add Coyote & Buffer
				_CurrentJump ++;
				_Jumping = true;
			}

			return true;
		}

		return false;
	}

	private bool _CheckDesiredFall() {
		return _inputManager.GetJumpReleaseActuation();
	}

	public void Jump() {
		Vector2 velocity = _playerBody.Velocity;
		velocity.Y = -JumpVelocity;

		_playerBody.Velocity = velocity;
	}

	public void JumpFallTransition() {
		Vector2 velocity = _playerBody.Velocity;
		velocity.Y /= 2.75f;
		_Jumping = false;

		_playerBody.Velocity = velocity;
		// GD.Print("Jump Released, cut vel " + velocity.Y);
	}

	public void Fall() {
		Vector2 velocity = _playerBody.Velocity;
		velocity.Y += gravity;

		_playerBody.Velocity = velocity;
	}
}
