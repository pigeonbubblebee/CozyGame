using Godot;
using System;

public partial class MoneyPile : Area2D, IHittable
{
	[Export] private NodePath _healthSystemPath;
	private HealthSystem _healthSystem;

	[Export] private int _amt;
	[Export] private Item coin_resource;

	public override void _Ready() {
		_healthSystem = GetNode<HealthSystem>(_healthSystemPath);
		_healthSystem.DeathEvent += OnDeath;
		_healthSystem.ResetHealth();
	}

	public void OnHit(Player player, int damage, int direction, int postureDamage) {
		// GD.Print("direction" + direction + "scale" + this.Scale.X);
		_healthSystem.TakeDamage(damage);
		return; // TODO: implement taking damage
	}

	public Vector2 GetSlashEffectPosition(Vector2 playerPos) {
		return GlobalPosition; // TODO: implement taking damage
	}

	public virtual void OnDeath() {
		GetNode<Player>("/root/Player").InventoryManager.AddItemToInventory(coin_resource, _amt);
		QueueFree();
	}
}