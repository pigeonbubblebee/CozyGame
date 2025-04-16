using Godot;
using System.IO;
using Newtonsoft.Json;
// using Dictionary = Godot.Collections.Dictionary;
using System.Collections.Generic;
using RoomData = SaveFile.RoomData;
using MerchantData = SaveFile.MerchantData;
using System;

public partial class SaveLoader : Node
{
	public event Action LoadEvent;

	public Dictionary<object, object> CurrentSaveFile;
	readonly string PATH = ProjectSettings.GlobalizePath("user://");

	private List<RoomData> _roomDatas = new List<RoomData>();
	private List<MerchantData> _merchantDatas = new List<MerchantData>();
	private List<string> _unlockedMapMarkers = new List<string>();

	private int _saveSlot = 1;
	private int _faeSaved;

	private string fileName => "SaveSlot" + _saveSlot + ".json";

    // public override void _ExitTree()
    // {
	// 	Save();
    //     base._ExitTree();

		
    // }

	public override void _Notification(int notification) { // Handles Quit Requests to AutoSave before quitting

		if(notification == NotificationWMCloseRequest) {
			Save();

			GetTree().Quit();
		}
	}

    public void Save() {

		GD.Print("save");
		SaveFile saveFile = new SaveFile();

		// GD.Print(GetNode<MainHandler>("/root/MainHandler").GetCurrentScene());

		HandleNewRoomData(GetNode<MainHandler>("/root/MainHandler").GetCurrentScene());

		saveFile.PlayerHealth = GetNode<Player>("/root/Player").PlayerHealth.CurrentHealthPoints;
		saveFile.PlayerHeals = GetNode<Player>("/root/Player").HealController.CurrentHeals;
		saveFile.PlayerMystic = GetNode<Player>("/root/Player").CurseController.CurrentCurse;

		saveFile.hasPickedUpEquip = GetNode<Player>("/root/Player").InventoryManager.HasPickedUpEquip;

		saveFile.faeSaved = _faeSaved;
		
		saveFile.attributes = GetNode<Player>("/root/Player").SerializeCurrentBuffs();

		saveFile.RespawnID = GetNode<MainHandler>("/root/MainHandler").GetRespawnID();
		saveFile.RoomDatas = _roomDatas;

		saveFile.MerchantDatas = _merchantDatas;

		saveFile.UnlockedMapMarkers = _unlockedMapMarkers;

		saveFile.inventory = GetNode<Player>("/root/Player").InventoryManager.SerializeInventory();
		saveFile.inventoryStacks = GetNode<Player>("/root/Player").InventoryManager.SerializeInventoryStacks();

		saveFile.equipped = GetNode<Player>("/root/Player").InventoryManager.SerializeEquipped();

		SaveToFile(PATH, fileName, JsonConvert.SerializeObject(saveFile.Save()));
	}

	public void Load() {
		GD.Print("LOADING FROM:" + PATH);
		// GD.PrintErr("test");
		CurrentSaveFile = LoadFromFile(PATH, fileName);

		// foreach(object key in CurrentSaveFile.Keys) {
		// 	GD.Print(key.ToString() + " " + CurrentSaveFile[key].ToString());

		// 	// if(key.Equals("Inventory")) {
		// 	// 	GD.Print(JsonConvert.DeserializeObject<string[]>(CurrentSaveFile[key].ToString()));
		// 	// }
		// 	// GD.Print(JsonConvert.DeserializeObject<string[]>(CurrentSaveFile["Inventory"].ToString()));
		// }

		ProcessCurrentSaveFile();
		LoadEvent?.Invoke();
	}

	public void HandleNewRoomData(SceneManager sceneManager) {
		if(sceneManager == null)
			return;
		// GD.Print("QUITTING" + sceneManager.Name);
		RoomData[] temp = _roomDatas.ToArray();
		for(int i = 0; i < temp.Length; i++) {
			if(temp[i].RoomID == sceneManager.SceneID) {
				_roomDatas.Add(_UpdateRoomData(temp[i].RoomID, sceneManager));
				_roomDatas.Remove(temp[i]);
				return;
			}
		}

		_roomDatas.Add(_UpdateRoomData(sceneManager.SceneID, sceneManager));
	}

	public void ResetRoomData() {
		foreach(RoomData r in _roomDatas) {
			for(int i = 0; i < r.EnemiesAlive.Length; i++) {
				r.EnemiesAlive[i] = true;
			}
		}
	}
	
	public RoomData GetRoomData(string ID) {
		foreach(RoomData r in _roomDatas) {
			// GD.Print("Searching for Room Data: " + r.RoomID);
			if(r.RoomID == (ID))
				return r;
		}

		return null;
	}

	public string[] GetExploredRooms() {
		string[] res = new string[_roomDatas.Count];
		int i = 0;
		foreach(RoomData r in _roomDatas) {
			res[i] = r.RoomID;
			i++;
		}
		return res;
	}

