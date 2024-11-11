using Godot;
using System;

public partial class TestingItem : Area2D, IInteractable
{
	[Export] private Item _itemResource { get; set; } // TO FIX: ItemResource Always null??

	public override void _Ready() { // TODO: Move to manager item?d
		CollisionLayer = (uint) PhysicsLayers.InteractableLayer;
	}
	public void OnInteract(Player player) {
		GD.Print("Interacted!");
		GD.Print(_itemResource);
		player.InventoryManager.AddItemToInventory(_itemResource);
		this.QueueFree();
	}

	public void OnInteractHold(Player player) {}
}
