using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] private NodePath _stateMachinePath;
	public PlayerStateMachine StateMachine  { get; private set; }
	[Export] private NodePath _movementControllerPath;
	public PlayerMovementController MovementController  { get; private set; }
	[Export] private NodePath _attackControllerPath;
	public PlayerAttackController AttackController  { get; private set; }
	[Export] private NodePath _playerSpritePath;
	public PlayerInteractionController InteractionController  { get; private set; }
	[Export] private NodePath _interactionControllerPath;
	public Sprite2D PlayerSprite  { get; private set; }
	[Export] private NodePath _inventoryManagerPath;
	public PlayerInventoryManager InventoryManager  { get; private set; }
	[Export] public PlayerStats PlayerStatsResource { get; private set; }
	public PlayerStats CurrentPlayerStats => PlayerStatsResource;
	[Export] private NodePath _healthSystemPath;
	public HealthSystem PlayerHealth  { get; private set; }
	[Export] private NodePath _healControllerPath;
	public PlayerHealController HealController  { get; private set; }
	[Export] private NodePath _spellControllerPath;
	public PlayerSpellController SpellController  { get; private set; }
	[Export] private NodePath _animationControllerPath;
	public PlayerAnimationController AnimationController  { get; private set; }
	[Export] private NodePath _deflectControllerPath;
	public PlayerDeflectController DeflectController  { get; private set; }
	[Export] private NodePath _postureControllerPath;
	public PlayerPostureController PostureController  { get; private set; }
	[Export] private NodePath _cameraPath;
	public PlayerCamera Camera  { get; private set; }

	public override void _EnterTree()
	{
		base._EnterTree();

		MovementController = GetNode<PlayerMovementController>(_movementControllerPath);
		AttackController = GetNode<PlayerAttackController>(_attackControllerPath);
		StateMachine = GetNode<PlayerStateMachine>(_stateMachinePath);
		PlayerSprite = GetNode<Sprite2D>(_playerSpritePath);
		InteractionController = GetNode<PlayerInteractionController>(_interactionControllerPath);
		InventoryManager = GetNode<PlayerInventoryManager>(_inventoryManagerPath);
		PlayerHealth = GetNode<HealthSystem>(_healthSystemPath);
		HealController = GetNode<PlayerHealController>(_healControllerPath);
		SpellController = GetNode<PlayerSpellController>(_spellControllerPath);
		AnimationController = GetNode<PlayerAnimationController>(_animationControllerPath);
		DeflectController = GetNode<PlayerDeflectController>(_deflectControllerPath);
		PostureController = GetNode<PlayerPostureController>(_postureControllerPath);
		Camera = GetNode<PlayerCamera>(_cameraPath);
		
		PlayerHealth.MaxHealthPoints = PlayerStatsResource.MaxHealth;
		PlayerHealth.ResetHealth();
		PlayerHealth.Invincible = false;

		StateMachine.Initialize(this); // PLEASE ADD A PLAYER COMPONENT CLSS OR SMTH
		MovementController.Initialize(this);
		AttackController.Initialize(this);
		InteractionController.Initialize(this);
		HealController.Initialize(this);
		SpellController.Initialize(this);
		AnimationController.Initialize(this);
		DeflectController.Initialize(this);
		PostureController.Initialize(this);
		
		SpellController.ResetMana();
		HealController.ResetHeals();
		
		PostureController.ResetPosture();

		PlayerSprite.ZIndex = RenderingLayers.PlayerLayer;
	}
	
	public override void _Ready() {
		GetNode<UIManager>("/root/UIManager").SetCurrentPlayerInstance(this);
	}
	
	public void TakeDamage(int damage, int postureDamage, Enemy e) {
		if(DeflectController.Blocking) {
			DeflectController.Block(damage, postureDamage, e);
		} else {
			PlayerHealth.TakeDamage(damage);
			PostureController.TakePostureDamage(postureDamage);
		}
	}
}
