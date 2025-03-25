using Godot;
using System;
using System.Collections.Generic;

using Dictionary = Godot.Collections.Dictionary;
using Array = Godot.Collections.Array;

public class SaveFile
{
	public int PlayerHealth { get; set; }
	public List<RoomData> RoomDatas { get; set; } = new List<RoomData>();
	public string RespawnID; // Delimited by ;
	public List<string> inventory = new List<string>();
	
	public class RoomData {
		public string RoomID;
		public bool[] EnemiesAlive;
		public bool[] BreakablesAlive;
	}

	public Dictionary<object, object> Save()
	{
		Dictionary<object, object> res = new Dictionary<object, object>()
		{
			{ "RespawnID", RespawnID },
			{ "PlayerHealth", PlayerHealth },
			{ "Inventory", inventory }
		};

		foreach(RoomData roomData in RoomDatas) {
			res.Add("RD:" + roomData.RoomID, roomData);
		}

		return res;
	}

	public static Dictionary<object, object> GenerateFreshSave() {

		Dictionary<object, object> res = new Dictionary<object, object>()
		{
			{ "RespawnID", "TutorialArea;testing;StartingLocation" },
			{ "PlayerHealth", 50 }
		};

		return res;
	}
}
