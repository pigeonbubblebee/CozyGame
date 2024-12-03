using Godot;
using System;

public partial class SlashAttack : EnemyAttack
{
	[Export] public float SlashRange;
	public bool CanSlash = true;
	
	[Export] public int SlashDamage;
	[Export] public float SlashCooldown;
	private bool _canHit;
	
	[Export] private NodePath _attackAreaColliderPath;
	private CollisionShape2D _attackAreaCollider;
	[Export] private NodePath _attackAreaPath;
	private Area2D _attackArea;
	
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
	
	public override void Execute(Player p, Enemy e) {
		base.Execute(p, e);

		Slash();
		StartSlashCooldown();
	}
	
	public void Slash() {
		// SlashEvent?.Invoke(_playerStats.SlashDamage, _playerStats.SlashTime, _playerStats.SlashRange);

		_canHit = true;
		_attackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
		
		GetTree().CreateTimer(AttackLength).Timeout += _FinishSlash;
		// await ToSignal(GetTree().CreateTimer(SlashTime), SceneTreeTimer.SignalName.Timeout);
	}

	private void _FinishSlash() {
		_canHit = false;
		// _rightAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		_attackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		// CanSwitchAttackDirection = true;
		Finish();
	}
	
	public void StartSlashCooldown() {
		CanSlash = false;
		GetTree().CreateTimer(SlashCooldown).Timeout += _FinishSlashCooldown;
	}

	private void _FinishSlashCooldown() {
		CanSlash = true; 
	}
	
	private void _OnSlashHit(Node2D hit) {
		GD.Print(hit.Name + " Hit!");
		if(!_canHit || !(hit is Player)) {
			return;
		}
		
		((Player)hit).PlayerHealth.TakeDamage(SlashDamage);
		// HitEvent?.Invoke((IHittable) hit, _playerStats.SlashDamage, this.GlobalPosition.X > hit.GlobalPosition.X ? 1 : -1, _playerStats.SlashPostureDamage);
	}
}
