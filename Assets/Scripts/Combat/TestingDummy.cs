using Godot;
using System;

public partial class TestingDummy : Area2D, IHittable
{
	public void OnHit(Player player, int damage, int direction) {
		GD.Print("YEOWCH!");
	}

	public Vector2 GetSlashEffectPosition() {
		return this.GlobalPosition;
	}
}
