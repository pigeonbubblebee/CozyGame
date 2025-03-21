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
	
	[Export] private NodePath _hitSFXPath;
	private AudioStreamPlayer2D _hitSFX;
	
	public event Action ExitGrabEvent;
	
	private GameManager _gameManager;

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
		
		// PlayerHealth.DeathEvent += Quit;
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

		PlayerSprite.ZIndex = RenderingLayers.PlayerLayer;
		
		_hitSFX = GetNode<AudioStreamPlayer2D>(_hitSFXPath);
	}
	
	public void Respawn() {
		
		GD.Print("respawn");
		// GD.Print(GetNode<SaveLoader>("/root/SaveLoader").CurrentSaveFile["PlayerHealth"]);
		PlayerHealth.SetHealth(Convert.ToInt32(GetNode<SaveLoader>("/root/SaveLoader").CurrentSaveFile["PlayerHealth"]));
		// PlayerHealth.ResetHealth();
		SpellController.ResetMana();
		HealController.ResetHeals();
		
		PostureController.ResetPosture();

		GetNode<UIManager>("/root/UIManager").SetCurrentPlayerInstance(this);
	}
	
	private void Quit() { // Temp
		GetTree().Quit();
	}
	
	public override void _Ready() {
		//GetNode<UIManager>("/root/UIManager").SetCurrentPlayerInstance(this);
		_gameManager = GetNode<GameManager>("/root/GameManager");
	}
	
	public void TakeDamage(EnemyAttackData e, Enemy enemy) {
		Camera.Shake(0.2f, 2000f);
		_gameManager.FreezeFrame(0.02f, 0.08f);
		// GD.Print(e.Enemy.GlobalPosition.X > GlobalPosition.X ? -1 : 1);
		MovementController.ApplyKnockback(enemy.GlobalPosition.X > GlobalPosition.X ? -1 : 1, 1000, 500f, 0.1f);
		
		if(DeflectController.Blocking) {
			if(e.Unstoppable && (e.Type == EnemyAttackData.AttackType.Sweep || e.Type == EnemyAttackData.AttackType.Cleave)) {
				TakeTrueDamage(e, enemy);
				return;
			}
			PostureController.TakePostureDamage(CurrentPlayerStats.CurseBuildRate);
			DeflectController.Block(e, enemy);
		} else {
			TakeTrueDamage(e, enemy);
		}
	}
	
	public void TakeTrueDamage(EnemyAttackData e, Enemy enemy) {
		_hitSFX.Play();
		HealController.ConvertInternalDamage();
		PlayerHealth.TakeDamage(e.Damage);
		HealController.TakeInternalDamage(e.InternalDamage);
		PostureController.ResetPosture();
		// PostureController.TakePostureDamage(e.PostureDamage);
	}

	public void TakeCurseDamage(int damage, int internalDamage) {
		PlayerHealth.TakeDamage(damage);
		HealController.TakeInternalDamage(internalDamage);
	}
	
	public void ExitGrab() {
		ExitGrabEvent?.Invoke();
	}
}
