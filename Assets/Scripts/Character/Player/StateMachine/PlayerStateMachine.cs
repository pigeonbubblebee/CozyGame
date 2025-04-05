using Godot;
using System;

public partial class PlayerStateMachine : StateMachine
{
	private IInputManager _inputManager;
	public PlayerMovementController MovementController { get; private set; }
	public PlayerAttackController AttackController { get; private set; }
	public PlayerInteractionController InteractionController { get; private set; }
	public PlayerHealController HealController { get; private set; }
	public PlayerSpellController SpellController { get; private set; }
	public PlayerAnimationController AnimationController { get; private set; }
	public PlayerDeflectController DeflectController { get; private set; }
	public PlayerCurseController CurseController { get; private set; }
	
	public Player Player;
	public PlayerStats Stats => Player.CurrentPlayerStats;
	
	[Export] private NodePath _playerIdleStatePath;
	public PlayerIdleState IdleState { get; private set; }
	[Export] private NodePath _playerRunStatePath;
	public PlayerRunState RunState { get; private set; }
	[Export] private NodePath _playerJumpStatePath;
	public PlayerJumpState JumpState { get; private set; }
	[Export] private NodePath _playerFallStatePath;
	public PlayerFallState FallState { get; private set; }
	[Export] private NodePath _playerAttackStatePath;
	public PlayerAttackState AttackState { get; private set; }
	[Export] private NodePath _playerDashStatePath;
	public PlayerDashState DashState { get; private set; }
	[Export] private NodePath _playerHealStatePath;
	public PlayerHealState HealState { get; private set; }
	[Export] private NodePath _playerSpellStatePath;
	public PlayerSpellState SpellState { get; private set; }
	[Export] private NodePath _playerBlockStatePath;
	public PlayerBlockState BlockState { get; private set; }
	[Export] private NodePath _playerStaggerStatePath;
	public PlayerStaggerState StaggerState { get; private set; }
	[Export] private NodePath _playerGrabStatePath;
	public PlayerGrabState GrabState { get; private set; }
	
	public void Initialize(Player player) {
		Player = player;
		MovementController = player.MovementController;
		AttackController = player.AttackController;
		InteractionController = player.InteractionController;
		HealController = player.HealController;
		SpellController = player.SpellController;
		AnimationController = player.AnimationController;
		DeflectController = player.DeflectController;
		CurseController = player.CurseController;
		// Stats = player.PlayerStatsResource;
	}
	
	public override void _Ready() {
		_inputManager = GetNode<IInputManager>("/root/InputManager");

		IdleState = GetNode<PlayerIdleState>(_playerIdleStatePath);
		IdleState.Initialize(this);
		RunState = GetNode<PlayerRunState>(_playerRunStatePath);
		RunState.Initialize(this);
		JumpState = GetNode<PlayerJumpState>(_playerJumpStatePath);
		JumpState.Initialize(this);
		FallState = GetNode<PlayerFallState>(_playerFallStatePath);
		FallState.Initialize(this);
		AttackState = GetNode<PlayerAttackState>(_playerAttackStatePath);
		AttackState.Initialize(this);
		DashState = GetNode<PlayerDashState>(_playerDashStatePath);
		DashState.Initialize(this);
		HealState = GetNode<PlayerHealState>(_playerHealStatePath);
		HealState.Initialize(this);
		SpellState = GetNode<PlayerSpellState>(_playerSpellStatePath);
		SpellState.Initialize(this);
		BlockState = GetNode<PlayerBlockState>(_playerBlockStatePath);
		BlockState.Initialize(this);
		StaggerState = GetNode<PlayerStaggerState>(_playerStaggerStatePath);
		StaggerState.Initialize(this);
		GrabState = GetNode<PlayerGrabState>(_playerGrabStatePath);
		GrabState.Initialize(this);

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
