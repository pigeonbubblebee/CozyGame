using Godot;
using System;

public partial class Bullet : Node2D
{
	public bool InUse { get; private set; }
	[Export] public float Lifetime;
	public float Speed = 1600f;
	public float MaxLifetime = 2f;
	[Export] public NodePath SpritePath { get; private set; }
	private Node2D _sprite;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = GetNode<Node2D>(SpritePath);
		InUse = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		this.Visible = InUse;
		
		if(!InUse) {
			return;
		}
		
		Vector2 direction = Vector2.Right.Rotated(Rotation);
		GlobalPosition += Speed * direction * (float)delta;
	}
	
	 public void Fire() {
		InUse = true;
		GetTree().CreateTimer(MaxLifetime).Timeout += _ReturnToPool;
		_sprite.GlobalRotation = Mathf.DegToRad(0);
		if(Mathf.RadToDeg(this.GlobalRotation) > 90) {
			_sprite.GlobalRotation = Mathf.DegToRad(180) - this.GlobalRotation;
			// GD.Print(_sprite.GlobalRotation);
		}
	}

	private void _ReturnToPool() {
		InUse = false;
	}
}
