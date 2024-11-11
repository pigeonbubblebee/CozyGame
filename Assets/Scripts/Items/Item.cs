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

	public int CompareTo(Item other) { // Reversed Compare so important items show up at the top.
		if(other.ItemTier < ItemTier) {
			return -1;
		} else if(other.ItemTier > ItemTier) {
			return 1;
		} else {
			return -SortingID.CompareTo(other.SortingID);
		}
	}
	
	public enum ItemType {
		Weapon,
		Pickup
	}
}
