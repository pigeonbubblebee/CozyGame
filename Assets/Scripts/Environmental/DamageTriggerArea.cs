using Godot;
using System;

public partial class DamageTriggerArea : Area2D
{
	[Export] private int damage;

    public override void _Ready()
    {
        base._Ready();

		BodyEntered += _hit;
    }

	private void _hit(Node2D hit) {
		if(hit is Player) {
			((Player)hit).TakeCurseDamage(damage, 0);
			QueueFree();
		}
	}

}
