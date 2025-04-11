using Godot;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

public partial class MainHandler : Node
{
	private SceneManager _currentScene;
	private SceneManager[] _adjacentScenes;
	private Player _player;
	private string _respawnLocationArea;
	private string _respawnLocationLevel;
	private string _respawnLocationLocation;
	
	private UIManager _uiManager;
	private Godot.Collections.Array _loadProgess;
	
	private string _currentLoadScenePath;
	
	private string _loadSpawnPosition;

	private SaveLoader _saveLoader;

	private string _currentArea;
	private string _currentLevel;

	private bool _respawnRequest = false;

	private Vector2 _chunkPosition;
	private bool _chunkRequest = false;
	private bool _adjacentRequest = false;
	
	public bool Loading { get; private set; }

	public override void _Ready() {
		GD.Randomize();


		_player = GetNode<Player>("/root/Player");
		_uiManager = GetNode<UIManager>("/root/UIManager");

		GetTree().AutoAcceptQuit = false; // Allows the game to Handle Quit Requests to autosave
		
		// _respawnLocationArea = "TutorialArea";
		// _respawnLocationLevel = "testing";
		// _respawnLocationLocation = "StartingLocation";
		_saveLoader = GetNode<SaveLoader>("/root/SaveLoader");
		_saveLoader.Load();

		string[] delimitedRespawnID = ((string)(_saveLoader.CurrentSaveFile["RespawnID"])).Split(';');
		_respawnLocationArea = delimitedRespawnID[0];
		_respawnLocationLevel = delimitedRespawnID[1];
		_respawnLocationLocation = delimitedRespawnID[2];
		
		// LoadLevel(_respawnLocationArea, _respawnLocationLevel, _respawnLocationLocation); // Change to only w fresh save
		_respawnRequest = true;
		SpawnPlayer();
		_player.PlayerHealth.DeathEvent += RespawnPlayer;
	}

	public SceneManager GetCurrentScene() {
		return _currentScene;
	}

	public void SetRespawnID(string location) {
		_respawnLocationArea = _currentArea;
		_respawnLocationLevel = _currentLevel;
		_respawnLocationLocation = location;
	}

	public string GetRespawnID() {
		return _respawnLocationArea + ';' + _respawnLocationLevel + ';' + _respawnLocationLocation;
	}
	
	public override void _Process(double delta) {
		// if(_adjacentScenes != null) {
		// int index = 0;
		// foreach(SceneManager s in _adjacentScenes) {
		// 	GD.Print(s == null ? "null" : s.SceneID + " " + index);
		// 	index++;
		// }
		// }

		ResourceLoader.ThreadLoadStatus status = ResourceLoader.LoadThreadedGetStatus(_currentLoadScenePath, _loadProgess);
		
		if(status == ResourceLoader.ThreadLoadStatus.Loaded && !_finishedLoad) {
			_finishedLoad = true;
			_FinishLoadSceneRequest();
		}
	}

	private bool _finishedLoad = false;

