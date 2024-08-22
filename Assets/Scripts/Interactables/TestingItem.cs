using Godot;
using System;

public partial class TestingItem : Area2D, IInteractable
{
	public override void _Ready() { // TODO: Move to manager item?
		CollisionLayer = (uint) PhysicsLayers.InteractableLayer;
	}
	public void OnInteract(Player player) {
		GD.Print("Interacted!");
	}

	public void OnInteractHold(Player player) {}
}
