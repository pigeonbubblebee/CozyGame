using Godot;
using System;

public partial class PlayerInteractionController : Area2D
{
	public bool CanInteract = true;
	public bool DesiredInteract => _CheckDesiredInteract() && CanInteract  && !_uiManager.InventoryOpen;
	private IInputManager _inputManager;

	private Player _player;

	public IInteractable ObjectCurrentlyInteracting;

	private UIManager _uiManager;
	
	public void Initialize(Player player) {
		_player = player;
	}

	public override void _Ready()
	{
		_uiManager = GetNode<UIManager>("/root/UIManager");
		_inputManager = GetNode<IInputManager>("/root/InputManager");

		this.CollisionMask = (uint) PhysicsLayers.InteractableLayer;
		this.CollisionLayer = (uint) PhysicsLayers.UntouchableLayer;
		AreaEntered += _OnItemOverlap;
		AreaExited += _OnItemExit;
	}

	private void _OnItemOverlap(Node2D hit) {
		if(hit is IInteractable) {
			if(!((IInteractable) hit).GetCondition(this._player))
				return;

			if(ObjectCurrentlyInteracting != null) {
				ObjectCurrentlyInteracting.OnExit(_player);
			}
			ObjectCurrentlyInteracting = (IInteractable) hit;
			
			ObjectCurrentlyInteracting.OnEnter(_player);
		}
	}

	private void _OnItemExit(Node2D hit) {
		if(hit is IInteractable) {
			if(ObjectCurrentlyInteracting != null) {
				ObjectCurrentlyInteracting.OnExit(_player);
			}
			ObjectCurrentlyInteracting = null;
		}
	}

	public override void _Process(double delta) {
		if(DesiredInteract) { // TODO: Add Press + Hold Mode
			_Interact();
		}
	}

	private bool _CheckDesiredInteract() {
		return _inputManager.GetInteractActuation();
	}
	
	private void _Interact() {
		if(ObjectCurrentlyInteracting != null) {
			if(ObjectCurrentlyInteracting.GetCondition(_player))
				ObjectCurrentlyInteracting.OnInteract(_player);
		}
	}
}
