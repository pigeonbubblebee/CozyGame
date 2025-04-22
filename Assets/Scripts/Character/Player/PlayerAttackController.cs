using Godot;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public partial class PlayerAttackController : Node2D // TODO: Attack Buffer
{
	[Export] private NodePath _slashBufferPath;
	private Timer _slashBuffer;

	public bool DesiredAttack => _CheckDesiredAttack() && CanSlash && !_uiManager.InventoryOpen  && !_uiManager.MerchantOpen;
	private IInputManager _inputManager;
	
	public bool DesiredDown => _inputManager.GetDown() && !_movementController.Grounded;

	private GameManager _gameManager;

	public event Action<int, float, float> SlashEvent; // Damage, Speed, Range
	public event Action<IHittable, int, int, int> HitEvent; // Hittable, Damage, Direction
	public event Action FinishSlashEvent;

	[Export] private NodePath _attackAreaPath;
	private Area2D _attackArea;
	[Export] private NodePath _rightAttackAreaColliderPath;
	private CollisionShape2D _rightAttackAreaCollider;
	[Export] private NodePath _leftAttackAreaColliderPath;
	private CollisionShape2D _leftAttackAreaCollider;
	[Export] private NodePath _downAttackAreaColliderPath;
	private CollisionShape2D _downAttackAreaCollider;
	
	[Export] private NodePath _deathblowCheckAreaPath;
	private Area2D _deathblowCheckArea;
	[Export] private NodePath _rightDeathblowCheckAreaColliderPath;
	private CollisionShape2D _rightDeathblowCheckAreaCollider;
	[Export] private NodePath _leftDeathblowCheckAreaColliderPath;
	private CollisionShape2D _leftDeathblowCheckAreaCollider;

	[Export] private NodePath _attackSpritePath;
	private AnimatedSprite2D _attackSprite;
	
	[Export] private NodePath _slashParticlePoolPath;
	private Node2D _slashParticlePool;

	private CollisionShape2D _currentAttackAreaCollider;
	public bool CanSwitchAttackDirection = true;

	private bool _canHit = false;
	public bool CanSlash { get; private set; }

	private Player _player;
	private PlayerStats _playerStats => _player.CurrentPlayerStats;
	
	private HashSet<IHittable> _invincibleHittables = new HashSet<IHittable>();

	private PlayerMovementController _movementController;

	private float _slashComboCounter;
	public int CurrentSlashComboAttack { get; private set; }
	
	public bool CanDeathBlow { get; private set; }
	
	[Export] private NodePath _swingSFXPath;
	private AdjustableSound _swingSFX;
	
	[Export] private NodePath _slashHitSFXPath;
	private AudioStreamPlayer2D _slashHitSFX;
	
	[Export] private NodePath _deathBlowSFXPath;
	private AudioStreamPlayer2D _deathBlowSFX;
	
	public event Action DeathBlowEvent;

	private UIManager _uiManager;

	public override void _Ready()
	{
		CanSlash = true;
		
		ResetComboCounter();

		_inputManager = GetNode<IInputManager>("/root/InputManager");
		_gameManager = GetNode<GameManager>("/root/GameManager");
		_uiManager = GetNode<UIManager>("/root/UIManager");
		
		_attackArea = GetNode<Area2D>(_attackAreaPath);
		_rightAttackAreaCollider = GetNode<CollisionShape2D>(_rightAttackAreaColliderPath);
		_leftAttackAreaCollider = GetNode<CollisionShape2D>(_leftAttackAreaColliderPath);
		_downAttackAreaCollider = GetNode<CollisionShape2D>(_downAttackAreaColliderPath);
		
		_deathblowCheckArea = GetNode<Area2D>(_deathblowCheckAreaPath);
		_rightDeathblowCheckAreaCollider = GetNode<CollisionShape2D>(_rightDeathblowCheckAreaColliderPath);
		_leftDeathblowCheckAreaCollider = GetNode<CollisionShape2D>(_leftDeathblowCheckAreaColliderPath);

		_attackSprite = GetNode<AnimatedSprite2D>(_attackSpritePath);
		// _slashHitParticle = GetNode<GpuParticles2D>(_slashHitParticlePath);
		_slashParticlePool = GetNode<Node2D>(_slashParticlePoolPath);

		_slashBuffer = GetNode<Timer>(_slashBufferPath);
		_slashBuffer.WaitTime = _playerStats.SlashBuffer;
		
		_swingSFX = GetNode<AdjustableSound>(_swingSFXPath);
		_slashHitSFX = GetNode<AudioStreamPlayer2D>(_slashHitSFXPath);
		_deathBlowSFX = GetNode<AudioStreamPlayer2D>(_deathBlowSFXPath);

		_attackArea.AreaEntered += _OnSlashHit;
		_attackArea.BodyEntered += _OnSlashHit;
		_attackArea.CollisionMask = (uint) PhysicsLayers.HittableLayer;
		_attackArea.CollisionLayer = (uint) PhysicsLayers.UntouchableLayer;
		
		// _deathblowCheckArea.AreaEntered += _OnDeflectCheck;
		_deathblowCheckArea.CollisionMask = (uint) PhysicsLayers.HittableLayer;
		_deathblowCheckArea.CollisionLayer = (uint) PhysicsLayers.UntouchableLayer;

		HitEvent += _OnHit;
		SlashEvent += _EnableSlashSprite;
		FinishSlashEvent += _DisableSlashSprite;
	}

	public void Initialize(Player player) {
		_player = player;
		// _playerStats = player.PlayerStatsResource;
		_movementController = _player.MovementController;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(_slashComboCounter > 0) {
			_slashComboCounter -= (float)delta;
		}
		if(_slashComboCounter <= 0) {
			ResetComboCounter();
		}
		
		// GD.Print(_slashComboCounter);
			
		_UpdateAttackCollider();

		if(_CheckDesiredAttack() && !CanSlash) {
			StartSlashBuffer();
		}
	}

	private bool _CheckDesiredAttack() {
		return _inputManager.GetAttackActuation();
	}

	public void Slash() {
		// GD.Print("Slashing !");
		_invincibleHittables.Clear();
		foreach(Area2D hit in _deathblowCheckArea.GetOverlappingAreas()) {
			if(hit is EnemyHitbox) {
			// GD.Print(((Enemy) hit).Staggered);
				if(((EnemyHitbox) hit).EnemyAIParent.Staggered) {
					CanDeathBlow = true;
				}
			}
		}
		
		if(!CanDeathBlow) {
			_swingSFX.PlayPitchShifted(0.1f);
		}
		
		CanSwitchAttackDirection = false;
		SlashEvent?.Invoke(_playerStats.SlashDamage, _playerStats.SlashTime, _playerStats.SlashRange);
		
		_slashComboCounter = _playerStats.SlashComboTime;

		_canHit = true;
		
		if(CanDeathBlow) {
			GetTree().CreateTimer(0.1f).Timeout += _EnableAttackCollision;
		} else {
			_EnableAttackCollision();
		}

		// GD.Print(CalculateAS());
		GetTree().CreateTimer(CanDeathBlow ? 1f : CalculateAS()).Timeout += _FinishSlash;
		// await ToSignal(GetTree().CreateTimer(SlashTime), SceneTreeTimer.SignalName.Timeout);
	}
	
	private void _EnableAttackCollision() {
		_currentAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
	}

	private void _FinishSlash() {
		// GD.Print("FinishSlash");
		_increaseSlashCombo();
		
		_canHit = false;
		_rightAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		_leftAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		_downAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		CanSwitchAttackDirection = true;
		FinishSlashEvent?.Invoke();
		// _slashHitParticle.Emitting = false;
		CanDeathBlow = false;
	}
	
	public void CancelSlash() {
		_increaseSlashCombo();
		
		_canHit = false;
		_rightAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		_leftAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		_downAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		CanSwitchAttackDirection = true;
		// _slashHitParticle.Emitting = false;
		// FinishSlashEvent?.Invoke();
		CanDeathBlow = false;
	}

	public void StartSlashCooldown() {
		CanSlash = false;
		GetTree().CreateTimer(CalculateASCD()).Timeout += _FinishSlashCooldown;
	}

	private void _FinishSlashCooldown() {
		CanSlash = true; 
	}

	private void _OnSlashHit(Node2D hit) {
		if(!_canHit || !(hit is IHittable)) {
			return;
		}
		
		int damage = _playerStats.SlashDamage + _player.CurrentBuffs.Strength;
		
		if(hit is EnemyHitbox) {
			// GD.Print(((Enemy) hit).Staggered);
			if(((EnemyHitbox) hit).EnemyAIParent.Staggered) {
				damage = ((EnemyHitbox) hit).EnemyAIParent.DeathBlowDamage;
				DeathBlowEvent?.Invoke();
			}
		}
		
		//_canHit = false;
		//_rightAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		//_leftAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		
		// GD.Print(damage);
		
		HitEvent?.Invoke((IHittable) hit, damage, this.GlobalPosition.X > hit.GlobalPosition.X ? 1 : -1,  _playerStats.SlashDamage + _player.CurrentBuffs.Strength);
	}

	private void _OnHit(IHittable hittable, int damage, int direction, int postureDamage) {
		// GD.Print(_slashHitParticle.Emitting);
		
		if(_invincibleHittables.Contains(hittable))
			return;
			
		_emitSlashParticle();
		
		
		bool iFrame = false;
		
		if(hittable is EnemyHitbox) {
			((EnemyHitbox) hittable).EnemyAIParent.ApplyKnockback(-direction, _playerStats.SlashKnockback, _playerStats.SlashKnockbackAcceleration, _playerStats.SlashKnockbackTime);
			if(!((EnemyHitbox) hittable).EnemyAIParent.Invincible) {
				if(!DesiredDown) {
					if(!((EnemyHitbox) hittable).EnemyAIParent.Staggered) {
						_slashHitSFX.Play();
						_gameManager.FreezeFrame(_playerStats.SlashFreezeTime, _playerStats.SlashFreezeDelay);
						_player.Camera.Shake(_player.DeflectController.Counter ? _playerStats.CounterShakeTime : _playerStats.SlashShakeTime, _player.DeflectController.Counter ?  _playerStats.CounterShakeMagnitude : _playerStats.SlashShakeMagnitude);
					} else {
						_deathBlowSFX.Play();
						_gameManager.FreezeFrame(_playerStats.DeathBlowFreezeTime, _playerStats.SlashFreezeDelay);
						_player.Camera.Shake(_playerStats.CounterShakeTime, _playerStats.CounterShakeMagnitude);
					}
				} else {
					_slashHitSFX.Play();
					_gameManager.FreezeFrame(_playerStats.SlashFreezeTime, _playerStats.SlashFreezeDelay);
					_player.Camera.Shake(_player.DeflectController.Counter ? _playerStats.CounterShakeTime : _playerStats.SlashShakeTime, _player.DeflectController.Counter ?  _playerStats.CounterShakeMagnitude : _playerStats.SlashShakeMagnitude);
					if(((EnemyHitbox) hittable).EnemyAIParent.CurrentAttack != null) {
						if(((EnemyHitbox) hittable).EnemyAIParent.CurrentAttack.GetAttackData().Type == EnemyAttackData.AttackType.Sweep) {
							((EnemyHitbox) hittable).EnemyAIParent.TakePostureDamage(((EnemyHitbox) hittable).EnemyAIParent.CurrentAttack.GetAttackData().DeflectPostureDamage);
						}
					}
					
				}
			} else {
				iFrame = true;
			}
		} else {
			_slashHitSFX.Play();
					_gameManager.FreezeFrame(_playerStats.SlashFreezeTime, _playerStats.SlashFreezeDelay);
					_player.Camera.Shake(_player.DeflectController.Counter ? _playerStats.CounterShakeTime : _playerStats.SlashShakeTime, _player.DeflectController.Counter ?  _playerStats.CounterShakeMagnitude : _playerStats.SlashShakeMagnitude);
		}
		
		if(!iFrame) {
			_invincibleHittables.Add(hittable);
		}

		// GD.Print(((Node)hittable).Name);
		
		hittable.OnHit(_player, damage, direction, postureDamage); // Maybe flip direction, not sure yet
		_movementController.ApplyKnockback(direction, _playerStats.SlashKnockback, _playerStats.SlashKnockbackAcceleration, _playerStats.SlashKnockbackTime);
		// TODO: Slash Particle, Screen Shake
	}

	private void _emitSlashParticle() {
		foreach(Node p in _slashParticlePool.GetChildren()) {
			if(p is GpuParticles2D) {
				if(!((GpuParticles2D)p).Emitting) {
					GpuParticles2D _slashHitParticle = ((GpuParticles2D)p);

					// ((ParticleProcessMaterial)(_slashHitParticle.ProcessMaterial)).
					_slashHitParticle.Lifetime = CalculateAS() / 1.5f;

					if(_movementController.Direction < 0) {
						_slashHitParticle.GlobalRotation = Mathf.DegToRad(245f);
						_slashHitParticle.Position = new Vector2(-215, 32f);
					} else {
						_slashHitParticle.GlobalRotation = Mathf.DegToRad(0f);
						_slashHitParticle.Position = new Vector2(225, 0f);
					}
					
					if(CurrentSlashComboAttack == 0 && !_player.DeflectController.Counter) {
						_slashHitParticle.GlobalRotation += (_movementController.Direction == 1) ? Mathf.DegToRad(75f) : Mathf.DegToRad(-75f);
					}
					
					if(DesiredDown) {
						_slashHitParticle.GlobalRotation = Mathf.DegToRad(-45f);
						_slashHitParticle.Position = new Vector2(0, 280f);
					}
					
					_slashHitParticle.Restart();
					return;
				}
			}
		}
		
	}

	private void _UpdateAttackCollider() {
		// if(!CanSwitchAttackDirection) {
		//	return;
		// }
		
		// GD.Print(_movementController.Direction);

		// _attackSprite = _movementController.Direction == 1 ? _rightAttackSprite : _leftAttackSprite;
		_currentAttackAreaCollider = _movementController.Direction == 1 ? _rightAttackAreaCollider : _leftAttackAreaCollider;
		if(DesiredDown) {
			_currentAttackAreaCollider = _downAttackAreaCollider;
		}
		// GD.Print(_currentAttackAreaCollider.Name);
		if(_movementController.Direction == 1) {
			_rightDeathblowCheckAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
			_leftDeathblowCheckAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		} else if(_movementController.Direction == -1) {
			_rightDeathblowCheckAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
			_leftDeathblowCheckAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
		}
		
	}

	public void StartSlashBuffer() {
		_slashBuffer.Start();
	}

	public bool GetSlashBufferStop() {
		return _slashBuffer.IsStopped();
	}
	
	 public void ResetComboCounter() {
		CurrentSlashComboAttack = 0;
	}

	private void _increaseSlashCombo() {
		CurrentSlashComboAttack ++; // Increase Combo, Reset If Finished
		if(CurrentSlashComboAttack > 1) {
			CurrentSlashComboAttack = 0;
		}
	}

	private void _EnableSlashSprite(int damage, float speed, float range) {
		// _attackSprite.Play(_movementController.Direction == 1 ? "slash_right" : "slash_left");
		
		// _attackSprite.Visible = true;
		
		// _attackSprite.ZIndex = _movementController.Direction == 1 ? 1 : -1;
		// GD.Print(_attackSprite.Animation);
	}

	private void _DisableSlashSprite() {
		// _currentAttackSprite.Visible = false;
		_attackSprite.Play("idle");
		// _attackSprite.Visible = false;
	}
	
	public float CalculateAS() {
		float speed = _playerStats.SlashCooldown; // Base 2
		float trueSpeed = _playerStats.SlashTime; // base 2.5

		speed += (0.1f * _player.CurrentBuffs.Dexterity);

		if(speed  < 2.5)
			return 1 / trueSpeed;
		else
			return 1 / speed;
	}

	public float CalculateASCD() {
		float speed = _playerStats.SlashCooldown; // Base 2

		speed += (0.1f * _player.CurrentBuffs.Dexterity);
		return 1 / speed;
	}
}
