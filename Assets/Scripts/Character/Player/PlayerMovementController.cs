using Godot;
using System;
using System.Threading.Tasks;

// Handles Run/Jump, will add Dash and stuff

public partial class PlayerMovementController : Node
{
	private int _airDashesLeft = 1;
	[Export] private NodePath _dashBufferPath;
	private Timer _dashBuffer;

	// --------- Jump Timer Variables ---------
	[Export] private NodePath _coyoteTimePath;
	private Timer _coyoteTime;
	[Export] private NodePath _jumpBufferPath;
	private Timer _jumpBuffer;

	// --------- Jump Variables ---------
	private int _currentJump = 0;
	public bool DesiredJump => _CheckDesiredJump();
	private bool _Jumping = false;
	public bool CanJump => _currentJump < _playerStats.MaxJumps;
	public bool DesiredFall => _CheckDesiredFall();
	public bool Grounded => _playerBody.IsOnFloor();

	// --------- Velocity Variables ---------
	public Vector2 Velocity => _playerBody.Velocity;
	public Vector2 InputVector => _GetInputVector();

	// --------- Dash Variables ---------
	public bool DesiredDash => _CheckDesiredDash() && CanDash;
	public event Action FinishDashEvent;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public readonly float GRAVITY = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	private Player _playerBody;
	private PlayerStats _playerStats => _playerBody.CurrentPlayerStats;
	
	private IInputManager _inputManager;

	public int Direction = 1;
	public bool CanSwitchDirections = true;
	public bool CanDash { get; private set; }

	public override void _Ready()
	{
		CanDash = true;

		_inputManager = GetNode<IInputManager>("/root/InputManager");

		_coyoteTime = GetNode<Timer>(_coyoteTimePath);
		_jumpBuffer = GetNode<Timer>(_jumpBufferPath);
		_dashBuffer = GetNode<Timer>(_dashBufferPath);

		_coyoteTime.WaitTime = _playerStats.CoyoteTime;
		_jumpBuffer.WaitTime = _playerStats.JumpBuffer;
		_dashBuffer.WaitTime = _playerStats.DashBuffer;
	}

	public void Initialize(Player body) {
		_playerBody = body;
		// _playerStats = body.PlayerStatsResource;
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
		if(!CanSwitchDirections) {
			return;
		}
		/*
		if(Direction == 1 && Velocity.X < 0) {
			Direction = -1;
		} else if(Direction == -1 && Velocity.X > 0) {
			Direction = 1;
		}
		*/
		if(Direction == 1 && InputVector.X < 0) {
			Direction = -1;
		} else if(Direction == -1 && InputVector.X > 0) {
			Direction = 1;
		}
	}

	private Vector2 _GetInputVector() {
		return _inputManager.GetMovementDirection();
	}

	public void Accelerate(Vector2 direction) {
		Vector2 velocity = _playerBody.Velocity.MoveToward(_playerStats.Speed * direction, _playerStats.Acceleration);
		velocity.Y = Velocity.Y;
		_playerBody.Velocity = velocity;
	}
	
	public void Recoil(float maxSpeed, float magnitude) {
		Vector2 velocity = _playerBody.Velocity.MoveToward(maxSpeed * new Vector2(Direction == 1 ? -1 : 1, 0), magnitude);
		velocity.Y = Velocity.Y;
		_playerBody.Velocity = velocity;
	}

	public void DashAccelerate() {
		// Vector2 velocity = _playerBody.Velocity.MoveToward(DashAcceleration * direction, DashAcceleration);
		// velocity.Y = Velocity.Y;
		_playerBody.Velocity = new Vector2(_playerStats.DashSpeed * Direction, 0f);
	}

	public void SwitchDirection(Vector2 direction) {
		Vector2 velocity = _playerBody.Velocity.MoveToward(_playerStats.Speed * direction, _playerStats.DirectionSwitchSpeed);
		velocity.Y = Velocity.Y;
		_playerBody.Velocity = velocity;
	}

	public void AddFriction() {
		Vector2 zero = Vector2.Zero;
		zero.Y = Velocity.Y;
		_playerBody.Velocity = _playerBody.Velocity.MoveToward(zero, _playerStats.Friction);
	}

	public void AddDashFriction() {
		Vector2 zero = Vector2.Zero;
		zero.Y = Velocity.Y;
		_playerBody.Velocity = _playerBody.Velocity.MoveToward(zero, _playerStats.DashFriction);
	}

	private void _PlayerMovement() {
		_playerBody.MoveAndSlide();
	}
	
	private bool _CheckDesiredJump() {
		if(_inputManager.GetJumpActuation()) {
			return true;
		}

		return false;
	}

	private bool _CheckDesiredFall() {
		return _inputManager.GetJumpReleaseActuation();
	}

	public void Jump() {
		if(!CanJump) {
			GD.Print(_currentJump);
			return;
		}
		
		Vector2 velocity = _playerBody.Velocity;
		velocity.Y = -_playerStats.JumpVelocity;

		_playerBody.Velocity = velocity;
		
		_currentJump ++;
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
			_currentJump = 0;
		}
	}

	public void Fall() {
		Vector2 velocity = _playerBody.Velocity;
		velocity.Y += GRAVITY;

		_playerBody.Velocity = velocity;
	}

	public void JumpFall() {
		Vector2 velocity = _playerBody.Velocity;
		velocity.Y += _playerStats.JumpingGravity;

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
		GetTree().CreateTimer(_playerStats.DashTime).Timeout += FinishDashEvent;
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
			_airDashesLeft = _playerStats.MaxAirDash;
		}
	}

	public void StartDashCooldown() {
		CanDash = false;
		GetTree().CreateTimer(_playerStats.DashCooldown).Timeout += _FinishDashCooldown;
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
