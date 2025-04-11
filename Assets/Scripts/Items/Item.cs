using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Item : Resource, IComparable<Item> {
	[Export] public string ID { get; set; }
	[Export] public string Description { get; set; }
	[Export] public float ItemTier { get; set; }
	[Export] public int SortingID { get; set; }
	[Export] public Texture2D Image { get; set; }
	[Export] public ItemType Type { get; set; }
	[Export] protected string[] _attributeNames { get; set; }
	[Export] protected string[] _attributeValues { get; set; }
	[Export] public bool Stackable { get; private set; }

	public int CompareTo(Item other) { // Reversed Compare so important items show up at the top.
		if(other.ItemTier < ItemTier) {
			return -1;
		} else if(other.ItemTier > ItemTier) {
			return 1;
		} else {
			return -SortingID.CompareTo(other.SortingID);
		}
	}

	public virtual string GetItemPath() {
		return ID;
	}

	public Dictionary<string, string> GetAttributes(Player p) {
		Dictionary<string, string> res = new Dictionary<string, string>();
		if(_attributeNames == null) {
			return res;
		}
		for(int i = 0; i < _attributeNames.Length; i++) {
			res.Add(_attributeNames[i], _attributeValues[i]);
		}
		return res;
	}
	
	public enum ItemType {
		Weapon,
		Pickup,
		equippable,
		key_item,
		upgrade_material,
		achohol
	}
}
