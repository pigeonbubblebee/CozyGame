using Godot;
using System;

public partial class TestingItem : Interactable
{
	[Export] private Item _itemResource { get; set; } // TO FIX: ItemResource Always null??

	// public override void _Ready() { // TODO: Move to manager item?d
	// 	CollisionLayer = (uint) PhysicsLayers.InteractableLayer;
	// }
	public override void OnInteract(Player player) {
		GD.Print("Interacted!");
		GD.Print(_itemResource);
		player.InventoryManager.AddItemToInventory(_itemResource);
		this.QueueFree();
	}
}
