using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	[Export] private NodePath _stateMachinePath;
	public PlayerStateMachine StateMachine  { get; private set; }
	[Export] private NodePath _movementControllerPath;
	public PlayerMovementController MovementController  { get; private set; }
	[Export] private NodePath _attackControllerPath;
	public PlayerAttackController AttackController  { get; private set; }
	//[Export] private NodePath _playerSpritePath;
	public PlayerInteractionController InteractionController  { get; private set; }
	[Export] private NodePath _interactionControllerPath;
	//public Sprite2D PlayerSprite  { get; private set; }
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
	[Export] private NodePath _curseControllerPath;
	public PlayerCurseController CurseController  { get; private set; }
	[Export] private NodePath _summonControllerPath;
	public PlayerSummonController SummonController  { get; private set; }
	[Export] private NodePath _cameraPath;
	
	public PlayerCamera Camera  { get; private set; }
	public PlayerBuffs BaseBuffs { get; private set; } = new PlayerBuffs();
	public PlayerBuffs CurrentBuffs { get; private set; }
	[Export] private NodePath _hitSFXPath;
	private AudioStreamPlayer2D _hitSFX;
	
	public event Action ExitGrabEvent;
	public event Action<int, Player> PlayerHealEvent;
	
	private GameManager _gameManager;

	private UIManager _uiManager;

	private Dictionary<string, float> _timers = new();
	public override void _EnterTree()
	{
		base._EnterTree();
		CurrentBuffs = new PlayerBuffs(BaseBuffs);
		MovementController = GetNode<PlayerMovementController>(_movementControllerPath);
		AttackController = GetNode<PlayerAttackController>(_attackControllerPath);
		StateMachine = GetNode<PlayerStateMachine>(_stateMachinePath);
		//PlayerSprite = GetNode<Sprite2D>(_playerSpritePath);
		InteractionController = GetNode<PlayerInteractionController>(_interactionControllerPath);
		InventoryManager = GetNode<PlayerInventoryManager>(_inventoryManagerPath);
		PlayerHealth = GetNode<HealthSystem>(_healthSystemPath);
		HealController = GetNode<PlayerHealController>(_healControllerPath);
		SpellController = GetNode<PlayerSpellController>(_spellControllerPath);
		AnimationController = GetNode<PlayerAnimationController>(_animationControllerPath);
		DeflectController = GetNode<PlayerDeflectController>(_deflectControllerPath);
		CurseController = GetNode<PlayerCurseController>(_curseControllerPath);
		SummonController = GetNode<PlayerSummonController>(_summonControllerPath);
		Camera = GetNode<PlayerCamera>(_cameraPath);

		PlayerHealth.MaxHealthPoints = PlayerStatsResource.MaxHealth + CurrentBuffs.Vitality;
		CurseController.MaxCurse = PlayerStatsResource.MaxCurse + CurrentBuffs.Harmony;

		PlayerHealth.HealEvent += _CallPlayerHealEvent;

		// PlayerHealth.DeathEvent += Quit;
		PlayerHealth.Invincible = false;

		_uiManager = GetNode<UIManager>("/root/UIManager");

		StateMachine.Initialize(this); // PLEASE ADD A PLAYER COMPONENT CLSS OR SMTH
		MovementController.Initialize(this);
		AttackController.Initialize(this);
		InteractionController.Initialize(this);
		HealController.Initialize(this);
		SpellController.Initialize(this);
		AnimationController.Initialize(this);
		DeflectController.Initialize(this);
		CurseController.Initialize(this);
		InventoryManager.Initialize(this);
		SummonController.Initialize(this);

		// 		PlayerSprite.ZIndex = RenderingLayers.PlayerLayer;

		_hitSFX = GetNode<AudioStreamPlayer2D>(_hitSFXPath);


	}
	
	public void Respawn() {
		
		//GD.Print("respawn");
		// GD.Print(GetNode<SaveLoader>("/root/SaveLoader").CurrentSaveFile["PlayerHealth"]);
		PlayerHealth.SetHealth(Convert.ToInt32(GetNode<SaveLoader>("/root/SaveLoader").CurrentSaveFile["PlayerHealth"]));
		HealController.SetHeals(Convert.ToInt32(GetNode<SaveLoader>("/root/SaveLoader").CurrentSaveFile["PlayerHeals"]));
		CurseController.SetCurse(Convert.ToInt32(GetNode<SaveLoader>("/root/SaveLoader").CurrentSaveFile["PlayerMystic"]));
		// PlayerHealth.ResetHealth();
		GD.Print("spirit pickup " + GetNode<SaveLoader>("/root/SaveLoader").CurrentSaveFile["SpiritPickup"].ToString());
		InventoryManager.HasPickedUpEquip = GetNode<SaveLoader>("/root/SaveLoader").CurrentSaveFile["SpiritPickup"].ToString().Equals("True");
		//SpellController.ResetMana();
		//HealController.ResetHeals();
		int[] attributes = JsonConvert.DeserializeObject<int[]>(GetNode<SaveLoader>("/root/SaveLoader").CurrentSaveFile["Attributes"].ToString());
		BaseBuffs.Strength = attributes[0];
		BaseBuffs.Dexterity = attributes[1];
		BaseBuffs.Vitality = attributes[2];
		BaseBuffs.Focus = attributes[3];
		BaseBuffs.Harmony = attributes[4];

		SummonController.ClearSummons();
		
		//PostureController.ResetPosture();
		

		GetNode<UIManager>("/root/UIManager").SetCurrentPlayerInstance(this);
		try {
		
			// GD.Print("count: " + JsonConvert.DeserializeObject<string[]>(GetNode<SaveLoader>("/root/SaveLoader").CurrentSaveFile["Inventory"].ToString()).Count);
			InventoryManager.ReadInventory(JsonConvert.DeserializeObject<string[]>(GetNode<SaveLoader>("/root/SaveLoader").CurrentSaveFile["Inventory"].ToString()),
				JsonConvert.DeserializeObject<int[]>(GetNode<SaveLoader>("/root/SaveLoader").CurrentSaveFile["InventoryStacks"].ToString()));
			InventoryManager.ReadEquipped(JsonConvert.DeserializeObject<string[]>(GetNode<SaveLoader>("/root/SaveLoader").CurrentSaveFile["Equipped"].ToString()));
		} catch {}

		UpdateBuffs();

	}

	private int _prevVit = 0;

	public void CreateCustomTimer(string id, float duration)
    {
        _timers[id] = duration;
    }
	public void CancelCustomTimer(string id)
    {
        _timers.Remove(id);
    }
	public bool GetCustomTimerActive(string id)
	{
		if (!_timers.ContainsKey(id))
		{
			return false;
		}
		return _timers[id] > 0f;
	}

	public void UpdateTimers(double delta)
	{
		var finished = new List<string>();

		foreach (var keyval in _timers)
		{
			_timers[keyval.Key] -= (float)delta;
			if (_timers[keyval.Key] <= 0f)
				finished.Add(keyval.Key);
		}

		foreach (var id in finished)
		{
			_timers.Remove(id);
		}
	}

	public void UpdateBuffs()
	{
		CurrentBuffs.ResetBuffs(BaseBuffs);
		int i = 0;
		foreach (Equippable e in InventoryManager.EquippedItems)
		{
			if (e != null)
				CurrentBuffs = e.ApplyStatic(CurrentBuffs, this, i);
			i++;
		}

		// if(Input.IsActionPressed("debug")) { // TODO: Delete this before export
		// 	CurrentBuffs.Strength += 10000;
		// }

		int diff = 0;

		if (_prevVit < CurrentBuffs.Vitality)
			diff = CurrentBuffs.Vitality - _prevVit;
		_prevVit = CurrentBuffs.Vitality;

		PlayerHealth.MaxHealthPoints = PlayerStatsResource.MaxHealth + (int)Math.Ceiling(2.5 * (double)CurrentBuffs.Vitality);
		// PlayerHealth.AddHealth((int)Math.Ceiling(2.5*(double)diff), false);
		PlayerHealth.AddHealth(0, false);

		CurseController.MaxCurse = PlayerStatsResource.MaxCurse + (int)Math.Ceiling(5 * (double)CurrentBuffs.Harmony);
		CurseController.AddCurse(0);

		_uiManager.UpdateStatus();
		// CurseController.AddCurse((int)Math.Ceiling(5*(double)diff));


		// GD.Print("strength: "+ CurrentBuffs.Strength);
	}

	public int[] SerializeCurrentBuffs() {
		return new int[] {BaseBuffs.Strength, BaseBuffs.Dexterity, BaseBuffs.Vitality,BaseBuffs.Focus, BaseBuffs.Harmony};
	}
	
	private void Quit() { // Temp
		GetTree().Quit();
	}

    public override void _Process(double delta)
    {
		UpdateBuffs();
		UpdateTimers(delta);

		if (Input.IsActionJustPressed("debug"))
		{ // TODO: Delete this before export
			this.GlobalPosition = GetGlobalMousePosition();
		}
		if(Input.IsActionPressed("debug")) {
			InventoryManager.AddItemToInventory("acorn");
			InventoryManager.AddItemToInventory("forest_essence");
		}

        base._Process(delta);
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
			if(e.Unstoppable) {
				TakeTrueDamage(e);
				return;
			}
			// CurseController.AddCurse(CurrentPlayerStats.CurseBuildRate);
			DeflectController.Block(e, enemy);
		} else {
			TakeTrueDamage(e);
		}
	}

	public void TakeObstacleDamage(EnemyAttackData e, Node2D source, float knockBackMultiplier) {
		Camera.Shake(0.2f, 2000f);
		_gameManager.FreezeFrame(0.02f, 0.08f);
		// GD.Print(e.Enemy.GlobalPosition.X > GlobalPosition.X ? -1 : 1);
		MovementController.ApplyKnockback(source.GlobalPosition.X > GlobalPosition.X ? -1 : 1, 1000*knockBackMultiplier, 500f*knockBackMultiplier, 0.1f);
		
		TakeTrueDamage(e);
	}
	
	public void TakeTrueDamage(EnemyAttackData e) {
		_hitSFX.Play();
		HealController.ConvertInternalDamage();
		PlayerHealth.TakeDamage(e.Damage);
		HealController.TakeInternalDamage(e.InternalDamage);
		// PostureController.ResetPosture();
		// PostureController.TakePostureDamage(e.PostureDamage);
	}

	public void TakeCurseDamage(int damage, int internalDamage) {
		PlayerHealth.TakeDamage(damage);
		HealController.TakeInternalDamage(internalDamage);
	}
	
	public void ExitGrab() {
		ExitGrabEvent?.Invoke();
	}

	private void _CallPlayerHealEvent(int amt)
	{
		PlayerHealEvent?.Invoke(amt, this);
	}
}
