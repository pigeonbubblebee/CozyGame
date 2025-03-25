using Godot;
using System;

public partial class UIManager : Node2D
{
	[Export] private NodePath _inventoryPath;
	private InventoryUI _inventory;
	[Export] private NodePath _hudPath;
	private HUD _HUD;
	[Export] private NodePath _bossBarsPath;
	private BossBarsUI _bossBars;
	
	[Export] private NodePath _loadingScreenPath;
	private Control _loadingScreen;
	
	private IInputManager _inputManager;
	
	private Player _currentScenePlayer;

	public bool InventoryOpen => _inventory.Visible;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_inputManager = GetNode<IInputManager>("/root/InputManager");
		_inventory = GetNode<InventoryUI>(_inventoryPath);
		_HUD = GetNode<HUD>(_hudPath);
		_bossBars = GetNode<BossBarsUI>(_bossBarsPath);
		_loadingScreen = GetNode<Control>(_loadingScreenPath);
		
		_loadingScreen.Visible = false;
		
		ResetUI();
	}
	
	public void ResetUI() {
		_inventory.Visible = false;
		_HUD.Visible = true;
		
		_bossBars.Visible = false;
		_bossBars.Boss = null;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(_CheckDesiredInventory()) {
			_inventory.Visible = !_inventory.Visible;
			_HUD.Visible = !_inventory.Visible;
			
			if(!_inventory.Visible) {
				_inventory.CloseInventory();
			}
		}
	}

	private bool _CheckDesiredInventory() {
		return _inputManager.GetInventoryActuation();
	}
	
	public void SetCurrentPlayerInstance(Player p) {
		_currentScenePlayer = p;
		_HUD.SetCurrentPlayerInstance(p);
		_inventory.SetCurrentPlayerInstance(p);
	}
	
	public void EnableBossBar(Enemy e) {
		_bossBars.Visible = true;
		_bossBars.SetBoss(e);
	}
	
	public void DisableBossBar(Enemy e) {
		if(_bossBars.Boss == e) {
			_bossBars.Visible = false;
			_bossBars.Boss = null;
		}
	}
	
	public void EnableLoadingScreen() {
		_loadingScreen.Visible = true;
	}
	public void DisableLoadingScreen() {
		_loadingScreen.Visible = false;
	}
}
