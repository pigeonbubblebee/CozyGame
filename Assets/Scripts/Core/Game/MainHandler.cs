using Godot;
using System;

public partial class MainHandler : Node
{
	private SceneManager _currentScene;
	
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
		ResourceLoader.ThreadLoadStatus status = ResourceLoader.LoadThreadedGetStatus(_currentLoadScenePath, _loadProgess);
		
		if(status == ResourceLoader.ThreadLoadStatus.Loaded && !_finishedLoad) {
			_finishedLoad = true;
			_FinishLoadSceneRequest();
		}
	}

	private bool _finishedLoad = false;

	private void _FinishLoadSceneRequest() {
		if(_currentScene != null) {
			_currentScene.QueueFree();
		}

		_saveLoader.Load();

		if(_respawnRequest) {
			_saveLoader.ResetRoomData();
			_saveLoader.CurrentSaveFile["PlayerHealth"] = _player.PlayerHealth.MaxHealthPoints;
			_saveLoader.CurrentSaveFile["PlayerMystic"] = _player.CurrentPlayerStats.MaxCurse;
			_saveLoader.CurrentSaveFile["PlayerHeals"] = _player.CurrentPlayerStats.MaxHeals;

			_respawnRequest = false;
		}
		
		PackedScene level = (PackedScene)ResourceLoader.LoadThreadedGet(_currentLoadScenePath);
		
		SceneManager newScene = (SceneManager)level.Instantiate();
		// GD.Print(newScene.SceneID + ": loaded");
		AddChild(newScene);
		newScene.GlobalPosition = Vector2.Zero;
		newScene.Visible = true;
		
		_currentScene = newScene;

		newScene.Init();
		_player.PlayerHealth.Invincible = false;
		_player.Respawn();

		_player.GlobalPosition = _currentScene.GetSpawnPoint(_loadSpawnPosition);
		_uiManager.ResetUI();
		GetTree().CreateTimer(0.5f).Timeout += _DisableLoadingScreen;	
	}
	
	public void SpawnPlayer() {
		_player.PlayerHealth.Invincible = true;
		CallDeferred(MethodName.LoadLevel, _respawnLocationArea, _respawnLocationLevel, _respawnLocationLocation);
	}

	public void RespawnPlayer() {
		GD.Print("respawn Req");
		_saveLoader.Save();
		_respawnRequest = true;
		// _saveLoader.CurrentSaveFile["PlayerHealth"] = 20
		SpawnPlayer();
		
	}
	
	public void LoadLevel(string area, string levelName) {
		_finishedLoad = false;
		_uiManager.EnableLoadingScreen();
		_currentLoadScenePath = "res://Assets/Scene/Levels/" + area + "/" + levelName + ".tscn";

		_currentArea = area;
		_currentLevel = levelName;

		_uiManager.ResetUI();

		if(_currentScene != null) {
			_saveLoader.HandleNewRoomData(GetCurrentScene());
		}

		// PackedScene level = GD.Load<PackedScene>("res://Assets/Scene/Levels/" + area + "/" + levelName + ".tscn");
		ResourceLoader.LoadThreadedRequest(_currentLoadScenePath);
		// _loadSpawnPosition = Vector2.Zero;
	}
	
	private void _DisableLoadingScreen() {
		_uiManager.DisableLoadingScreen();
	}
	
	public void LoadLevel(string area, string levelName, string position) {
		LoadLevel(area, levelName);
		_loadSpawnPosition = position;		
	}
}
