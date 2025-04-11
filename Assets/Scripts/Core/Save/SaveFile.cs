using Godot;
using System;
using System.Collections.Generic;

using Dictionary = Godot.Collections.Dictionary;
using Array = Godot.Collections.Array;

public class SaveFile
{
	public int PlayerHealth { get; set; }
	public int PlayerHeals { get; set; }
	public int PlayerMystic { get; set; }
	public List<RoomData> RoomDatas { get; set; } = new List<RoomData>();
	public string RespawnID; // Delimited by ;
	public string[] inventory = new string[0];
	public int[] inventoryStacks = new int[0];

	public string[] equipped = new string[0];

	public int[] attributes = new int[0];
	public List<MerchantData> MerchantDatas { get; set; } = new List<MerchantData>();
	
	public class RoomData {
		public string RoomID;
		public bool[] EnemiesAlive;
		public bool[] BreakablesAlive;
	}
	public class MerchantData {
		public string MerchantID;
		public int[] Stock;
	}

	public Dictionary<object, object> Save()
	{
		Dictionary<object, object> res = new Dictionary<object, object>()
		{
			{ "RespawnID", RespawnID },
			{ "PlayerHealth", PlayerHealth },
			{ "PlayerHeals", PlayerHeals },
			{ "PlayerMystic", PlayerMystic },
			{ "Inventory", inventory },
			{ "InventoryStacks", inventoryStacks },
			{ "Equipped", equipped },
			{ "Attributes", attributes }
		};

		foreach(RoomData roomData in RoomDatas) {
			res.Add("RD:" + roomData.RoomID, roomData);
		}
		foreach(MerchantData merchantData in MerchantDatas) {
			res.Add("MD:" + merchantData.MerchantID, merchantData);
		}

		return res;
	}

	public static Dictionary<object, object> GenerateFreshSave() {
		Dictionary<object, object> res = new Dictionary<object, object>()
		{
			{ "RespawnID", "WestWoods;ww1;StartingLocation" },
			{ "PlayerHealth", 50 },
			{ "PlayerHeals", 2 },
			{ "PlayerMystic", 0 },
			{ "Inventory",  "[\"sword\"]" },
			{ "InventoryStacks", "[1]" },
			{ "Equipped", new string[0] },
			{ "Attributes", "[0,0,0,0,0]" }
		};
		// GD.Print("inventory:" + res["Inventory"]);
		return res;
	}
}