	private void _FinishLoadSceneRequest() {
		if(_currentScene != null && !_chunkRequest && !_adjacentRequest) {
			_currentScene.QueueFree();
			// foreach(Node n in _adjacentScenes) {
			// 	GD.Print(n == null ? "null" : ((SceneManager)n).SceneID);
			// 	if(n != _currentScene)
			// 		n.QueueFree();
			// }
		}
		if(!_adjacentRequest) {
			_saveLoader.Load();
			if(_respawnRequest) {
				_saveLoader.ResetRoomData();
				_saveLoader.CurrentSaveFile["PlayerHealth"] = _player.PlayerHealth.MaxHealthPoints;
				_saveLoader.CurrentSaveFile["PlayerMystic"] = 0;
				_saveLoader.CurrentSaveFile["PlayerHeals"] = _player.CurrentPlayerStats.MaxHeals;
			}
		}
	
		PackedScene level = (PackedScene)ResourceLoader.LoadThreadedGet(_currentLoadScenePath);
		
		SceneManager newScene = (SceneManager)level.Instantiate();
		// GD.Print(newScene.SceneID + ": loaded");
		AddChild(newScene);

		if(!_chunkRequest) {
			newScene.GlobalPosition = Vector2.Zero;
			_player.GlobalPosition = _currentScene.GetSpawnPoint(_loadSpawnPosition);
			_player.PlayerHealth.Invincible = false;
			_player.Respawn();

			GetTree().CreateTimer(0.5f).Timeout += _DisableLoadingScreen;

			_chunkRequest = false;
		} else {

			// newScene.GlobalPosition = _chunkPosition;
		}

		newScene.Visible = true;

		if(!_adjacentRequest) {
			GD.Print("Not Adj Req!");

			_player.Respawn();
			_currentScene = newScene;
			
			if(_respawnRequest)
				_player.GlobalPosition = _currentScene.GetSpawnPoint(_loadSpawnPosition);

			_LoadAdjScenes();
		} else {
			GD.Print("index: " + _adjSceneNum + " "+ _adjacentScenes.Length);
			_adjacentScenes[_adjSceneNum] = newScene;
			_adjacentRequest = false;
			_adjSceneNum ++;

			if(_adjSceneNum < _adjacentScenes.Length) {
				string currentLevel = "";
				SceneManager currentScene = null;
				if(_prevScene != null) {
					currentLevel = _prevScene.SceneID;
					currentScene = _prevScene;
				}

				_adjacentRequest = true;
				string s = _currentScene.AdjacentAreas[_adjSceneNum];

				if(s.Split(';')[1].Equals(currentLevel)) {
					if(IsInstanceValid(currentScene)) {
						_adjacentScenes[_adjSceneNum] = currentScene;
						GD.Print("exists: " + _adjacentScenes[_adjSceneNum].SceneID + " " + _adjSceneNum);
						GD.Print("arr: " + _adjacentScenes[_adjSceneNum].SceneID + " length: " + _adjacentScenes.Length + " index: " + _adjSceneNum);
						_adjSceneNum ++;
					}
				}
				if(_adjSceneNum < _adjacentScenes.Length) {
					s = _currentScene.AdjacentAreas[_adjSceneNum];
					LoadLevel(s.Split(';')[0], s.Split(';')[1]); 
				}
			}
		}

		//if(_adjSceneNum == _adjacentScenes.Length) {
			// foreach(SceneManager sceneManager in _adjacentScenes) {
			// 	if(sceneManager != null)
			// 		GD.Print("arr: " + sceneManager.SceneID + " length: " + _adjacentScenes.Length + " index: " + _adjSceneNum);
			// }
		//}

		newScene.Init();

		if(_respawnRequest) {
			_player.PlayerHealth.ResetHealth();
			_respawnRequest = false;
		}

		_uiManager.ResetUI();
	}
	
	public void SpawnPlayer() {
		// _player.PlayerHealth.Invincible = true;
		CallDeferred(MethodName.LoadLevelChunk, _respawnLocationArea, _respawnLocationLevel, _respawnLocationLocation);
	}

	public void RespawnPlayer() {
		//GD.Print("respawn Req");
		_saveLoader.Save();
		_respawnRequest = true;
		if(_currentScene != null) {
			_currentScene.QueueFree();
			if(_adjacentScenes != null) {
				foreach(Node n in _adjacentScenes) {
					if(!IsInstanceValid(n))
						continue;
					if(n != _currentScene)
						n.QueueFree();
				}
			}
		}
		_currentScene = null;
		_adjacentScenes = null;
		_adjacentRequest = false;
		_currentLevel = "";
		_currentArea = "";
		if(_prevScene!= null) {
			_prevScene.QueueFree();
			_prevScene = null;
		}
		
		
		// _chunkRequest = false;
		// _adjacentRequest = false;
		// _saveLoader.CurrentSaveFile["PlayerHealth"] = 20
		SpawnPlayer();
		// _player.PlayerHealth.Invincible = true;
		// CallDeferred(MethodName.LoadLevelChunk, _respawnLocationArea, _respawnLocationLevel, _respawnLocationLocation);
	}
	
	
	public void LoadLevel(string area, string levelName) {
		_finishedLoad = false;
		if(!_chunkRequest) {
			_uiManager.EnableLoadingScreen();
			Loading = true;
		}
		_currentLoadScenePath = "res://Assets/Scene/Levels/" + area + "/" + levelName + ".tscn";
		GD.Print(area + " " + levelName);

		if(!_adjacentRequest) {
			_currentArea = area;
			_currentLevel = levelName;
		}

		_uiManager.ResetUI();

		if(_currentScene != null) {
			_saveLoader.HandleNewRoomData(GetCurrentScene());
		}

		// PackedScene level = GD.Load<PackedScene>("res://Assets/Scene/Levels/" + area + "/" + levelName + ".tscn");
		ResourceLoader.LoadThreadedRequest(_currentLoadScenePath);
		// _loadSpawnPosition = Vector2.Zero;
	}

