using Godot;
using System;

public partial class PlayerStateMachine : StateMachine
{
	private IInputManager _inputManager;
	public PlayerMovementController MovementController { get; private set; }
	public PlayerAttackController AttackController { get; private set; }
	
	[Export] public NodePath PlayerIdleStatePath;
	public PlayerIdleState IdleState { get; private set; }
	[Export] public NodePath PlayerRunStatePath;
	public PlayerRunState RunState { get; private set; }
	[Export] public NodePath PlayerJumpStatePath;
	public PlayerJumpState JumpState { get; private set; }
	[Export] public NodePath PlayerFallStatePath;
	public PlayerFallState FallState { get; private set; }
	[Export] public NodePath PlayerAttackStatePath;
	public PlayerAttackState AttackState { get; private set; }
	[Export] public NodePath PlayerDashStatePath;
	public PlayerDashState DashState { get; private set; }
	
	public void Initialize(Player player) {
		MovementController = player.MovementController;
		AttackController = player.AttackController;
	}
	
	public override void _Ready() {
		_inputManager = GetNode<IInputManager>("/root/InputManager");

		IdleState = GetNode<PlayerIdleState>(PlayerIdleStatePath);
		IdleState.Initialize(this);
		RunState = GetNode<PlayerRunState>(PlayerRunStatePath);
		RunState.Initialize(this);
		JumpState = GetNode<PlayerJumpState>(PlayerJumpStatePath);
		JumpState.Initialize(this);
		FallState = GetNode<PlayerFallState>(PlayerFallStatePath);
		FallState.Initialize(this);
		AttackState = GetNode<PlayerAttackState>(PlayerAttackStatePath);
		AttackState.Initialize(this);
		DashState = GetNode<PlayerDashState>(PlayerDashStatePath);
		DashState.Initialize(this);

		EnterDefaultState();
	}
	
	public void PlayStateAnimation() {
		
	}
	
	public bool EnterDefaultState() {
		var inputDir = MovementController.InputVector;
		
		ChangeState(inputDir == Vector2.Zero ? IdleState : RunState);

		return true;
	}
}
