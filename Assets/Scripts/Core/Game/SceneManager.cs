using Godot;
using System;

public partial class SceneManager : Node2D
{
	// [Export] public Player ScenePlayer;
	[Export] private NodePath _respawnPointsParentPath;
	private Node2D _respawnPointsParent;
	private Node2D[] _respawnPoints;
	
	[Export]
	private NodePath _enemiesParentPath;
	private Node2D _enemiesParent;
	private Enemy[] _enemies;
	
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
				if(loadedRoomDataFromSave.EnemiesAlive[i])
					_enemies[i].DeathEvent += _CleanupEnemy;
				else {
					_enemies[i].QueueFree();
					_enemies[i] = null;
				}
			}
			i ++;
		}
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
