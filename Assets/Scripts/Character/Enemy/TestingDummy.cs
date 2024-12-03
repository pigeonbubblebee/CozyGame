using Godot;
using System;

public partial class TestingDummy : Area2D, IHittable
{
	public void OnHit(Player player, int damage, int direction, int postureDamage) {
		GD.Print("YEOWCH!");
	}

	public Vector2 GetSlashEffectPosition(Vector2 playerPosition) {
		return this.GlobalPosition;
	}
}
