using Godot;
using System;

public partial class Bullet : Area2D
{
	public bool InUse { get; private set; }
	[Export] public float Lifetime;
	[Export] public float Speed = 1600f;
	public float MaxLifetime = 2f;
	
	[Export] public NodePath SpritePath { get; private set; }
	private Node2D _sprite;
	
	[Export] public NodePath LifetimePath { get; private set; }
	private Timer _lifetime;
	
	public event Action<Node2D, Bullet> InUseAreaEntered;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = GetNode<Node2D>(SpritePath);
		_lifetime = GetNode<Timer>(LifetimePath);
		_lifetime.WaitTime = Lifetime;
		_lifetime.Timeout += ReturnToPool;
		
		InUse = false;
		AreaEntered += _AreaEntered;
		BodyEntered += _AreaEntered;
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
		_lifetime.Start();
	}

	public void ReturnToPool() {
		InUse = false;
	}
	
	private void _AreaEntered(Node2D node) {
		GD.Print(node.Name);
		if(InUse) {
			InUseAreaEntered.Invoke(node, this);
		}
	}
}
