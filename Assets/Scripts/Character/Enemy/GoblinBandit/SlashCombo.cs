using Godot;
using System;

public partial class SlashCombo : EnemyAttack
{
	[Export] public float SlashRange;
	public bool CanSlash = true;
	
	[Export] public int SlashDamage;
	[Export] public int PostureDamage;
	[Export] public float SlashCooldown;
	private bool _canHit;
	
	[Export] public float LungeSpeed;
	[Export] public float LungeRange;
	[Export] public int StopLungeFrame;
	[Export] public int StopLungeFrame2;
	
	[Export] public int OnFrame;
	[Export] public int OnFrame2;
	
	[Export] public int OffFrame;
	[Export] public int OffFrame2;
	
	[Export] private NodePath _attackAreaColliderPath;
	private CollisionShape2D _attackAreaCollider;
	[Export] private NodePath _attackAreaPath;
	private Area2D _attackArea;
	
	private bool _accelerating;
	
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
		
		_attackArea.BodyEntered += _OnSlashHit;
		_attackArea.CollisionMask = (uint) PhysicsLayers.PlayerLayer;
		_attackArea.CollisionLayer = (uint) PhysicsLayers.UntouchableLayer;
		
		CanSlash = true;
	}
	
	public override bool GetCondition(Player p, Enemy e) {
		return (Mathf.Abs(p.GlobalPosition.X - e.GlobalPosition.X) <= SlashRange) && CanSlash;
	}
	
	public override void _Process(double delta) {
		base._Process(delta);
		
		if(!Active)
			return;
		if(_accelerating) {
			if((Mathf.Abs(((EnemySwordsmanAI)EnemyAI).TargetPlayer.GlobalPosition.X - EnemyAI.GlobalPosition.X) > LungeRange)) {
				bool facingPlayer = false;
				if(((EnemyPatrolAI)EnemyAI).GetMoveDirection() > 0 && ((EnemySwordsmanAI)EnemyAI).TargetPlayer.GlobalPosition.X > EnemyAI.GlobalPosition.X)
					facingPlayer = true;
				else if(((EnemyPatrolAI)EnemyAI).GetMoveDirection() < 0 && ((EnemySwordsmanAI)EnemyAI).TargetPlayer.GlobalPosition.X < EnemyAI.GlobalPosition.X)
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

		_accelerating = false;
		Slash();
		StartSlashCooldown();
	}
	
	public void Slash() {
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
	
	public void StartSlashCooldown() {
		CanSlash = false;
		GetTree().CreateTimer(SlashCooldown).Timeout += _FinishSlashCooldown;
	}

	private void _FinishSlashCooldown() {
		CanSlash = true; 
	}
	
	private void _OnSlashHit(Node2D hit) {
		// GD.Print(hit.Name + " Hit!");
		if(!_canHit || !(hit is Player)) {
			return;
		}
		
		_attackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		
		((Player)hit).TakeDamage(SlashDamage, PostureDamage, EnemyAI);
		// HitEvent?.Invoke((IHittable) hit, _playerStats.SlashDamage, this.GlobalPosition.X > hit.GlobalPosition.X ? 1 : -1, _playerStats.SlashPostureDamage);
	}
}
