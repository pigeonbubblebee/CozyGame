using Godot;
using System;
using System.Collections.Generic;

public partial class Cannon : Node2D
{
	private PackedScene _bulletScene;
	[Export] private string _sceneName;

	[Export] private NodePath _targetPostionPath;
	private Vector2 targetPosition;
	[Export] private EnemyAttackData _attackData;
	
	private List<Bullet> bullets = new List<Bullet>();
	
	[Export] private NodePath _objectPoolPath;
	public Node ObjectPool { get; private set; }

	[Export] private float _shotIntervalTime = 1f;
	private float timer = 0;

	[Export] private NodePath _spritePath;
	private AnimatedSprite2D _sprite;

	private string _bulletPath => "res://Assets/Scene/Environment/Projectile/" + _sceneName + ".tscn";
	public override void _Ready()
	{
		targetPosition = GetNode<Node2D>(_targetPostionPath).GlobalPosition;
		_sprite = GetNode<AnimatedSprite2D>(_spritePath);

		_sprite.Play("idle");

		ObjectPool = GetNode<Node>(_objectPoolPath);
		_bulletScene = ResourceLoader.Load<PackedScene>(_bulletPath);

		int objPoolAmt = 3;
		
		for(int i = 0; i < objPoolAmt; i++) {
			InstantiateBullet();
		}
	}

    public override void _Process(double delta)
    {
        base._Process(delta);

		timer += (float)delta;

		if(timer >= _shotIntervalTime) {
			FireCannon();
			timer = 0;
		}
    }


	public void FireCannon() {
		_sprite.Play("fire");

		foreach(Bullet b in bullets) {
			if(!b.InUse) {
				b.GlobalPosition = this.GlobalPosition;
				
				b.LookAt(targetPosition);
				// GD.Print(Mathf.RadToDeg(b.GlobalRotation));
				
				b.Fire();
				
				break;
			}
		}
	}

	private void InstantiateBullet() {
		Bullet bullet = (Bullet)_bulletScene.Instantiate();
		ObjectPool.AddChild(bullet);
		bullet.Position = this.GlobalPosition;
		bullets.Add(bullet);
		
		bullet.InUseAreaEntered += _OnBulletHit;
		bullet.CollisionMask = (uint) PhysicsLayers.PlayerLayer;
		bullet.CollisionLayer = (uint) PhysicsLayers.UntouchableLayer;
	}

	private void _OnBulletHit(Node2D hit, Bullet b) {
		if(hit is Player || hit.IsInGroup("Ground")) {
			b.ReturnToPool();	
		}
		
		if(!(hit is Player)) {
			return;
		}

		((Player)hit).TakeObstacleDamage(_attackData, this, 3f);
	}
}
