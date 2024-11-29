using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] public NodePath HitboxPath { get; private set; }
	private EnemyHitbox _hitbox;
	[Export] public NodePath HealthSystemPath { get; private set; }
	private HealthSystem _health;
	
	public readonly float GRAVITY = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_hitbox = GetNode<EnemyHitbox>(HitboxPath);
		_hitbox.OnHitEvent += OnHit;
		
		_health = GetNode<HealthSystem>(HealthSystemPath);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if(!IsOnFloor()) {
			Vector2 velocity = Velocity;
			velocity.Y += GRAVITY * (float) delta * 60f;

			Velocity = velocity;
		}
	}
	
	public virtual void OnHit(Player player, int damage, int direction) {
		GD.Print("OUCH!");
		_health.TakeDamage(damage);
	}
}
