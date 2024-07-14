using Godot;
using System;
using System.Threading.Tasks;

// Handles Run/Jump, will add Dash and stuff

public partial class PlayerMovementController : Node
{
	// --------- Settings Vars, Move to PlayerData ---------
	[Export] public float Speed = 500.0f;
	[Export] public float Acceleration = 50.0f;
	[Export] public float DirectionSwitchSpeed = 100.0f;
	[Export] public float Friction = 70.0f;
	
	[Export] public float JumpVelocity = 4500.0f;
	[Export] public int MaxJumps = 2;
	[Export] public float jumpingGravity = 200f;

	[Export] public float DashSpeed = 3000.0f;
	// [Export] public float DashAcceleration = 1000.0f;
	[Export] public float DashFriction = 1000.0f;
	[Export] public float DashTime = 0.1f;
	[Export] public bool CanAirDash = true;
	[Export] public int MaxAirDash = 1;
	private int _airDashesLeft = 1;
	[Export] public float DashCooldown;
	[Export] public NodePath DashBufferPath;
	private Timer _dashBuffer;

	// --------- Jump Timer Variables ---------
	[Export] public NodePath CoyoteTimePath;
	private Timer _coyoteTime;
	[Export] public NodePath JumpBufferPath;
	private Timer _jumpBuffer;

	// --------- Jump Variables ---------
	// [Export] public float JumpReleaseVelocity = 100f;
	private int _CurrentJump = 1;
	public bool DesiredJump => _CheckDesiredJump();
	private bool _Jumping = false;
	public bool DesiredFall => _CheckDesiredFall();
	public bool Grounded => _playerBody.IsOnFloor();

	// --------- Velocity Variables ---------
	public Vector2 Velocity => _playerBody.Velocity;
	public Vector2 InputVector => _GetInputVector();

	// --------- Dash Variables ---------
	public bool DesiredDash => _CheckDesiredDash() && CanDash;
	public event Action FinishDashEvent;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	private CharacterBody2D _playerBody;
	
	private IInputManager _inputManager;

	public int Direction = 1;
	public bool CanDash { get; private set; }

	public override void _Ready()
	{
		CanDash = true;

		_inputManager = GetNode<IInputManager>("/root/InputManager");

		_coyoteTime = GetNode<Timer>(CoyoteTimePath);
		_jumpBuffer = GetNode<Timer>(JumpBufferPath);
		_dashBuffer = GetNode<Timer>(DashBufferPath);
	}

	public void Initialize(CharacterBody2D body) {
		_playerBody = body;
	}

	public override void _Process(double delta) {
		if(_CheckDesiredDash() && !CanDash) {
			StartDashBuffer();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		_PlayerMovement();
		_DirectionCheck();
		_ResetJump();
		_ResetDash();
	}

	private void _DirectionCheck() {
		if(Direction == 1 && Velocity.X < 0) {
			Direction = -1;
		} else if(Direction == -1 && Velocity.X > 0) {
			Direction = 1;
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

	public void DashAccelerate() {
		// Vector2 velocity = _playerBody.Velocity.MoveToward(DashAcceleration * direction, DashAcceleration);
		// velocity.Y = Velocity.Y;
		_playerBody.Velocity = new Vector2(DashSpeed * Direction, 0f);
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

	public void AddDashFriction() {
		Vector2 zero = Vector2.Zero;
		zero.Y = Velocity.Y;
		_playerBody.Velocity = _playerBody.Velocity.MoveToward(zero, DashFriction);
	}

	private void _PlayerMovement() {
		_playerBody.MoveAndSlide();
	}
	
	private bool _CheckDesiredJump() {
		if(_inputManager.GetJumpActuation()) {
			// if(_CurrentJump < MaxJumps) { // Add Coyote & Buffer
			// 	_CurrentJump ++;
			// 	_Jumping = true;
			// }

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
		velocity.Y /= 2f;
		_Jumping = false;

		_playerBody.Velocity = velocity;
		// GD.Print("Jump Released, cut vel " + velocity.Y);
	}

	private void _ResetJump() {
		if(Grounded) {
			_CurrentJump = 1;
		}
	}

	public void Fall() {
		Vector2 velocity = _playerBody.Velocity;
		velocity.Y += gravity;

		_playerBody.Velocity = velocity;
	}

	public void JumpFall() {
		Vector2 velocity = _playerBody.Velocity;
		velocity.Y += jumpingGravity;

		_playerBody.Velocity = velocity;
	}

	public void StartCoyoteTimer() {
		_coyoteTime.Start();
	}

	public bool GetCoyoteStop() {
		return _coyoteTime.IsStopped();
	}

	public void StartJumpBuffer() {
		_jumpBuffer.Start();
	}

	public bool GetJumpBufferStop() {
		return _jumpBuffer.IsStopped();
	}

	private bool _CheckDesiredDash() {
		return _inputManager.GetDashActuation();
	}

	public void Dash() {
		GetTree().CreateTimer(DashTime).Timeout += FinishDashEvent;
	}

	public bool UseAirDash() {
		if(_airDashesLeft > 0) {
			_airDashesLeft -= 1;
			return true;
		}

		return false;
	}

	private void _ResetDash() {
		if(Grounded) {
			_airDashesLeft = MaxAirDash;
		}
	}

	public void StartDashCooldown() {
		CanDash = false;
		GetTree().CreateTimer(DashCooldown).Timeout += _FinishDashCooldown;
	}

	private void _FinishDashCooldown() {
		CanDash = true;
	}

	public void StartDashBuffer() {
		_dashBuffer.Start();
	}

	public bool GetDashBufferStop() {
		return _dashBuffer.IsStopped();
	}
}
