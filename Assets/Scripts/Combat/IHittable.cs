using Godot;
using System;

public interface IHittable
{
	void OnHit(Player player, int damage, int direction);

	Vector2 GetSlashEffectPosition();
}
