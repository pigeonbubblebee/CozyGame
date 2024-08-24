using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public abstract partial class Item : Resource, IComparable<Item> {
    [Export] public string ID { get; set; }
    [Export] public string Description { get; set; }
    [Export] public int ItemTier { get; set; }
    [Export] public int SortingID { get; set; }
    [Export] public Texture Image { get; set; }

    public int CompareTo(Item other) {
        if(other.ItemTier < ItemTier) {
            return 1;
        } else if(other.ItemTier > ItemTier) {
            return -1;
        } else {
            return SortingID.CompareTo(other.SortingID);
        }
    }
}