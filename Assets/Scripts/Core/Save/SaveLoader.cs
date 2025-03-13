using Godot;
using System.IO;
using Newtonsoft.Json;
// using Dictionary = Godot.Collections.Dictionary;
using System.Collections.Generic;
using RoomData = SaveFile.RoomData;
using System;

public partial class SaveLoader : Node
{
	public event Action LoadEvent;

	public Dictionary<object, object> CurrentSaveFile;
	readonly string PATH = ProjectSettings.GlobalizePath("user://");

	private List<RoomData> _roomDatas = new List<RoomData>();

	private int _saveSlot = 1;

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
		SaveFile saveFile = new SaveFile();

		// GD.Print(GetNode<MainHandler>("/root/MainHandler").GetCurrentScene());

		saveFile.PlayerHealth = GetNode<Player>("/root/Player").PlayerHealth.CurrentHealthPoints;
		saveFile.RespawnID = GetNode<MainHandler>("/root/MainHandler").GetRespawnID();
		saveFile.RoomDatas = _roomDatas;

		SaveToFile(PATH, fileName, JsonConvert.SerializeObject(saveFile.Save()));
	}

	public void Load() {
		GD.Print("LOADING FROM:" + PATH);
		CurrentSaveFile = LoadFromFile(PATH, fileName);
		ProcessCurrentSaveFile();
		LoadEvent?.Invoke();
	}

	public void HandleNewRoomData(SceneManager sceneManager) {
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
			GD.Print("Searching for Room Data: " + r.RoomID);
			if(r.RoomID == (ID))
				return r;
		}

		return null;
	}

	private RoomData _UpdateRoomData(string ID, SceneManager sceneManager) {
		RoomData newRoomData = new RoomData();

		newRoomData.RoomID = ID;
		newRoomData.EnemiesAlive = sceneManager.GetEnemiesAlive();

		return newRoomData;
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

					GD.Print(roomData.RoomID + " " + roomData.EnemiesAlive[4]);
				}
			}
		}
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
