using Godot;
using System;

public partial class CameraBound : Area2D
{
	// Called when the node enters the scene tree for the first time.
	[Export] private NodePath _shapePath;
	private CollisionShape2D _shape;
	public override void _Ready()
	{
		// this.BodyEntered += _CheckPlayer;

		_shape = GetNode<CollisionShape2D>(_shapePath);
	}

	private void _CheckPlayer(Node2D hit) {
		if(hit is Player) {
			if(_shape == null)
				_shape = GetNode<CollisionShape2D>(_shapePath);
			Player p = (Player)hit;

			p.Camera.LimitLeft = (int) p.Camera.GlobalPosition.X - (2560/2);
			p.Camera.LimitRight = (int) p.Camera.GlobalPosition.X + (2560/2);

			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(p.Camera, "limit_left", (int)(_shape.GlobalPosition.X - (((RectangleShape2D)_shape.Shape).Size.X / 2)), 0.25f);
			Tween tween2 = GetTree().CreateTween();
			tween2.TweenProperty(p.Camera, "limit_right", (int)(_shape.GlobalPosition.X + (((RectangleShape2D)_shape.Shape).Size.X / 2)), 0.25f);
			//p.Camera.LimitLeft = (int)(_shape.GlobalPosition.X - (((RectangleShape2D)_shape.Shape).Size.X / 2));
			//p.Camera.LimitRight = (int)(_shape.GlobalPosition.X + (((RectangleShape2D)_shape.Shape).Size.X / 2));
			p.Camera.LimitTop = (int) p.Camera.GlobalPosition.Y - (1440/2);
			p.Camera.LimitBottom = (int) p.Camera.GlobalPosition.Y + (1440/2);

			Tween tween3 = GetTree().CreateTween();
			tween3.TweenProperty(p.Camera, "limit_top", (int)(_shape.GlobalPosition.Y - (((RectangleShape2D)_shape.Shape).Size.Y / 2)), 0.25f);
			Tween tween4 = GetTree().CreateTween();
			tween4.TweenProperty(p.Camera, "limit_bottom", (int)(_shape.GlobalPosition.Y + (((RectangleShape2D)_shape.Shape).Size.Y / 2)), 0.25f);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
