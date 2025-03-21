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

	public string GetRespawnID() {
		return _respawnLocationArea + ';' + _respawnLocationLevel + ';' + _respawnLocationLocation;
	}
	
	public override void _Process(double delta) {
		ResourceLoader.ThreadLoadStatus status = ResourceLoader.LoadThreadedGetStatus(_currentLoadScenePath, _loadProgess);
		
		if(status == ResourceLoader.ThreadLoadStatus.Loaded) {
			_FinishLoadSceneRequest();
		}
	}

	private void _FinishLoadSceneRequest() {
		if(_currentScene != null) {
			_currentScene.QueueFree();
		}

		_saveLoader.Load();

		if(_respawnRequest) {
			_saveLoader.ResetRoomData();
			_saveLoader.CurrentSaveFile["PlayerHealth"] = _player.PlayerHealth.MaxHealthPoints;

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

		_player.Respawn();

		_player.GlobalPosition = _currentScene.GetSpawnPoint(_loadSpawnPosition);
		_uiManager.ResetUI();
		GetTree().CreateTimer(0.5f).Timeout += _DisableLoadingScreen;	
	}
	
	public void SpawnPlayer() {
		CallDeferred(MethodName.LoadLevel, _respawnLocationArea, _respawnLocationLevel, _respawnLocationLocation);
	}

	public void RespawnPlayer() {
		// _saveLoader.Save();
		_respawnRequest = true;
		// _saveLoader.CurrentSaveFile["PlayerHealth"] = 20
		SpawnPlayer();
		
	}
	
	public void LoadLevel(string area, string levelName) {
		_uiManager.EnableLoadingScreen();
		_currentLoadScenePath = "res://Assets/Scene/Levels/" + area + "/" + levelName + ".tscn";

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
