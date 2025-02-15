using Godot;
using System;

public partial class SlashAttack : EnemyAttack
{
	[Export] public float SlashRange;
	[Export] public float SlashMinRange = 0f;
	public bool CanSlash = true;

	[Export] public float SlashCooldown;
	protected bool _canHit;
	
	[Export] public float LungeSpeed;
	[Export] public float LungeRange;
	[Export] public int StopLungeFrame;
	
	[Export] public int OnFrame;
	[Export] public int OffFrame;
	
	[Export] private NodePath _attackAreaColliderPath;
	protected CollisionShape2D _attackAreaCollider;
	[Export] private NodePath _attackAreaPath;
	protected Area2D _attackArea;
	
	protected bool _accelerating;
	
	[Export] private NodePath _slashSFXPath;
	private AudioStreamPlayer2D _slashSFX;
	
	[Export] protected EnemyAttackData _attackData;
	
	public override void Initialize(Enemy e) {
		base.Initialize(e);
		EnemyAI.Sprite.FrameChanged += _checkAnimationEvent;
	}
	
	protected virtual void _checkAnimationEvent() {
		if(EnemyAI.Sprite.Frame == OnFrame && EnemyAI.Sprite.Animation == AnimationName) {
			_accelerating = true;
			// _canHit = true;
			_attackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
		}
		
		if(EnemyAI.Sprite.Frame == OffFrame && EnemyAI.Sprite.Animation == AnimationName) {
			_accelerating = false;
			// _canHit = false;
			_canHit = false;
			// _rightAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
			_attackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		}
		
		if(EnemyAI.Sprite.Frame == StopLungeFrame && EnemyAI.Sprite.Animation == AnimationName) {
			_accelerating = false;
		}
	}
	
	public override void _Ready() {
		base._Ready();
		
		_attackAreaCollider = GetNode<CollisionShape2D>(_attackAreaColliderPath);
		_attackArea = GetNode<Area2D>(_attackAreaPath);
		_slashSFX = GetNode<AudioStreamPlayer2D>(_slashSFXPath);
		
		_attackArea.BodyEntered += _OnSlashHit;
		_attackArea.CollisionMask = (uint) PhysicsLayers.PlayerLayer;
		_attackArea.CollisionLayer = (uint) PhysicsLayers.UntouchableLayer;
	}
	
	public override bool GetCondition(Player p, Enemy e) {
		return (Mathf.Abs(p.GlobalPosition.X - e.GlobalPosition.X) <= SlashRange) && CanAttack
			&& (Mathf.Abs(p.GlobalPosition.X - e.GlobalPosition.X) >= SlashMinRange);
	}
	
	public override void _Process(double delta) {
		base._Process(delta);
		
		// GD.Print("canHit?: " + _canHitSlash);
		
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
		// GD.Print(AttackLength);
		// SlashEvent?.Invoke(_playerStats.SlashDamage, _playerStats.SlashTime, _playerStats.SlashRange);		
		GetTree().CreateTimer(AttackLength).Timeout += _FinishSlash;
		// await ToSignal(GetTree().CreateTimer(SlashTime), SceneTreeTimer.SignalName.Timeout);
	}

	private void _FinishSlash() {
		GD.Print("Finish!");
		_accelerating = false;
		// _canHit = false;
		_canHit= false;
		// _rightAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		_attackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		// CanSwitchAttackDirection = true;
		Finish();
	}
	
	public override void Interrupt() {
		base.Interrupt();
		
		_accelerating = false;
		_canHit= false;
		// _canHit = false;
		// _rightAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		_attackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}
	
	protected virtual void _OnSlashHit(Node2D hit) {
		// GD.Print(hit.Name + " Hit! " + _canHit);
		if(!_canHit || !(hit is Player)) {
			return;
		}
		// _canHit = false;
		// _rightAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		_attackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		// ((Player)hit).TakeDamage(SlashDamage, PostureDamage, EnemyAI, PostureDamage);
		_canHit = false;
		((Player)hit).TakeDamage(_attackData, EnemyAI);
		
		GD.Print("SlashHIt!");
		// HitEvent?.Invoke((IHittable) hit, _playerStats.SlashDamage, this.GlobalPosition.X > hit.GlobalPosition.X ? 1 : -1, _playerStats.SlashPostureDamage);
	}
}
