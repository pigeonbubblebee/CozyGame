using Godot;
using System;

public partial class SceneManager : Node2D
{
	// [Export] public Player ScenePlayer;
	[Export] public string[] AdjacentAreas; // semicolon delimited
	[Export] private NodePath _respawnPointsParentPath;
	private Node2D _respawnPointsParent;
	private Node2D[] _respawnPoints;
	
	[Export]
	private NodePath _enemiesParentPath;
	private Node2D _enemiesParent;
	private Enemy[] _enemies;

	[Export]
	private NodePath _breakablesPath;
	private Node2D _breakablesParent;
	private Node2D[] _breakables;
	
	private bool _initialized = false;

	[Export]
	public string SceneID;
	
	public override void _Ready() {
		
		if(!_initialized) {
			Init();
		}
	}
	
	public void Init() {
		_initialized = true;
		
		_respawnPointsParent = GetNode<Node2D>(_respawnPointsParentPath);
		_respawnPoints = new Node2D[_respawnPointsParent.GetChildren().Count];
		int i = 0;
		
		foreach(Node n in _respawnPointsParent.GetChildren()) {
			_respawnPoints[i] = (Node2D) n;
			i ++;
		}
		
		_enemiesParent = GetNode<Node2D>(_enemiesParentPath);
		_enemies = new Enemy[_enemiesParent.GetChildren().Count];
		i = 0;

		_breakablesParent = GetNode<Node2D>(_breakablesPath);
		_breakables = new Node2D[_breakablesParent.GetChildren().Count];

		SaveFile.RoomData loadedRoomDataFromSave = (GetNode<SaveLoader>("/root/SaveLoader").GetRoomData(this.SceneID));
		// GD.Print(this.SceneID);

		foreach(Node n in _enemiesParent.GetChildren()) {
			if(!(n is Enemy)) {
				foreach(Node b in n.GetChildren()) {
					if(b is Enemy)
						_enemies[i] = (Enemy) b;
				}
			} else {
				_enemies[i] = (Enemy) n;
			}
			// GD.Print(loadedRoomDataFromSave.RoomID + " from scene manager");
			if(loadedRoomDataFromSave == null) {
				_enemies[i].DeathEvent += _CleanupEnemy;
			} else {
				if(i < loadedRoomDataFromSave.EnemiesAlive.Length) {
					if(loadedRoomDataFromSave.EnemiesAlive[i])
						_enemies[i].DeathEvent += _CleanupEnemy;
					else {
						_enemies[i].QueueFree();
						_enemies[i] = null;
					}
				}
			}
			i ++;
		}

		i = 0;

		foreach(Node n in _breakablesParent.GetChildren()) {
			
			_breakables[i] = (Node2D) n;
			// GD.Print("breakable" + _breakables[i] +" " + _breakables.Length);
			// GD.Print(loadedRoomDataFromSave.RoomID + " from scene manager");
			if(loadedRoomDataFromSave == null) {
				// _breakables[i].TreeExited += _CleanupBreakables;
			} else {
				if(i < loadedRoomDataFromSave.BreakablesAlive.Length) {
					if(loadedRoomDataFromSave.BreakablesAlive[i]) {
						// _breakables[i].TreeExited += _CleanupBreakables;
					} else {
						_breakables[i].QueueFree();
						_breakables[i] = null;
					}
				}
			}
			i ++;
		}
	}

	public bool[] GetBreakablesAlive() {
		bool[] res = new bool[_breakables.Length];

		for(int i = 0; i < res.Length; i++) {
			res[i] = !(_breakables[i] is null);
			// GD.Print(res[i]);
		}

		return res;
	}

	public bool[] GetEnemiesAlive() {
		bool[] res = new bool[_enemies.Length];

		for(int i = 0; i < res.Length; i++) {
			res[i] = !(_enemies[i] is null);
			// GD.Print(res[i]);
		}

		return res;
	}

	public override void _Process(double delta) {
		// _CleanupEnemy();
		_CleanupBreakables();
	}
	
	private void _CleanupBreakables() {
		int i = 0;
		foreach(Node2D x in _breakables) {
			if(!IsInstanceValid(x)) {
				_breakables[i] = null;
			}
			i++;
		}
	}

	private void _CleanupEnemy(Enemy e) {
		int i = 0;
		foreach(Enemy x in _enemies) {
			if(e == x) {
				_enemies[i] = null;
				// GD.Print("DeadEnemy!");
			}
			i++;
		}
		// GD.PrintRaw("\n");
	}

	public Vector2 GetSpawnPoint(string point) {
		foreach(Node2D n in _respawnPoints) {
			if(n.Name == point) {
				return n.GlobalPosition;
			}
		}
		GD.Print("Cannot find specified point!");
		return Vector2.Zero;
	}
}
