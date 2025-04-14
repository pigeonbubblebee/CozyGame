using Godot;
using System;

public partial class HiddenArea : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += _OnPlayerEnter;
		BodyExited += _OnPlayerExit;
	}

	private void _OnPlayerEnter(Node2D hit) {
		if(hit is Player) {
			Tween tween = CreateTween();
			tween.TweenProperty(this, "modulate:a", 0, 0.5f);
		}
	}
	private void _OnPlayerExit(Node2D hit) {
		if(hit is Player) {
			Tween tween = CreateTween();
			tween.TweenProperty(this, "modulate:a", 1, 0.5f);
		}
	}
}
