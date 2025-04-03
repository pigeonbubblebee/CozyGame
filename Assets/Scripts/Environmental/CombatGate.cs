using Godot;
using System;

public partial class CombatGate : StaticBody2D
{
	[Export] private NodePath[] enemyPaths;
	private Enemy[] _enemies;
	int totalDeath = 0;
    public override void _Ready()
    {
        base._Ready();

		totalDeath = 0;

		_enemies = new Enemy[enemyPaths.Length];

		for(int i = 0; i < enemyPaths.Length; i++) {
			if(!(GetNode(enemyPaths[i]) is Enemy)) {
				foreach(Node b in GetNode(enemyPaths[i]).GetChildren()) {
					if(b is Enemy)
						_enemies[i] = (Enemy) b;
				}
			} else {
				_enemies[i] = (Enemy) GetNode(enemyPaths[i]);
			}
			_enemies[i].DeathEvent += _Cleanup;
		}
    }

	private void _Cleanup(Enemy e) {
		totalDeath ++;

		if(totalDeath == _enemies.Length) {
			QueueFree();
		}
	}
}
