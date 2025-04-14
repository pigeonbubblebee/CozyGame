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

	public override void _Ready() {
		_healthSystem = GetNode<HealthSystem>(_healthSystemPath);
		_healthSystem.DeathEvent += OnDeath;
		_healthSystem.ResetHealth();

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
		_sprite.AnimationFinished += 
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