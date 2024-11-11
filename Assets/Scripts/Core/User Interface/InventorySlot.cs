using Godot;
using System;

public partial class InventorySlot : Panel
{
	private TextureRect _itemImage;
	private Item _item;
	public event Action<Item> OnItemClickEvent;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.CustomMinimumSize = new Vector2(128, 128);
		
		TextureRect instance = new TextureRect();
		_itemImage = instance;
		
		AddChild(_itemImage);
		_itemImage.CustomMinimumSize = new Vector2(128, 128);
		if(_item != null) {
			_itemImage.Texture = _item.Image;
		}
	}
	
	public void ChangeItem(Item i, int stack) {
		_itemImage.Texture = i.Image;
		_item = i;
	}
	
	public InventorySlot(Item i, int stack) {
		_item = i;
	}
	
	public InventorySlot() {
		// TODO: Add default blank texture
	}
	
	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mb)
		{
			if (mb.ButtonIndex == MouseButton.Left && mb.Pressed)
			{
				OnItemClickEvent?.Invoke(_item);
			}
		}
	}
}
