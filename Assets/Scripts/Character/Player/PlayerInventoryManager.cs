using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerInventoryManager : Node
{
	private SortedDictionary<Item, int> _playerInventory = new SortedDictionary<Item, int>(new ItemComparer());
	public SortedDictionary<Item, int> CurrentInventory => _playerInventory;
	
	public event Action<Item> InventoryAddItemEvent;
	public event Action<Item> InventoryRemoveItemEvent;
	
	public int GetItemCount(Item i) {
		int value;
		if(_playerInventory.TryGetValue(i, out value)) {
			return value;
		}

		return 0;
	}

	public void AddItemToInventory(Item i) {
		int value;
		GD.Print(i);
		if(_playerInventory.TryGetValue(i, out value)) {
			_playerInventory[i] = value + 1;
		} else {
			_playerInventory.Add(i, 1);
		}

		InventoryAddItemEvent?.Invoke(i);
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
}
