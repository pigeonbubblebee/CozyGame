using Godot;
using System;

public partial class UIManager : Node2D
{
	[Export] public NodePath InventoryPath;
	private CanvasItem _inventory;
	private IInputManager _inputManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_inputManager = GetNode<IInputManager>("/root/InputManager");
		_inventory = GetNode<CanvasItem>(InventoryPath);

		_inventory.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(_CheckDesiredInventory()) {
			_inventory.Visible = !_inventory.Visible;
		}
	}

	private bool _CheckDesiredInventory() {
		return _inputManager.GetInventoryActuation();
	}
}
