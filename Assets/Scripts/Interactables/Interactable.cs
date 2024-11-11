using Godot;
using System;

public abstract partial class Interactable : Node, IInteractable
{
	[Export] private NodePath _interactAreaPath;
	private Area2D _interactArea;

	public override void _Ready()
	{
		_interactArea = GetNode<Area2D>(_interactAreaPath);

		_interactArea.AreaEntered += _OnPlayerOverlap;
		_interactArea.AreaExited += _OnPlayerExit;
		_interactArea.CollisionLayer = (uint) PhysicsLayers.InteractableLayer;
		// _interactArea.CollisionLayer = (uint) PhysicsLayers.UntouchableLayer;
	}

	private void _OnPlayerOverlap(Node2D hit) {
		if(hit is PlayerInteractionController) {
			((PlayerInteractionController) hit).ObjectCurrentlyInteracting = this;
		}
	}

	private void _OnPlayerExit(Node2D hit) {
		if(hit is PlayerInteractionController) {
			((PlayerInteractionController) hit).ObjectCurrentlyInteracting = null;
		}
	}

	public virtual void OnInteract(Player player) {}
	public virtual void OnInteractHold(Player player) {}
}
