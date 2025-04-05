using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public partial class InventoryUI : Control
{
	[Export] private NodePath _gridContainerPath;
	private GridContainer _gridContainer;
	private List<InventorySlot> itemSlots = new List<InventorySlot>();

	[Export] private NodePath _equippedContainerPath;
	private GridContainer _equippedContainer;
	private List<InventorySlot> equippedSlots = new List<InventorySlot>();
	
	private Player _currentScenePlayer;
	
	// Item Display
	[Export] private NodePath _ItemDisplayNamePath;
	private RichTextLabel _ItemDisplayName;
	[Export] private NodePath _ItemDisplayDescriptionPath;
	private RichTextLabel _ItemDisplayDescription;
	[Export] private NodePath _ItemDisplayImagePath;
	private TextureRect _ItemDisplayImage;

	[Export] private Texture2D _inventorySlotTexture;
	[Export] private Font _inventorySlotFont;

	private RichTextLabel _equipPrompt;
	[Export] private NodePath _equipPromptPath;

	private IInputManager _inputManager;
	private Item _currentSelectedItem;
	public int SelectedEquipSlot;

	private Control _tooltip;
	[Export] private NodePath _tooltipPath;
	private RichTextLabel _tooltipText;
	[Export] private NodePath _tooltipTextPath;

	public int CurrentScreen { get; private set; }
	[Export] private NodePath[] _screensPath;
	private CanvasItem[] _screens;

	[Export] private NodePath _leftMenuIconPath;
	private TextureRect _leftMenuIcon;
	[Export] private NodePath _rightMenuIconPath;
	private TextureRect _rightMenuIcon;

	[Export] private Texture2D[] _menuIcons;

	[Export] private NodePath _attributesPath;
	private RichTextLabel _attributes;
	[Export] private NodePath _attributesValPath;
	private RichTextLabel _attributesVal;
	[Export] private NodePath _moneyTextPath;
	private RichTextLabel _moneyText;
	[Export] private NodePath _levelsTextPath;
	private RichTextLabel _levelsText;

	[Export] private NodePath _incrementAttributePath;
	[Export] private NodePath _decrementAttributePath;
	private AttributeButton[] _incrementAttributesButtons;
	private AttributeButton[] _decrementAttributesButtons;

	
	public override void _Ready() {
		CurrentScreen = 1;

		_inputManager = GetNode<IInputManager>("/root/InputManager");
		_gridContainer = GetNode<GridContainer>(_gridContainerPath);
		_equippedContainer = GetNode<GridContainer>(_equippedContainerPath);

		_attributes = GetNode<RichTextLabel>(_attributesPath);
		_attributes.Text = _createDescriptionNoColor(_attributes.Text);

		_moneyText = GetNode<RichTextLabel>(_moneyTextPath);
		_levelsText = GetNode<RichTextLabel>(_levelsTextPath);

		_screens = new CanvasItem[_screensPath.Length];
		for(int i = 0; i < _screensPath.Length; i++) {
			_screens[i] = GetNode<CanvasItem>(_screensPath[i]);
			_screens[i].Visible = false;

			if(i == CurrentScreen) {
				_screens[i].Visible = true;
			}
		}

		_attributes.MetaHoverStarted += _showTooltip;
		_attributes.MetaHoverEnded += _hideToolTip;

		_attributesVal = GetNode<RichTextLabel>(_attributesValPath);
		
		Node incrementParent = GetNode<Node>(_incrementAttributePath);
		Node decrementParent = GetNode<Node>(_decrementAttributePath);

		_incrementAttributesButtons = new AttributeButton[incrementParent.GetChildren().Count];
		_decrementAttributesButtons = new AttributeButton[decrementParent.GetChildren().Count];

		for(int i = 0; i < incrementParent.GetChildren().Count; i++) {
			_incrementAttributesButtons[i] = (AttributeButton)(incrementParent.GetChildren()[i]);
			_decrementAttributesButtons[i] = (AttributeButton)(decrementParent.GetChildren()[i]);

			_incrementAttributesButtons[i].IncrementEvent += _IncrementAttribute;
			_decrementAttributesButtons[i].DecrementEvent += _DecrementAttribute;
		}

		

		_leftMenuIcon = GetNode<TextureRect>(_leftMenuIconPath);
		_rightMenuIcon = GetNode<TextureRect>(_rightMenuIconPath);

		_UpdateMenuIcons();

		_equipPrompt = GetNode<RichTextLabel>(_equipPromptPath);
		
		_ItemDisplayName = GetNode<RichTextLabel>(_ItemDisplayNamePath);
		_ItemDisplayDescription = GetNode<RichTextLabel>(_ItemDisplayDescriptionPath);
		_ItemDisplayImage = GetNode<TextureRect>(_ItemDisplayImagePath);

		_ItemDisplayDescription.MetaHoverStarted += _showTooltip;
		_ItemDisplayDescription.MetaHoverEnded += _hideToolTip;
		
		_ItemDisplayName.Visible = false;
		_ItemDisplayDescription.Visible = false;
		_ItemDisplayImage.Visible = false;
		_equipPrompt.Visible = false;

		_tooltip = GetNode<Control>(_tooltipPath);
		_tooltipText = GetNode<RichTextLabel>(_tooltipTextPath);

		_tooltip.Visible = false;

		SelectedEquipSlot = 0;
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
		if(_inputManager.GetMenuLeftActuation() && this.Visible) {
			_screens[CurrentScreen].Visible = false;

			if(CurrentScreen == 0) {
				CurrentScreen = 2;
			} else {
				CurrentScreen --;
			}

			//GD.Print(CurrentScreen);

			_screens[CurrentScreen].Visible = true;

			_UpdateMenuIcons();
		}

		if(_inputManager.GetMenuRightActuation() && this.Visible) {
			_screens[CurrentScreen].Visible = false;

			if(CurrentScreen == 2) {
				CurrentScreen = 0;
			} else {
				CurrentScreen ++;
			}

			//GD.Print(CurrentScreen);

			_screens[CurrentScreen].Visible = true;

			_UpdateMenuIcons();
		}

		if(_inputManager.GetInteractActuation() && CurrentScreen == 1) {
			bool emptySlot = false;
			bool alreadyEquipped = false;
			for(SelectedEquipSlot = 0; SelectedEquipSlot < _currentScenePlayer.InventoryManager.EquippedItems.Length; SelectedEquipSlot++) {
				if(_currentScenePlayer.InventoryManager.EquippedItems[SelectedEquipSlot] == null) {
					emptySlot = true;
					break;
				}
				if(_currentScenePlayer.InventoryManager.EquippedItems[SelectedEquipSlot] == _currentSelectedItem) {
					alreadyEquipped = true;
					_currentScenePlayer.InventoryManager.UnequipItem(SelectedEquipSlot);

					// equippedSlots[SelectedEquipSlot].RemoveItem();

					_updateEquipPrompt();
				}
			}
			if(emptySlot && !alreadyEquipped) {
				if(_currentSelectedItem is Equippable) {
					_currentScenePlayer.InventoryManager.EquipItem((Equippable)_currentSelectedItem, SelectedEquipSlot);
					// equippedSlots[SelectedEquipSlot].ChangeItem(_currentSelectedItem, 1);

					_updateEquipPrompt();
				}
			}
		}
    }

	private void _IncrementAttribute() {
		GD.Print("add increment");

		if(_currentScenePlayer == null)
			return;
		if(_currentScenePlayer.InventoryManager.GetItemCount("forest_essence") <= 0)
			return;

		if(_incrementAttributesButtons[0].Selected) {
			_currentScenePlayer.BaseBuffs.Strength += 1;
		}
		if(_incrementAttributesButtons[1].Selected) {
			_currentScenePlayer.BaseBuffs.Dexterity += 1;
		}
		if(_incrementAttributesButtons[2].Selected) {
			_currentScenePlayer.BaseBuffs.Vitality += 1;
		}
		if(_incrementAttributesButtons[3].Selected) {
			_currentScenePlayer.BaseBuffs.Focus += 1;
		}
		if(_incrementAttributesButtons[4].Selected) {
			_currentScenePlayer.BaseBuffs.Harmony += 1;
		}
		_currentScenePlayer.InventoryManager.RemoveItemFromInventory("forest_essence");
	}

	private void _DecrementAttribute() {
		GD.Print("decrement");
		if(_currentScenePlayer == null)
			return;
		
		if(_decrementAttributesButtons[0].Selected && _currentScenePlayer.BaseBuffs.Strength > 0) {
			_currentScenePlayer.BaseBuffs.Strength -= 1;
			_currentScenePlayer.InventoryManager.AddItemToInventory("forest_essence");
		}
		if(_decrementAttributesButtons[1].Selected && _currentScenePlayer.BaseBuffs.Dexterity > 0) {
			_currentScenePlayer.BaseBuffs.Dexterity -= 1;
			_currentScenePlayer.InventoryManager.AddItemToInventory("forest_essence");
		}
		if(_decrementAttributesButtons[2].Selected && _currentScenePlayer.BaseBuffs.Vitality > 0) {
			_currentScenePlayer.BaseBuffs.Vitality -= 1;
			_currentScenePlayer.InventoryManager.AddItemToInventory("forest_essence");
		}
		if(_decrementAttributesButtons[3].Selected && _currentScenePlayer.BaseBuffs.Focus > 0) {
			_currentScenePlayer.BaseBuffs.Focus -= 1;
			_currentScenePlayer.InventoryManager.AddItemToInventory("forest_essence");
		}
		if(_decrementAttributesButtons[4].Selected && _currentScenePlayer.BaseBuffs.Harmony > 0) {
			_currentScenePlayer.BaseBuffs.Harmony -= 1;
			_currentScenePlayer.InventoryManager.AddItemToInventory("forest_essence");
		}
	}

	private void _UpdateMenuIcons() {
		_leftMenuIcon.Texture = CurrentScreen == 0 ? _menuIcons[2] : _menuIcons[CurrentScreen - 1];
		_rightMenuIcon.Texture = CurrentScreen == 2 ? _menuIcons[0] : _menuIcons[CurrentScreen + 1];
	}

    public void CloseInventory() {
		_ItemDisplayName.Visible = false;
		_ItemDisplayDescription.Visible = false;
		_ItemDisplayImage.Visible = false;
		_equipPrompt.Visible = false;
	}
	
	private void _addItem(Item i, int stack) {
		// GD.Print("texture:" + _inventorySlotTexture);
		InventorySlot instance = new InventorySlot(i, stack, _inventorySlotTexture, _inventorySlotFont);
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
		
	// 	GD.Print("ItemSlots: " + itemSlots.Count + " " + items.Count);
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
			InventorySlot instance = new InventorySlot(_inventorySlotTexture, _inventorySlotFont);
			_gridContainer.AddChild(instance);
			itemSlots.Add(instance);
			instance.OnItemClickEvent += _UpdateItemDisplay;
		}
		
		// Correct the item slot displays
		int index = 0;
		foreach(Item i in items.Keys) {
			// TODO: Add stack size
			// GD.Print("stack size:" +  items[i]);
			itemSlots[index].ChangeItem(i, items[i]);
			index ++;
		}
	}

	private void _updateEquipSlots() {
		// GD.Print("Update Equip" +  _currentScenePlayer.InventoryManager.EquippedItems.Length);
		for(int i = 0; i < _currentScenePlayer.InventoryManager.EquippedItems.Length; i++) {
			// GD.Print("inventory slot null:" + _currentScenePlayer.InventoryManager.EquippedItems[i] != null + " " + _currentScenePlayer.InventoryManager.EquippedItems[i]);
			
			// GD.Print("Equip Slot: " + i + " " + _currentScenePlayer.InventoryManager.EquippedItems[i]);
			if(_currentScenePlayer.InventoryManager.EquippedItems[i] == null) {
				equippedSlots[i].RemoveItem();
			} else {
				equippedSlots[i].ChangeItem(_currentScenePlayer.InventoryManager.EquippedItems[i], 1);
			}
		}
	}
	
	public void SetCurrentPlayerInstance(Player p) {
		_currentScenePlayer = p;
		p.InventoryManager.InventoryAddItemEvent += _updateItemSlotsHelper;
		p.InventoryManager.InventoryRemoveItemEvent += _updateItemSlotsHelper;
		p.InventoryManager.ChangeEquipEvent += _updateEquipSlots;

		for(int i = 0; i < equippedSlots.Count; i++) {
			equippedSlots[i].QueueFree();
		}
		equippedSlots.Clear();

		for(int i = 0; i < p.CurrentPlayerStats.EquipSlots; i++) {
			InventorySlot instance = new InventorySlot(_inventorySlotTexture, _inventorySlotFont);
			_equippedContainer.AddChild(instance);
			equippedSlots.Add(instance);
			instance.OnItemClickEvent += _UpdateItemDisplay;
		}
	}

	public void UpdateStatusScreen() {
		if(_currentScenePlayer == null || CurrentScreen != 2)
			return;
		_attributesVal.Text = "[right]" + _currentScenePlayer.PlayerHealth.CurrentHealthPoints + " / " + _currentScenePlayer.PlayerHealth.MaxHealthPoints + "\n"
			+ (_currentScenePlayer.CurseController.CurrentCurse) + " / " + _currentScenePlayer.CurseController.MaxCurse + "\n"
			+ (_currentScenePlayer.HealController.CurrentHeals) + " / " + _currentScenePlayer.CurrentPlayerStats.MaxHeals + "\n\n";
			// + _currentScenePlayer.CurrentBuffs.Strength + "\n" + _currentScenePlayer.CurrentBuffs.Dexterity + "\n" + _currentScenePlayer.CurrentBuffs.Vitality 
			// + "\n" + _currentScenePlayer.CurrentBuffs.Focus + "\n" + _currentScenePlayer.CurrentBuffs.Harmony;
		

		_attributesVal.Text += (_currentScenePlayer.CurrentBuffs.Strength - _currentScenePlayer.BaseBuffs.Strength == 0) ? _currentScenePlayer.BaseBuffs.Strength + "\n" : 
			_currentScenePlayer.BaseBuffs.Strength + "[color=#56d36d]+" + (_currentScenePlayer.CurrentBuffs.Strength - _currentScenePlayer.BaseBuffs.Strength) + "[/color]\n";
		_attributesVal.Text += (_currentScenePlayer.CurrentBuffs.Dexterity - _currentScenePlayer.BaseBuffs.Dexterity == 0) ? _currentScenePlayer.BaseBuffs.Dexterity + "\n" : 
			_currentScenePlayer.BaseBuffs.Dexterity + "[color=#56d36d]+" + (_currentScenePlayer.CurrentBuffs.Dexterity - _currentScenePlayer.BaseBuffs.Dexterity) + "[/color]\n";
		_attributesVal.Text += (_currentScenePlayer.CurrentBuffs.Vitality - _currentScenePlayer.BaseBuffs.Vitality == 0) ? _currentScenePlayer.BaseBuffs.Vitality + "\n" : 
			_currentScenePlayer.BaseBuffs.Vitality + "[color=#56d36d]+" + (_currentScenePlayer.CurrentBuffs.Vitality - _currentScenePlayer.BaseBuffs.Vitality) + "[/color]\n";
		_attributesVal.Text += (_currentScenePlayer.CurrentBuffs.Focus - _currentScenePlayer.BaseBuffs.Focus == 0) ? _currentScenePlayer.BaseBuffs.Focus + "\n" : 
			_currentScenePlayer.BaseBuffs.Focus + "[color=#56d36d]+" + (_currentScenePlayer.CurrentBuffs.Focus - _currentScenePlayer.BaseBuffs.Focus) + "[/color]\n";
		_attributesVal.Text += (_currentScenePlayer.CurrentBuffs.Harmony - _currentScenePlayer.BaseBuffs.Harmony == 0) ? _currentScenePlayer.BaseBuffs.Harmony + "\n" : 
			_currentScenePlayer.BaseBuffs.Harmony + "[color=#56d36d]+" + (_currentScenePlayer.CurrentBuffs.Harmony - _currentScenePlayer.BaseBuffs.Harmony) + "[/color]\n";

		_moneyText.Text = "[right]" + _currentScenePlayer.InventoryManager.GetItemCount("acorn").ToString();
		_levelsText.Text = "[right]" + _currentScenePlayer.InventoryManager.GetItemCount("forest_essence").ToString();

		_decrementAttributesButtons[0].Visible = _currentScenePlayer.BaseBuffs.Strength > 0;
		_decrementAttributesButtons[1].Visible = _currentScenePlayer.BaseBuffs.Dexterity > 0;
		_decrementAttributesButtons[2].Visible = _currentScenePlayer.BaseBuffs.Vitality > 0;
		_decrementAttributesButtons[3].Visible = _currentScenePlayer.BaseBuffs.Focus > 0;
		_decrementAttributesButtons[4].Visible = _currentScenePlayer.BaseBuffs.Harmony > 0;

		foreach(CanvasItem item in _incrementAttributesButtons) {
			item.Visible = _currentScenePlayer.InventoryManager.GetItemCount("forest_essence") > 0;
		}
	}
	
	private void _UpdateItemDisplay(Item i) {
		_ItemDisplayName.Text = "[center]" + Tr(i.ID) + "[/center]"; // TODO: Add Localizer
		_ItemDisplayDescription.Text = Tr("item_type_" + i.Type) + "\n\n";
		_ItemDisplayDescription.Text += _createDescription(Tr(i.Description));

		Dictionary<string, string> attributes = i.GetAttributes(_currentScenePlayer);
		if(attributes.Keys.Count > 0)
			_ItemDisplayDescription.Text += "\n\n";
		foreach(string s in attributes.Keys) {
			_ItemDisplayDescription.Text += Tr("attribute_" + s) + ": " + _createDescription(attributes[s]) + "\n";
		}
		
		_equipPrompt.Visible = i is Equippable;
		
		
		
		_ItemDisplayImage.Texture = i.Image;
		
		_ItemDisplayName.Visible = true;
		_ItemDisplayDescription.Visible = true;
		_ItemDisplayImage.Visible = true;

		_currentSelectedItem = i;

		_updateEquipPrompt();
	}

	private void _updateEquipPrompt() {
		bool alreadyEquipped = false;

		if(_currentSelectedItem != null) {
			for(int j = 0; j < _currentScenePlayer.InventoryManager.EquippedItems.Length; j++) {
				if(_currentScenePlayer.InventoryManager.EquippedItems[j] == _currentSelectedItem) {
					alreadyEquipped = true;
				}
			}
		}

		if(alreadyEquipped) {
			_equipPrompt.Text = "[right]" + "F" + " - Unequip";
		} else {
			_equipPrompt.Text = "[right]" + "F" + " - Equip";
		}
	}

	private string _createDescription(string desc) {
		string res = desc;
		res = res.Replace("STRENGTH", "[url=\"tooltip_strength\"][color=#f93e00]STRENGTH[/color][/url]");
		res = res.Replace("MYSTIC", "[url=\"tooltip_mystic\"][color=#78ff26]MYSTIC[/color][/url]");
		res = res.Replace("HEALTH", "[url=\"tooltip_health\"][color=#cf0451]HEALTH[/color][/url]");
		res = res.Replace("REGROWTH", "[url=\"tooltip_regrowth\"][color=#cf0451]REGROWTH[/color][/url]");
		res = res.Replace("DEXTERITY", "[url=\"tooltip_dexterity\"][color=#ecd42c]DEXTERITY[/color][/url]");
		res = res.Replace("VITALITY", "[url=\"tooltip_vitality\"][color=#cf0451]VITALITY[/color][/url]");
		res = res.Replace("HARMONY", "[url=\"tooltip_harmony\"][color=#78ff26]HARMONY[/color][/url]");
		res = res.Replace("FOCUS", "[url=\"tooltip_focus\"][color=#9fe7ff]FOCUS[/color][/url]");
		res = res.Replace("PROWESS", "[url=\"tooltip_prowess\"][color=#ff7103]PROWESS[/color][/url]");
		res = res.Replace("DECAY", "[url=\"tooltip_decay\"][color=#2e2a32]DECAY[/color][/url]");
		res = res.Replace("DAZE", "[url=\"tooltip_daze\"][color=#d84f2f]DAZE[/color][/url]");
		res = res.Replace("CRITICAL", "[url=\"tooltip_critical\"][color=#ecd42c]CRITICAL[/color][/url]");
		
		// res = _addColorToText(res, "STRENGTH", "d84f2f");
		//res = _addColorToText(res, "MYSTIC", "78ff26");
		return res;
	}

	private string _createDescriptionNoColor(string desc) {
		string res = desc;
		res = res.Replace("STRENGTH", "[url=\"tooltip_strength\"]STRENGTH[/url]");
		res = res.Replace("MYSTIC", "[url=\"tooltip_mystic\"]MYSTIC[/url]");
		res = res.Replace("HEALTH", "[url=\"tooltip_health\"]HEALTH[/url]");
		res = res.Replace("REGROWTH", "[url=\"tooltip_regrowth\"]REGROWTH[/url]");
		res = res.Replace("DEXTERITY", "[url=\"tooltip_dexterity\"]DEXTERITY[/url]");
		res = res.Replace("VITALITY", "[url=\"tooltip_vitality\"]VITALITY[/url]");
		res = res.Replace("HARMONY", "[url=\"tooltip_harmony\"]HARMONY[/url]");
		res = res.Replace("FOCUS", "[url=\"tooltip_focus\"]FOCUS[/url]");
		res = res.Replace("PROWESS", "[url=\"tooltip_prowess\"]PROWESS[/url]");
		res = res.Replace("DECAY", "[url=\"tooltip_decay\"]DECAY[/url]");
		res = res.Replace("DAZE", "[url=\"tooltip_daze\"]DAZE[/url]");
		res = res.Replace("CRITICAL", "[url=\"tooltip_critical\"]CRITICAL[/url]");
		
		// res = _addColorToText(res, "STRENGTH", "d84f2f");
		//res = _addColorToText(res, "MYSTIC", "78ff26");
		return res;
	}

	private void _showTooltip(Variant meta) {
		_tooltip.Visible = true;
		_tooltip.Position = GetGlobalMousePosition();
		_tooltipText.Text = _createDescription(Tr(meta.ToString()));
		// GD.Print(meta.ToString());
	}

	private void _hideToolTip(Variant meta) {
		_tooltip.Visible = false;
		// GD.Print(meta.ToString());
	}
}
