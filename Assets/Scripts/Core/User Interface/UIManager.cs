using Godot;
using System;

public partial class UIManager : Node2D
{
	[Export] private NodePath _inventoryPath;
	private InventoryUI _inventory;
	[Export] private NodePath _dialoguePath;
	private Dialogue _dialogue;
	[Export] private NodePath _hudPath;
	private HUD _HUD;
	[Export] private NodePath _bossBarsPath;
	private BossBarsUI _bossBars;
	[Export] private NodePath _merchantPath;
	private MerchantUI _merchant;
	
	[Export] private NodePath _loadingScreenPath;
	private Control _loadingScreen;
	
	private IInputManager _inputManager;
	
	private Player _currentScenePlayer;

	public bool InventoryOpen => _inventory.Visible;
	public bool DialogueOpen => _dialogue.Visible;
	public bool MerchantOpen => _merchant.Visible;

	public Vector2 OriginalCamZoom { get; private set; }
	public float OriginalOffset { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_inputManager = GetNode<IInputManager>("/root/InputManager");
		_inventory = GetNode<InventoryUI>(_inventoryPath);
		_HUD = GetNode<HUD>(_hudPath);
		_bossBars = GetNode<BossBarsUI>(_bossBarsPath);
		_loadingScreen = GetNode<Control>(_loadingScreenPath);
		_dialogue = GetNode<Dialogue>(_dialoguePath);
		_merchant = GetNode<MerchantUI>(_merchantPath);
		_loadingScreen.Visible = false;
		_merchant.Visible = false;
		
		ResetUI();
	}
	
	public void ResetUI() {
		_inventory.Visible = false;
		_HUD.Visible = true;
		
		_bossBars.Visible = false;
		_bossBars.Boss = null;
	}

	public void UpdateStatus() {
		_inventory.UpdateStatusScreen();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(_CheckDesiredInventory()) {
			if(!DialogueOpen && !MerchantOpen) {
				_inventory.Visible = !_inventory.Visible;
				_HUD.Visible = !_inventory.Visible;
				
				if(!_inventory.Visible) {
					_inventory.CloseInventory();
				} else {
					_inventory.OpenInventory();
				}
			}
			if(MerchantOpen) {
				_merchant.Visible = false;
			}
		}
	}

	public void LoadDialogue(DialogueData d) {
		_inventory.Visible = false;
		_inventory.CloseInventory();
		_merchant.Visible = false;

		OriginalCamZoom = new Vector2(_currentScenePlayer.Camera.Zoom.X, _currentScenePlayer.Camera.Zoom.Y);
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(_currentScenePlayer.Camera, "zoom", OriginalCamZoom * 1.2f, 0.075f);
		OriginalOffset = _currentScenePlayer.Camera._Offset;
		Tween tween2 = GetTree().CreateTween();
		tween2.TweenProperty(_currentScenePlayer.Camera, "_Offset", 0f, 0.075f);
		
		_dialogue.Visible = true;
		_dialogue.LoadDialogue(d);
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

	public void LoadMerchant(MerchantInventory merchantInventory) {
		_merchant.Visible = true;
		_merchant.LoadMerchant(merchantInventory);
	}

	public void DisableMerchant() {
		_merchant.Visible = false;
	}
}
