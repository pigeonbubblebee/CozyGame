using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerSpellController : Node2D
{	
	public bool DesiredShoot => _CheckDesiredShoot() && CanShoot;
	private int _maxMana => _playerStats.MaxMana;
	[Export] public int CurrentMana { get; private set; }

	public event Action<int> ManaUseEvent;
	public event Action<int> AddManaEvent;
	
	public event Action FinishShootEvent;
	
	private Player _player;
	private PlayerMovementController _movementController;
	private PlayerStats _playerStats => _player.CurrentPlayerStats;
	
	private PackedScene _bulletScene;
	private readonly string BULLET_PATH = "res://Assets/Scene/bullet.tscn";
	
	private List<Bullet> bullets = new List<Bullet>();
	
	[Export] private NodePath _objectPoolPath;
	public Node ObjectPool { get; private set; }
	
	[Export] private NodePath _shootBufferPath;
	private Timer _shootBuffer;
	
	public bool CanShoot { get; private set; }
	
	private IInputManager _inputManager;

	// TODO: implement health changes
	
	public void Initialize(Player player) {
		_player = player;
		_movementController = player.MovementController;
	}
	
	public override void _Ready() {
		CanShoot = true;
		
		_inputManager = GetNode<IInputManager>("/root/InputManager");
		
		ObjectPool = GetNode<Node>(_objectPoolPath);
		_bulletScene = GD.Load<PackedScene>(BULLET_PATH);
		
		_shootBuffer = GetNode<Timer>(_shootBufferPath);
		_shootBuffer.WaitTime = _playerStats.ShootBuffer;
		
		for(int i = 0; i < _playerStats.BulletObjectPool; i++) {
			InstantiateBullet();
		}
		// UseSpell();
	}
	
	public override void _Process(double delta)
	{
		if(_CheckDesiredShoot() && !CanShoot) {
			StartShootBuffer();
		}
	}

	public void ResetMana() {
		CurrentMana = _maxMana;
	}
	
	public void AddMana(int amt) {
		CurrentMana += amt;
		CurrentMana = Mathf.Min(CurrentMana, _maxMana);
		AddManaEvent?.Invoke(amt);
	}

	public void UseMana(int amt) { // Temp until spells implemented
		if(CurrentMana - amt < 0) {
			return;
		}
		
		CurrentMana -= amt;
		ManaUseEvent?.Invoke(amt);
	}
	
	public void UseSpell() {
		// CanSwitchAttackDirection = false;
		// ShootEvent?.Invoke(_playerStats.SlashDamage, _playerStats.SlashTime, _playerStats.SlashRange);
		// GD.Print("PEW");
		foreach(Bullet b in bullets) {
			if(!b.InUse) {
				b.GlobalPosition = this.GlobalPosition;
				
				if(_movementController.Direction == -1) {
					b.Rotation = Mathf.DegToRad(180);
				} else {
					b.Rotation = Mathf.DegToRad(0);
				}
				
				b.Fire();
				
				break;
			}
		}
		GetTree().CreateTimer(_playerStats.ShootTime).Timeout += _FinishShoot;
		// await ToSignal(GetTree().CreateTimer(SlashTime), SceneTreeTimer.SignalName.Timeout);
	}

	private void _FinishShoot() {
		FinishShootEvent?.Invoke();
	}
	
	public void StartShootCooldown() {
		CanShoot = false;
		GetTree().CreateTimer(_playerStats.ShootCooldown).Timeout += _FinishShootCooldown;
	}

	private void _FinishShootCooldown() {
		CanShoot = true; 
	}
	
	private bool _CheckDesiredShoot() {
		return _inputManager.GetShootActuation();
	}
	
	private void InstantiateBullet() {
		Bullet bullet = (Bullet)_bulletScene.Instantiate();
		ObjectPool.AddChild(bullet);
		bullet.Position = this.GlobalPosition;
		bullets.Add(bullet);
	}
	
	public void StartShootBuffer() {
		_shootBuffer.Start();
	}

	public bool GetShootBufferStop() {
		return _shootBuffer.IsStopped();
	}
}