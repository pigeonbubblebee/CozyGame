using Godot;
using System;

public partial class PlayerStateMachine : StateMachine
{
	private IInputManager _inputManager;
	public PlayerMovementController MovementController { get; private set; }
	
	[Export] public NodePath PlayerIdleStatePath;
	public PlayerIdleState IdleState { get; private set; }
	[Export] public NodePath PlayerRunStatePath;
	public PlayerRunState RunState { get; private set; }
	[Export] public NodePath PlayerJumpStatePath;
	public PlayerJumpState JumpState { get; private set; }
	[Export] public NodePath PlayerFallStatePath;
	public PlayerFallState FallState { get; private set; }
	
	public void Initialize(PlayerMovementController movementController) {
		MovementController = movementController;
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

		EnterDefaultState();
	}
	
	public void PlayStateAnimation() {
		
	}
	
	public bool EnterDefaultState() {
		var inputDir = MovementController.InputVector;
		
		ChangeState(inputDir != Vector2.Zero ? IdleState : RunState);

		return true;
	}
}
