using Godot;
using System;

public partial class BreakableWall : StaticBody2D, IHittable
{
	public event Action<BreakableWall> BreakableWallBreakEvent;
	[Export] private NodePath _healthSystemPath;
	private HealthSystem _healthSystem;

	[Export] private bool _oneWay;

	public override void _Ready() {
		_healthSystem = GetNode<HealthSystem>(_healthSystemPath);
		_healthSystem.DeathEvent += OnDeath;
		_healthSystem.ResetHealth();
	}

	public void OnHit(Player player, int damage, int direction, int postureDamage) {
		// GD.Print("direction" + direction + "scale" + this.Scale.X);
		if(_oneWay) {
			if(direction == 1 && this.Scale.X < 0) {
				return;
			}
			if(direction == -1 && this.Scale.X > 0) {
				return;
			}
		}
		_healthSystem.TakeDamage(damage);
		return; // TODO: implement taking damage
	}

	public Vector2 GetSlashEffectPosition(Vector2 playerPos) {
		return GlobalPosition; // TODO: implement taking damage
	}

	public virtual void OnDeath() {
		BreakableWallBreakEvent?.Invoke(this);
		QueueFree();
	}
}