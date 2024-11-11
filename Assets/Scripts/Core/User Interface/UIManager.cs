using Godot;
using System;

public partial class UIManager : Node2D
{
	[Export] private NodePath _inventoryPath;
	private InventoryUI _inventory;
	[Export] private NodePath _hudPath;
	private HUD _HUD;
	
	private IInputManager _inputManager;
	
	private Player _currentScenePlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_inputManager = GetNode<IInputManager>("/root/InputManager");
		_inventory = GetNode<InventoryUI>(_inventoryPath);
		_HUD = GetNode<HUD>(_hudPath);

		_inventory.Visible = false;
		_HUD.Visible = true;
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
}
