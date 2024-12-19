using Godot;
using System;
using System.Collections.Generic;

public partial class InventoryUI : Control
{
	[Export] private Item t;
	[Export] private Item t2;
	[Export] private Item t3;
	[Export] private NodePath _gridContainerPath;
	private GridContainer _gridContainer;
	private List<InventorySlot> itemSlots = new List<InventorySlot>();
	
	private Player _currentScenePlayer;
	
	// Item Display
	[Export] private NodePath _ItemDisplayNamePath;
	private RichTextLabel _ItemDisplayName;
	[Export] private NodePath _ItemDisplayDescriptionPath;
	private RichTextLabel _ItemDisplayDescription;
	[Export] private NodePath _ItemDisplayImagePath;
	private TextureRect _ItemDisplayImage;
	
	public override void _Ready() {
		_gridContainer = GetNode<GridContainer>(_gridContainerPath);
		
		_ItemDisplayName = GetNode<RichTextLabel>(_ItemDisplayNamePath);
		_ItemDisplayDescription = GetNode<RichTextLabel>(_ItemDisplayDescriptionPath);
		_ItemDisplayImage = GetNode<TextureRect>(_ItemDisplayImagePath);
		
		_ItemDisplayName.Visible = false;
		_ItemDisplayDescription.Visible = false;
		_ItemDisplayImage.Visible = false;
	}
	
	public void CloseInventory() {
		_ItemDisplayName.Visible = false;
		_ItemDisplayDescription.Visible = false;
		_ItemDisplayImage.Visible = false;
	}
	
	private void _addItem(Item i) {
		InventorySlot instance = new InventorySlot(i, 1);
		_gridContainer.AddChild(instance);
		itemSlots.Add(instance);
		instance.OnItemClickEvent += _UpdateItemDisplay;
	}
	
	private void _updateItemSlotsHelper(Item i) {
		UpdateItemSlots(_currentScenePlayer.InventoryManager.CurrentInventory);
	}
	
	public void UpdateItemSlots(SortedDictionary<Item, int> items) {
		int addCount = 0;
		int removeCount = 0;
		
		// Get amount of slots needed to add/remove
		if(itemSlots.Count > items.Count) {
			removeCount = itemSlots.Count - items.Count;
		} else {
			addCount = items.Count - itemSlots.Count;
		}
		
		// Remove all unecessary item slot nodes
		for(int i = 0; i < removeCount; i++) {
			itemSlots[i].QueueFree();
		}
		itemSlots.RemoveRange(0, removeCount);
		
		// Add necessary nodes
		for(int i = 0; i < addCount; i++) { // TODO: Change to PackedScene
			InventorySlot instance = new InventorySlot();
			_gridContainer.AddChild(instance);
			itemSlots.Add(instance);
			instance.OnItemClickEvent += _UpdateItemDisplay;
		}
		
		// Correct the item slot displays
		int index = 0;
		foreach(Item i in items.Keys) {
			// TODO: Add stack size
			itemSlots[index].ChangeItem(i, 1);
			index ++;
		}
	}
	
	public void SetCurrentPlayerInstance(Player p) {
		_currentScenePlayer = p;
		p.InventoryManager.InventoryAddItemEvent += _updateItemSlotsHelper;
		p.InventoryManager.InventoryRemoveItemEvent += _updateItemSlotsHelper;
		
		
	}
	
	private void _UpdateItemDisplay(Item i) {
		_ItemDisplayName.Text = "[center]" + Tr(i.ID) + "[/center]"; // TODO: Add Localizer
		_ItemDisplayDescription.Text = Tr(i.Description);
		_ItemDisplayImage.Texture = i.Image;
		
		_ItemDisplayName.Visible = true;
		_ItemDisplayDescription.Visible = true;
		_ItemDisplayImage.Visible = true;
	}
}
