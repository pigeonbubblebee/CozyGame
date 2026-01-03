using Godot;
using System;

public partial class DashSummon : Summon
{
	[Export] private float _attackCooldown = 1f;
	[Export] private float _dashSpeed;
	[Export] private float _dashLen = 0.4f;
	private bool _CanAttack = true;

	[Export] private NodePath _attackAreaPath;
	private Area2D _attackArea;

	[Export] private NodePath _attackColliderAreaPath;
	private CollisionShape2D _attackColliderArea;

	private Vector2 _dashDir;
	[Export] private int _damage;
	[Export] private NodePath _hitParticlePath;
	private GpuParticles2D _hitParticle;
	[Export] private NodePath _hitSFXPath;
	private AudioStreamPlayer2D _hitSFX;

    public override void _Ready()
    {
        base._Ready();
		_attackArea = GetNode<Area2D>(_attackAreaPath);
		_attackColliderArea = GetNode<CollisionShape2D>(_attackColliderAreaPath);
		_attackColliderArea.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		_attackArea.AreaEntered += _OnHit;

		_hitParticle = GetNode<GpuParticles2D>(_hitParticlePath);
		_hitSFX = GetNode<AudioStreamPlayer2D>(_hitSFXPath);
    }


    public override void _Process(double delta)
    {
		if(!IsInstanceValid(_target)) {
			_target = null;
		}
		if(_CanAttack && _target != null) {
			_CanAttack = false;
			attacking = true;

			Vector2 direction = (_target.GlobalPosition - GlobalPosition).Normalized();
			_dashDir = direction * _dashSpeed;

			_attackColliderArea.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);

			GetTree().CreateTimer(_attackCooldown).Timeout += delegate { _CanAttack = true; attacking = false; };
			GetTree().CreateTimer(_dashLen).Timeout += delegate { attacking = false; _attackColliderArea.SetDeferred(CollisionShape2D.PropertyName.Disabled, true); };
		}

		if(attacking) {
			Velocity = _dashDir;
			MoveAndSlide();
		}
        base._Process(delta);
    }

	private void _OnHit(Node2D hit) {
		// GD.Print("summon hit");
		if(hit is EnemyHitbox) {
			// GD.Print("summon hit");
			_player.Camera.Shake(0.05f, 800f);
			_hitSFX.Play();

			Enemy e = ((EnemyHitbox)hit).EnemyAIParent;
			_attackColliderArea.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
			_hitParticle.GlobalPosition = e.GlobalPosition;
			_hitParticle.Emitting = true;

			_OnHitEffect(e);
			_InvokeOnHitEvent(e, _damage);

			e.OnHit(_player, _damage + _player.CurrentBuffs.Harmony, this.GlobalPosition.X > e.GlobalPosition.X ? 1 : -1, _damage + _player.CurrentBuffs.Harmony);
		}
	}

	protected virtual void _OnHitEffect(Enemy e)
	{
		
	}
}