	private RoomData _UpdateRoomData(string ID, SceneManager sceneManager) {
		RoomData newRoomData = new RoomData();

		newRoomData.RoomID = ID;
		newRoomData.EnemiesAlive = sceneManager.GetEnemiesAlive();
		// GD.Print("Updating data" + sceneManager.GetBreakablesAlive()[1]);
		newRoomData.BreakablesAlive = sceneManager.GetBreakablesAlive();

		return newRoomData;
	}

	public int GetStock(string MerchantID, int index, MerchantInventory merchantInventory) {
		foreach(MerchantData md in _merchantDatas) {
			if(md.MerchantID.Equals(MerchantID)) {
				return md.Stock[index];
			}
		}

		// for(int i = 0; i < merchantInventory.Inventory.Length; i++) {	
		// 	// if(merchantInventory.Inventory[i].ID.Equals(ItemID)) {
		// 	// 	return merchantInventory.Stock[i];
		// 	// }
			
		// }
		return merchantInventory.Stock[index];

		// return 0;
	}

	public void ReduceStock(string MerchantID, int index, MerchantInventory merchantInventory) {
		foreach(MerchantData md in _merchantDatas) {
			if(md.MerchantID.Equals(MerchantID)) { 
				// for(int i = 0; i < md.Stock.Length; i++) {
				// 	if(merchantInventory.Inventory[i].ID.Equals(ItemID)) {
				// 		md.Stock[i] --;
				// 		return;
				// 	}
				// }
				md.Stock[index] --;
				return;
			}
		}
		MerchantData mData = new MerchantData();
		mData.MerchantID = merchantInventory.MerchantNameCode;
		int[] newStock = new int[merchantInventory.Stock.Length];
		for(int i = 0; i < merchantInventory.Stock.Length; i++) {
			newStock[i] = merchantInventory.Stock[i];
			// if(merchantInventory.Inventory[i].ID.Equals(ItemID))
			// 	newStock[i]--;
		}
		newStock[index] --;
		mData.Stock = newStock;
		_merchantDatas.Add(mData);
	}

	public void ProcessCurrentSaveFile() {
		_roomDatas.Clear();
		foreach(object key in CurrentSaveFile.Keys) {
			char[] chars = ((string)key).ToCharArray();
			if(chars.Length > 3) {
				if(chars[0] == 'R' && chars[1] == 'D' && chars[2] == ':') {
					RoomData roomData = new RoomData();
					roomData.RoomID = ((string)key).Substring(3);
					roomData =  JsonConvert.DeserializeObject<RoomData>(CurrentSaveFile[key].ToString());
					_roomDatas.Add(roomData);

					// GD.Print(roomData.RoomID + " " + roomData.BreakablesAlive[1]);
				}
			}
		}
		_merchantDatas.Clear();
		foreach(object key in CurrentSaveFile.Keys) {
			char[] chars = ((string)key).ToCharArray();
			if(chars.Length > 3) {
				if(chars[0] == 'M' && chars[1] == 'D' && chars[2] == ':') {
					MerchantData merchantData = new MerchantData();
					merchantData.MerchantID = ((string)key).Substring(3);
					merchantData =  JsonConvert.DeserializeObject<MerchantData>(CurrentSaveFile[key].ToString());
					_merchantDatas.Add(merchantData);

					// GD.Print(roomData.RoomID + " " + roomData.BreakablesAlive[1]);
				}
			}
		}
		_unlockedMapMarkers.Clear();
		_unlockedMapMarkers = JsonConvert.DeserializeObject<List<string>>(CurrentSaveFile["MapMarkers"].ToString());

		_faeSaved = Convert.ToInt32(CurrentSaveFile["FaeSaved"]);
	}

	public void SaveFae() {
		_faeSaved ++;
	}

	public List<string> GetMapMarkers() {
		return _unlockedMapMarkers;
	}
	public void AddMapMarker(string marker) {
		// GD.Print(marker);
		foreach(string s in _unlockedMapMarkers) {
			if(s.Equals(marker))
				return;
		}
		_unlockedMapMarkers.Add(marker);
	}

	public Dictionary<object, object> LoadFromFile(string path, string fileName) {
		string loadedData = _LoadRawFromFile(path, fileName);

		if(loadedData.Equals("FRESH_SAVE")) {
			return SaveFile.GenerateFreshSave();
		}

		// Json jsonLoader = new Json();

		Dictionary<object, object> loadedDataDictionary = JsonConvert.DeserializeObject<Dictionary<object, object>>(loadedData);
		
		return loadedDataDictionary;
	}

	private string _LoadRawFromFile(string path, string fileName) {
		string data = null;

		path = Path.Join(path, fileName);

		if(!File.Exists(path)) {
			GD.Print("File doesn't exist, Generating fresh save!");
			return "FRESH_SAVE";
		}

		try {
			data = File.ReadAllText(path);
		} catch (System.Exception e) {
			GD.Print(e);
		}

		return data;
	}

	private void SaveToFile(string path, string fileName, string data) {
		if(!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
		}

		path = Path.Join(path, fileName);

		try {
			File.WriteAllText(path, data);
		} catch (System.Exception e) {
			GD.Print(e);
		}
	}
}
