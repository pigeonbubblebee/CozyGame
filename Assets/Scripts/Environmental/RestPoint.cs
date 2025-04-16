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
		GD.Print("rested");
		GetNode<MainHandler>("/root/MainHandler").SetRespawnID(this.Name);
		GetNode<MainHandler>("/root/MainHandler").RespawnPlayer();
	}

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);

		GetNode<SaveLoader>("/root/SaveLoader").AddMapMarker(GetNode<MainHandler>("/root/MainHandler").GetMarkerID(this.Name));
    }

	public override bool GetCondition(Player player) {
		return !(GetNode<Player>("/root/Player").StateMachine.CurrentState is PlayerRestState);
	}

	// public void OnInteractHold(Player player) {}
}
