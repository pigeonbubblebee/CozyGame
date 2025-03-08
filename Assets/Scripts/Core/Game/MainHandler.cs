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
	
	public override void _Ready() {
		GD.Randomize();
		_player = GetNode<Player>("/root/Player");
		_uiManager = GetNode<UIManager>("/root/UIManager");
		
		_respawnLocationArea = "TutorialArea";
		_respawnLocationLevel = "testing";
		_respawnLocationLocation = "StartingLocation";
		
		// LoadLevel(_respawnLocationArea, _respawnLocationLevel, _respawnLocationLocation); // Change to only w fresh save
		RespawnPlayer();
		_player.PlayerHealth.DeathEvent += RespawnPlayer;
	}
	
	public override void _Process(double delta) {
		ResourceLoader.ThreadLoadStatus status = ResourceLoader.LoadThreadedGetStatus(_currentLoadScenePath, _loadProgess);
		
		if(status == ResourceLoader.ThreadLoadStatus.Loaded) {
			if(_currentScene != null) {
				_currentScene.QueueFree();
			}
			
			PackedScene level = (PackedScene)ResourceLoader.LoadThreadedGet(_currentLoadScenePath);
			
			SceneManager newScene = (SceneManager)level.Instantiate();
			GD.Print(newScene.Name);
			AddChild(newScene);
			newScene.GlobalPosition = Vector2.Zero;
			newScene.Init();
			newScene.Visible = true;
			
			_currentScene = newScene;
			_player.GlobalPosition = _currentScene.GetSpawnPoint(_loadSpawnPosition);
			GetTree().CreateTimer(0.5f).Timeout += _DisableLoadingScreen;	
		}
	}
	
	public void RespawnPlayer() {
		CallDeferred(MethodName.LoadLevel, _respawnLocationArea, _respawnLocationLevel, _respawnLocationLocation);
		_player.Respawn();
	}
	
	public void LoadLevel(string area, string levelName) {
		_uiManager.EnableLoadingScreen();
		_currentLoadScenePath = "res://Assets/Scene/Levels/" + area + "/" + levelName + ".tscn";
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
