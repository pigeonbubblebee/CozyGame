using Godot;
using System;

public partial class TrappedCollectible : Area2D, IHittable
{
	[Export] private NodePath _healthSystemPath;
	private HealthSystem _healthSystem;

	[Export] private NodePath _spritePath;
	private AnimatedSprite2D _sprite;

	[Export] private NodePath _playerDetectPath;
	private Area2D _playerDetect;
	[Export] private NodePath _hitboxPath;
	private CollisionShape2D _hitbox;


	public override void _Ready() {
		_healthSystem = GetNode<HealthSystem>(_healthSystemPath);
		_healthSystem.DeathEvent += OnDeath;
		_healthSystem.ResetHealth();

		_hitbox = GetNode<CollisionShape2D>(_hitboxPath);

		this.Monitorable = true;
		this.Monitoring = true;

		_sprite = GetNode<AnimatedSprite2D>(_spritePath);
		_sprite.Play("sad");

		_playerDetect = GetNode<Area2D>(_playerDetectPath);
		_playerDetect.BodyEntered += _playHappy;
		_playerDetect.BodyExited += _playSad;
	}

	public void OnHit(Player player, int damage, int direction, int postureDamage) {
		// GD.Print("direction" + direction + "scale" + this.Scale.X);
		_healthSystem.TakeDamage(damage);
		return; // TODO: implement taking damage
	}

	public Vector2 GetSlashEffectPosition(Vector2 playerPos) {
		return GlobalPosition; // TODO: implement taking damage
	}

	public virtual void OnDeath() {
		_sprite.Play("free");
		_hitbox.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		GetTree().CreateTimer(1.91666667f).Timeout += 
			delegate { 
				GetNode<SaveLoader>("/root/SaveLoader").SaveFae();
				QueueFree(); 
			}; 
	}

	private void _playHappy(Node2D hit) {
		if(hit is Player)
			_sprite.Play("happy");
	}
	private void _playSad(Node2D hit) {
		if(hit is Player)
			_sprite.Play("sad");
	}
}