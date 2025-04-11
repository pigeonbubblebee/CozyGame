using Godot;
using System;

[GlobalClass]
public partial class MerchantInventory : Resource
{
	[Export] public string MerchantNameCode;
	[Export] public Item[] Inventory;
	[Export] public string[] Descriptions;
	[Export] public int[] Stock;
	[Export] public int[] Price;
	[Export] public int[] SellOutReq;
}
