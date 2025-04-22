using Godot;
using System;

public partial class TestingItem : Interactable
{
	[Export] private Item _itemResource { get; set; } // TO FIX: ItemResource Always null??
	[Export] private NodePath _itemIconPath;
	private Sprite2D _itemIcon;

    // public override void _Ready() { // TODO: Move to manager item?d
    // 	CollisionLayer = (uint) PhysicsLayers.InteractableLayer;
    // }
    public override void _Ready()
    {
        base._Ready();
		_itemIcon = GetNode<Sprite2D>(_itemIconPath);
		_itemIcon.Texture = _itemResource.Image;
    }
	public override void OnInteract(Player player) {
		GD.Print("Interacted!");
		GD.Print(_itemResource);
		player.InventoryManager.AddItemToInventory(_itemResource);
		this.QueueFree();
	}
}
