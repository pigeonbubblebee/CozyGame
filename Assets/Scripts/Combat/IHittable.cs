using Godot;
using System;

public interface IHittable
{
	void OnHit(Player player, int damage, int direction, int postureDamage);

	Vector2 GetSlashEffectPosition(Vector2 playerPosition);
}
