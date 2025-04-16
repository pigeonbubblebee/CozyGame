using Godot;
using System;
using System.Collections.Generic;

public partial class MerchantUI : Control
{
	[Export] private NodePath _gridContainerPath;
	private GridContainer _gridContainer;

	[Export] private NodePath _ItemDisplayNamePath;
	private RichTextLabel _ItemDisplayName;
	[Export] private NodePath _ItemDisplayDescriptionPath;
	private RichTextLabel _ItemDisplayDescription;
	[Export] private NodePath _ItemDisplayImagePath;
	private TextureRect _ItemDisplayImage;

	[Export] private Texture2D _inventorySlotTexture;
	[Export] private Font _inventorySlotFont;

	private IInputManager _inputManager;
	private Item _currentSelectedItem;
	private int _currentSelectedIndex;
	private Control _tooltip;
	[Export] private NodePath _tooltipPath;
	private RichTextLabel _tooltipText;
	[Export] private NodePath _tooltipTextPath;

	private RichTextLabel _moneyLabel;
	[Export] private NodePath _moneyLabelPath;

	private MerchantInventory _currentMerchant;
	private Player _player;
	private SaveLoader _saveLoader;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetNode<Player>("/root/Player");
		_inputManager = GetNode<IInputManager>("/root/InputManager");
		_saveLoader = GetNode<SaveLoader>("/root/SaveLoader");
		_gridContainer = GetNode<GridContainer>(_gridContainerPath);
		
		_ItemDisplayName = GetNode<RichTextLabel>(_ItemDisplayNamePath);
		_ItemDisplayDescription = GetNode<RichTextLabel>(_ItemDisplayDescriptionPath);
		_ItemDisplayImage = GetNode<TextureRect>(_ItemDisplayImagePath);

		_moneyLabel = GetNode<RichTextLabel>(_moneyLabelPath);

		_ItemDisplayDescription.MetaHoverStarted += _showTooltip;
		_ItemDisplayDescription.MetaHoverEnded += _hideToolTip;
		
		_ItemDisplayName.Visible = false;
		_ItemDisplayDescription.Visible = false;
		_ItemDisplayImage.Visible = false;

		_tooltip = GetNode<Control>(_tooltipPath);
		_tooltipText = GetNode<RichTextLabel>(_tooltipTextPath);

		_tooltip.Visible = false;
	}

	public void LoadMerchant(MerchantInventory merchant) {
		_currentMerchant = merchant;

		foreach(Node n in _gridContainer.GetChildren()) {
			n.QueueFree();
		}

		for(int i = 0; i < merchant.Inventory.Length; i++) {
			if(_saveLoader.GetStock(merchant.MerchantNameCode, i, merchant) == 0)
				continue;

			if(merchant.SellOutReq[i] != -1) {
				if(_saveLoader.GetStock(merchant.MerchantNameCode, merchant.SellOutReq[i], merchant) > 0) {
					continue;
				}
			}
			
			InventorySlot instance = new InventorySlot(_inventorySlotTexture, _inventorySlotFont);
			instance.MerchantIndex = i;
			
			_gridContainer.AddChild(instance);
			instance.ChangeItem(merchant.Inventory[i], merchant.Price[i]);
			
			instance.OnMerchantItemClickEvent += _UpdateItemDisplay;
		}
	}

	private void _UpdateItemDisplay(Item i, int index) {
		_ItemDisplayName.Text = "[center]" + Tr(i.ID) + "[/center]"; // TODO: Add Localizer
		_ItemDisplayDescription.Text = Tr("item_type_" + i.Type) + "\n\n";
		_ItemDisplayDescription.Text += Tr("price") + _currentMerchant.Price[index] + " " + Tr("acorn") + "\n";
		_ItemDisplayDescription.Text += Tr("stock") + _saveLoader.GetStock(_currentMerchant.MerchantNameCode, index, _currentMerchant) + Tr("stock_left") + "\n\n";
		_ItemDisplayDescription.Text += InventoryUI._createDescription(Tr(_currentMerchant.Descriptions[index]));

		// Dictionary<string, string> attributes = i.GetAttributes(_player);
		// if(attributes.Keys.Count > 0)
		// 	_ItemDisplayDescription.Text += "\n\n";
		// foreach(string s in attributes.Keys) {
		// 	_ItemDisplayDescription.Text += Tr("attribute_" + s) + ": " + InventoryUI._createDescription(attributes[s]) + "\n";
		// }
		
		// _equipPrompt.Visible = i is Equippable;
		
		_ItemDisplayImage.Texture = i.Image;
		
		_ItemDisplayName.Visible = true;
		_ItemDisplayDescription.Visible = true;
		_ItemDisplayImage.Visible = true;

		_currentSelectedItem = i;
		_currentSelectedIndex = index;

		// _updateEquipPrompt();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(_inputManager.GetInteractActuation() && Visible) {
			if(_player.InventoryManager.GetItemCount("acorn") >= _currentMerchant.Price[_currentSelectedIndex]) {
				// update stock in save loader
				_player.InventoryManager.AddItemToInventory(_currentSelectedItem);
				_saveLoader.ReduceStock(_currentMerchant.MerchantNameCode, _currentSelectedIndex, _currentMerchant);
				for(int i = 0; i < _currentMerchant.Price[_currentSelectedIndex]; i++) {
					_player.InventoryManager.RemoveItemFromInventory("acorn");
				}
				_ItemDisplayName.Visible = false;
				_ItemDisplayDescription.Visible = false;
				_ItemDisplayImage.Visible = false;
				LoadMerchant(_currentMerchant);
			}
		}
		_moneyLabel.Text = _player.InventoryManager.GetItemCount("acorn").ToString();
	}

	private void _showTooltip(Variant meta) {
		_tooltip.Visible = true;
		_tooltip.Position = GetGlobalMousePosition();
		_tooltipText.Text = InventoryUI._createDescription(Tr(meta.ToString()));
		// GD.Print(meta.ToString());
	}

	private void _hideToolTip(Variant meta) {
		_tooltip.Visible = false;
		// GD.Print(meta.ToString());
	}
}
