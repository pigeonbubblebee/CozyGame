using Godot;
using System;

public partial class TestingItem : Area2D, IInteractable
{
	[Export] public Item ItemResource { get; set; } // TO FIX: ItemResource Always null??
	[Export] public PlayerStats p { get; set; }
	public override void _Ready() { // TODO: Move to manager item?
		GD.Print(ItemResource);
		CollisionLayer = (uint) PhysicsLayers.InteractableLayer;
	}
	public void OnInteract(Player player) {
		GD.Print("Interacted!");
		// GD.Print(ItemResource);
		// player.InventoryManager.AddItemToInventory(ItemResource);
		// this.QueueFree();
	}

	public void OnInteractHold(Player player) {}
}
