using Godot;
using System;

public partial class SceneManager : Node2D
{
	// [Export] public Player ScenePlayer;
	[Export] private NodePath _respawnPointsParentPath;
	private Node2D _respawnPointsParent;
	private Node2D[] _respawnPoints;
	
	private bool _initialized = false;
	
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
