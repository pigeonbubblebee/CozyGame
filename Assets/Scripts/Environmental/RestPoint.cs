using Godot;
using System;

public partial class RestPoint : Interactable
{
	// [Export] private Item _itemResource { get; set; } // TO FIX: ItemResource Always null??

	// public override void _Ready() { // TODO: Move to manager item?d
	// 	CollisionLayer = (uint) PhysicsLayers.InteractableLayer;
	// }

	// public vo
	public override void OnInteract(Player player) {
		// GD.Print("Interacted!");
		// GD.Print(_itemResource);
		// player.InventoryManager.AddItemToInventory(_itemResource);
		// this.QueueFree();

		GetNode<MainHandler>("/root/MainHandler").SetRespawnID(this.Name);
		GetNode<MainHandler>("/root/MainHandler").RespawnPlayer();
	}

	// public void OnInteractHold(Player player) {}
}
