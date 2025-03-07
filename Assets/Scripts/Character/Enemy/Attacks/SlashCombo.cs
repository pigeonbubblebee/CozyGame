using Godot;
using System;

public partial class SlashCombo : EnemyAttack
{
	[Export] public float SlashRange;
	private bool _canHit;
	
	[Export] public float LungeSpeed;
	[Export] public float LungeRange;
	[Export] public int StopLungeFrame;
	[Export] public int StopLungeFrame2;
	
	[Export] public int OnFrame;
	[Export] public int OnFrame2;
	
	[Export] public int OffFrame;
	[Export] public int OffFrame2;
	
	[Export] public int StartSoundFrame2;
	
	[Export] private NodePath _attackAreaColliderPath;
	private CollisionShape2D _attackAreaCollider;
	[Export] private NodePath _attackAreaPath;
	private Area2D _attackArea;
	
	private bool _accelerating;
	
	[Export] private NodePath _slashSFXPath;
	private AudioStreamPlayer2D _slashSFX;
	[Export] private NodePath _slash2SFXPath;
	private AudioStreamPlayer2D _slash2SFX;
	
	[Export] private EnemyAttackData _attackData;
	
	public override void Initialize(Enemy e) {
		base.Initialize(e);
		EnemyAI.Sprite.FrameChanged += _checkAnimationEvent;
	}
	
	private void _checkAnimationEvent() {
		if((EnemyAI.Sprite.Frame == OnFrame || EnemyAI.Sprite.Frame == OnFrame2) && EnemyAI.Sprite.Animation == AnimationName) {
			_accelerating = true;
			_canHit = true;
			_attackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);	
		}
		
		if((EnemyAI.Sprite.Frame == StartSoundFrame2) && EnemyAI.Sprite.Animation == AnimationName) {
			_slash2SFX.Play();
		}
		
		if((EnemyAI.Sprite.Frame == StopLungeFrame || EnemyAI.Sprite.Frame == StopLungeFrame2) && EnemyAI.Sprite.Animation == AnimationName) {
			_accelerating = false;
		}
		
		if((EnemyAI.Sprite.Frame == OffFrame || EnemyAI.Sprite.Frame == OffFrame2) && EnemyAI.Sprite.Animation == AnimationName) {
			_accelerating = false;
			_canHit = false;
			// _rightAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
			_attackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		}
	}
	
	public override void _Ready() {
		base._Ready();
		
		_attackAreaCollider = GetNode<CollisionShape2D>(_attackAreaColliderPath);
		_attackArea = GetNode<Area2D>(_attackAreaPath);
		
		_slashSFX = GetNode<AudioStreamPlayer2D>(_slashSFXPath);
		_slash2SFX = GetNode<AudioStreamPlayer2D>(_slash2SFXPath);
		
		_attackArea.BodyEntered += _OnSlashHit;
		_attackArea.CollisionMask = (uint) PhysicsLayers.PlayerLayer;
		_attackArea.CollisionLayer = (uint) PhysicsLayers.UntouchableLayer;
	}
	
	public override bool GetCondition(Player p, Enemy e) {
		return (Mathf.Abs(p.GlobalPosition.X - e.GlobalPosition.X) <= SlashRange) && CanAttack;
	}
	
	public override void _Process(double delta) {
		base._Process(delta);
		
		if(!Active)
			return;
		if(_accelerating) {
			if((Mathf.Abs(((EnemyChaserAI)EnemyAI).TargetPlayer.GlobalPosition.X - EnemyAI.GlobalPosition.X) > LungeRange)) {
				bool facingPlayer = false;
				if(((EnemyPatrolAI)EnemyAI).GetMoveDirection() > 0 && ((EnemyChaserAI)EnemyAI).TargetPlayer.GlobalPosition.X > EnemyAI.GlobalPosition.X)
					facingPlayer = true;
				else if(((EnemyPatrolAI)EnemyAI).GetMoveDirection() < 0 && ((EnemyChaserAI)EnemyAI).TargetPlayer.GlobalPosition.X < EnemyAI.GlobalPosition.X)
					facingPlayer = true;
					
				if(facingPlayer)
					((EnemyPatrolAI)EnemyAI).Accelerate(LungeSpeed);
				else
					((EnemyPatrolAI)EnemyAI).Decelerate();
			} else {
				((EnemyPatrolAI)EnemyAI).Decelerate();
			}
		} else {
			((EnemyPatrolAI)EnemyAI).Decelerate();
		}
	}
	
	public override void Execute(Player p, Enemy e) {
		base.Execute(p, e);
		_canHit = true;
		_accelerating = false;
		Slash();
	}
	
	public void Slash() {
		_slashSFX.Play();
		// SlashEvent?.Invoke(_playerStats.SlashDamage, _playerStats.SlashTime, _playerStats.SlashRange);		
		GetTree().CreateTimer(AttackLength).Timeout += _FinishSlash;
		// await ToSignal(GetTree().CreateTimer(SlashTime), SceneTreeTimer.SignalName.Timeout);
	}

	private void _FinishSlash() {
		_accelerating = false;
		_canHit = false;
		// _rightAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		_attackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		// CanSwitchAttackDirection = true;
		Finish();
	}
	
	public override void Interrupt() {
		base.Interrupt();
		
		_accelerating = false;
		_canHit = false;
		// _rightAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		_attackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}
	
	private void _OnSlashHit(Node2D hit) {
		// GD.Print(hit.Name + " Hit!");
		if(!_canHit || !(hit is Player)) {
			return;
		}
		
		_attackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		
		// ((Player)hit).TakeDamage(SlashDamage, PostureDamage, EnemyAI, PostureDamage);
		((Player)hit).TakeDamage(_attackData, EnemyAI);
		// HitEvent?.Invoke((IHittable) hit, _playerStats.SlashDamage, this.GlobalPosition.X > hit.GlobalPosition.X ? 1 : -1, _playerStats.SlashPostureDamage);
	}
}
