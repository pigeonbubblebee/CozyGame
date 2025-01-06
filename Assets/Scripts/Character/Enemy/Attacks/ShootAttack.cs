using Godot;
using System.Collections.Generic;
using System;

public partial class ShootAttack : EnemyAttack
{	
	// [Export] public float ShootStartRange;
	
	[Export] public float ShootRange;
	[Export] public float ChargeTime;
	[Export] public float ShootStartRange;
	
	public bool CanShoot = true;
	[Export] public float ShootCooldown;
	
	private PackedScene _bulletScene;
	[Export] private string _sceneName;
	
	private List<Bullet> bullets = new List<Bullet>();
	
	[Export] private NodePath _objectPoolPath;
	public Node ObjectPool { get; private set; }
	
	[Export] private NodePath _bodyTopPath;
	private Node2D _bodyTop;
	
	private string _bulletPath => "res://Assets/Scene/Enemy/Projectile/" + _sceneName + ".tscn";
	
	[Export] private int _objectPoolNumber;
	
	private Vector2 _playerPosition;
	private Player _aimedPlayer;
	
	private bool _charging;
	
	[Export] private EnemyAttackData _attackData;
	
	
	public override void _Ready() {
		base._Ready();
		CanShoot = true;
		
		ObjectPool = GetNode<Node>(_objectPoolPath);
		_bulletScene = GD.Load<PackedScene>(_bulletPath);
		
		for(int i = 0; i < _objectPoolNumber; i++) {
			InstantiateBullet();
		}
		
		_bodyTop = GetNode<Node2D>(_bodyTopPath);
	}
	
	public override bool GetCondition(Player p, Enemy e) {
		return (Mathf.Abs(p.GlobalPosition.X - e.GlobalPosition.X) <= ShootStartRange) && CanShoot;
	}
	
	public override void Execute(Player p, Enemy e) {
		base.Execute(p, e);
		
		GD.Print("executing!");
		
		_playerPosition = p.GlobalPosition;
		_aimedPlayer = p;
		// _accelerating = false;
		Shoot();
		_charging = true;
		_bodyTop.Visible = true;
		_bodyTop.LookAt(_playerPosition);
		StartShootCooldown();
		((EnemyPatrolAI)e).Decelerate();
	}
	
	public override void _Process(double delta) {
		base._Process(delta);
		if(_charging) {
			_playerPosition = _aimedPlayer.GlobalPosition;
			
			if(EnemyAI.GlobalPosition.X < _playerPosition.X && ((EnemyPatrolAI)EnemyAI).GetMoveDirection() < 0) {
				((EnemyPatrolAI)EnemyAI).Flip();
			} else if(EnemyAI.GlobalPosition.X > _playerPosition.X && ((EnemyPatrolAI)EnemyAI).GetMoveDirection() > 0) {
				((EnemyPatrolAI)EnemyAI).Flip();
				// _swordsmanAI.Scale = new Vector2(-Mathf.Abs(_swordsmanAI.Scale.X), _swordsmanAI.Scale.Y);
			}
			
			_bodyTop.LookAt(_playerPosition);
			_bodyTop.Rotation = Mathf.Clamp(_bodyTop.Rotation, Mathf.DegToRad(-60f), Mathf.DegToRad(60f));
		}
	}
	
	public void Shoot() {
		// SlashEvent?.Invoke(_playerStats.SlashDamage, _playerStats.SlashTime, _playerStats.SlashRange);		
		GetTree().CreateTimer(AttackLength).Timeout += _FinishShoot;
		GetTree().CreateTimer(ChargeTime).Timeout += _UseShot;
		// await ToSignal(GetTree().CreateTimer(SlashTime), SceneTreeTimer.SignalName.Timeout);
	}
	
	private void _UseShot() {
		if(!Active)
			return;
			
		_charging = false;
			
		foreach(Bullet b in bullets) {
			if(!b.InUse) {
				b.GlobalPosition = this.GlobalPosition;
				
				b.LookAt(_playerPosition);
				// GD.Print(Mathf.RadToDeg(b.GlobalRotation));
				
				b.Fire();
				
				break;
			}
		}
		// GetTree().CreateTimer(_playerStats.ShootTime).Timeout += _FinishShoot;
	}

	private void _FinishShoot() {
		Finish();
		_bodyTop.Visible = false;
	}
	
	public override void Interrupt() {
		_bodyTop.Visible = false;
		base.Interrupt();
	}
	
	public void StartShootCooldown() {
		CanShoot = false;
		GetTree().CreateTimer(ShootCooldown).Timeout += _FinishShootCooldown;
	}

	private void _FinishShootCooldown() {
		CanShoot = true; 
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
		
		// if(!(hit is IHittable)) {
		//	return;
		// }
		
		// HitEvent?.Invoke((IHittable) hit, _playerStats.ShootDamage, this.GlobalPosition.X > hit.GlobalPosition.X ? 1 : -1, _playerStats.ShootPostureDamage);
		
		if(!(hit is Player)) {
			return;
		}
		
		// ((Player)hit).TakeDamage(ShotDamage, PostureDamage, EnemyAI, 0);
		((Player)hit).TakeDamage(_attackData, EnemyAI);
	}
}
