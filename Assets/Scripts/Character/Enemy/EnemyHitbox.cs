using Godot;
using System;

public partial class EnemyHitbox : Area2D, IHittable
{
	public event Action<Player, int, int, int> OnHitEvent;
	public Enemy EnemyAIParent { get; private set; }
	
	public void Initialize(Enemy e) {
		EnemyAIParent = e;
	}

	public void OnHit(Player player, int damage, int direction, int postureDamage) {
		OnHitEvent?.Invoke(player, damage, direction, postureDamage);
		// GD.Print("HIT!");
	}

	public Vector2 GetSlashEffectPosition(Vector2 playerPosition) {
		return this.GlobalPosition;
	}
}