	private void _LoadAdjScenes() {
		// GD.Print("Loading Adjacent!");
		string currentLevel = "";
		SceneManager currentScene = null;
		if(_prevScene != null) {
			currentLevel = _prevScene.SceneID;
			currentScene = _prevScene;
		}

		if(_currentScene != null) {
			_chunkRequest = true;

			if(_adjacentScenes != null) {
				foreach(Node n in _adjacentScenes) {
					GD.Print(n == null ? "null" : ((SceneManager)n).SceneID);
					if(n != _currentScene)
						n.QueueFree();
				}
			}

			_adjacentScenes = new SceneManager[_currentScene.AdjacentAreas.Length];
			_adjSceneNum = 0;

			if(_currentScene.AdjacentAreas.Length != 0) {
				_adjacentRequest = true;
				string s = _currentScene.AdjacentAreas[0];

				if(s.Split(';')[1].Equals(currentLevel)) {
					_adjacentScenes[_adjSceneNum] = currentScene;
					GD.Print("exists: " + _adjacentScenes[_adjSceneNum].SceneID + " " + _adjSceneNum);
					GD.Print("arr: " + _adjacentScenes[_adjSceneNum].SceneID + " length: " + _adjacentScenes.Length + " index: " + _adjSceneNum);
					_adjSceneNum ++;
				}

				if(_adjSceneNum < _adjacentScenes.Length) {
					s = _currentScene.AdjacentAreas[_adjSceneNum];
					LoadLevel(s.Split(';')[0], s.Split(';')[1]);
				}
			}

			// foreach(string s in _currentScene.AdjacentAreas) {
			// 	_adjacentRequest = true;
			// 	//GD.Print(s);
			// 	GD.Print("Loading Adjacent: " + s);
			// 	if(s.Split(';')[1].Equals(currentLevel)) {
			// 		_adjacentScenes[_adjSceneNum] = currentScene;
			// 		GD.Print("exists: " + _adjacentScenes[_adjSceneNum].SceneID + " " + _adjSceneNum);
			// 		GD.Print("arr: " + _adjacentScenes[_adjSceneNum].SceneID + " length: " + _adjacentScenes.Length + " index: " + _adjSceneNum);
			// 		_adjSceneNum ++;
			// 		continue;
			// 	}
			// 	_adjacentRequest = true;

			// 	LoadLevel(s.Split(';')[0], s.Split(';')[1]);
				
			// }
		}
	}

	private SceneManager _prevScene;
	private int _adjSceneNum;
	
	public void LoadLevelChunk(string area, string levelName) {
		// QueueFreePrevScene();
		// _chunkPosition = levelPosition;
		// _chunkRequest = true;
		// _prevScene = _currentScene;
		// GetTree().CreateTimer(0.3f).Timeout += delegate { _prevScene.QueueFree(); };
		// LoadLevel(area, levelName);
		GD.Print("attempting load");
		if(levelName.Equals(_currentLevel))
			return;

		_adjSceneNum = 0;

		_chunkRequest = true;

		if(_adjacentScenes != null) {
			foreach(SceneManager s in _adjacentScenes) {
				if(s.SceneID.Equals(levelName)) {
					GD.Print("in adj! " + s.SceneID);
					_prevScene = _currentScene;
					_currentScene = s;
					_currentArea = area;
					_currentLevel = levelName;
					_LoadAdjScenes();
					return;
				}
			}
		}
		GD.Print("attempting load");
		LoadLevel(area, levelName);
	}

	public void LoadLevelChunk(string a, string l, string pos) {
		_loadSpawnPosition = pos;
		LoadLevelChunk(a, l);
		
	}

	// public void QueueFreePrevScene() {
	// 	if(_prevScene != null) {
	// 		_prevScene.QueueFree();
	// 		_prevScene = null;
	// 	}
	// }

	private void _DisableLoadingScreen() {
		Loading = false;
		_uiManager.DisableLoadingScreen();
	}
	
	public void LoadLevel(string area, string levelName, string position) {
		LoadLevel(area, levelName);
		_loadSpawnPosition = position;		
	}
}
