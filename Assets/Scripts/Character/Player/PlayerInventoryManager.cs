using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerInventoryManager : Node
{
	private SortedDictionary<Item, int> _playerInventory = new SortedDictionary<Item, int>(new ItemComparer());
	public SortedDictionary<Item, int> CurrentInventory => _playerInventory;
	
	public event Action<Item> InventoryAddItemEvent;
	public event Action<Item> InventoryRemoveItemEvent;
	public event Action ChangeEquipEvent;

	public Equippable[] EquippedItems;
	private Player _player;

	public void Initialize(Player p) {
		_player = p;
		EquippedItems = new Equippable[p.CurrentPlayerStats.EquipSlots];
	}
	
	public int GetItemCount(Item i) {
		int value;
		if(_playerInventory.TryGetValue(i, out value)) {
			return value;
		}

		return 0;
	}

	public int GetItemCount(String id) {
		int value;

		foreach(Item i in _playerInventory.Keys) {
			if(i.ID.Equals(id)) {
				value = _playerInventory[i];
				return value;
			}
		}

		return 0;
	}

	public void AddItemToInventory(Item i) {
		int value;
		//GD.Print(i);
		if(_playerInventory.TryGetValue(i, out value)) {
			_playerInventory[i] = value + 1;
		} else {
			_playerInventory.Add(i, 1);
		}

		InventoryAddItemEvent?.Invoke(i);
	}

	public void AddItemToInventory(Item item, int stack) {
		for(int i = 0; i < stack; i++) {
			AddItemToInventory(item);
		}
	}

	public void EquipItem(Equippable e, int slot) {
		if(EquippedItems[slot] != null) {
			EquippedItems[slot].OnUnequip(_player);
		}

		e.OnEquip(_player);

		EquippedItems[slot] = e;
		//GD.Print(EquippedItems[slot].ID);

		ChangeEquipEvent?.Invoke();

		_player.UpdateBuffs();
	}

	public void UnequipItem(int slot) {
		if(EquippedItems[slot] != null) {
			EquippedItems[slot].OnUnequip(_player);
		}
		
		EquippedItems[slot] = null;

		ChangeEquipEvent?.Invoke();
		_player.UpdateBuffs();
	}

	public void AddItemToInventory(string id) {
		AddItemToInventory((Item)_LoadItemResource(id));
	}

	public void RemoveItemFromInventory(Item i) {
		int value;
		if(_playerInventory.TryGetValue(i, out value)) {
			_playerInventory[i] = value - 1;
			if(_playerInventory[i] == 0) {
				_playerInventory.Remove(i);
			}
		}
		InventoryRemoveItemEvent?.Invoke(i);
	}

	public void RemoveItemFromInventory(string ID) {
		// bool zero = false;
		Item item = new Item();

		foreach(Item i in _playerInventory.Keys) {
			if(i.ID.Equals(ID)) {
				item = i;
				
			}
		}

		if(item != null) {
			_playerInventory[item] -= 1;

			if(_playerInventory[item] == 0)
				_playerInventory.Remove(item);
		}

		InventoryRemoveItemEvent?.Invoke(item);
	}

	public string[] SerializeEquipped() {
		string[] res = new string[EquippedItems.Length];
		int index = 0;
		foreach(Item i in EquippedItems) {
			if(i != null) {
				res[index] = (i.GetItemPath());
			} else {
				res[index] = ("");
			}
			index++;
		}
		return res;
	}

	public bool HasItem(string ID) {
		foreach(Item i in _playerInventory.Keys) {
			if(i.ID.Equals(ID))
				return true;
		}
		return false;
	}
	
	public string[] SerializeInventory() {
		string[] res = new string[_playerInventory.Keys.Count];
		int index = 0;
		foreach(Item i in _playerInventory.Keys) {
			res[index] = (i.GetItemPath());
			index++;
		}
		return res;
	}

	public int[] SerializeInventoryStacks() {
		int[] res = new int[_playerInventory.Keys.Count];
		int index = 0;
		foreach(Item i in _playerInventory.Keys) {
			res[index] = (_playerInventory[i]);
			index++;
		}
		return res;
	}

	public void ReadInventory(string[] inventory, int[] inventoryStacks) {
		_playerInventory.Clear();
		
		int index = 0;
		foreach(string s in inventory) {
			for(int i = 0; i < inventoryStacks[index]; i++) {
				AddItemToInventory((Item)_LoadItemResource(s));
			}
			index++;
		}
	}

	public void ReadEquipped(string[] equipped) {
		// _playerInventory.Clear();
		EquippedItems = new Equippable[_player.CurrentPlayerStats.EquipSlots];
		
		int index = 0;
		foreach(string s in equipped) {
			if(!s.Equals("")) {
				EquipItem((Equippable)_LoadItemResource(s), index);
			} else {
				EquippedItems[index] = null;
			}
			index++;
		}
	}

	private Resource _LoadItemResource(string targetFile) {
		return GD.Load<Resource>("res://Data/Items/"+targetFile+".tres");
		// foreach(string file_name in DirAccess.GetFilesAt("res://Data/Items/")) {
		// 	GD.Print(file_name);
		// 	string new_file_name = "";
		// 	if (file_name.GetExtension() == "import") {
		// 		new_file_name = file_name.Replace(".import", "");
		// 	} else {
		// 		new_file_name = file_name;
		// 	}
			
		// 	if(new_file_name.Equals(targetFile + ".tres")) {
		// 		GD.Print(GD.Load<Resource>("res://Data/Items/"+new_file_name));
		// 		return GD.Load<Resource>("res://Data/Items/"+new_file_name);
		// 	}
		// }

		// return null;
	}
}
