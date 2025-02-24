using Godot;
using System;

public partial class PlayerDeflectController : Node
{
	[Export] private NodePath _deflectBufferPath;
	private Timer _deflectBuffer;
	
	public bool DesiredDeflect => _CheckDesiredDeflect();
	public bool DeflectActuation => _GetDeflectActuation() && CanBlock;
	
	private Player _player;
	private PlayerStats _playerStats => _player.CurrentPlayerStats;
	
	[Export] private NodePath _deflectWindowTimerPath;
	private Timer _deflectWindowTimer;
	
	[Export] private NodePath _deflectCancelTimerPath;
	private Timer _deflectCancelTimer;
	
	public bool Counter = false;
	
	private IInputManager _inputManager;
	public bool Blocking { get; private set; }
	
	public event Action<bool, int, Enemy> BlockEvent;
	
	private GameManager _gameManager;
	
	public bool CanBlock { get; private set; }
	
	[Export] private NodePath _blockParticlePoolPath;
	public Node2D BlockParticlePool { get; private set; }
	
	[Export] private NodePath _deflectParticlePoolPath;
	public Node2D DeflectParticlePool { get; private set; }
	
	[Export] private NodePath _deflectSFXPath;
	private AudioStreamPlayer2D _deflectSFX;
	
	[Export] private NodePath _blockSFXPath;
	private AudioStreamPlayer2D _blockSFX;
	
	[Export] private NodePath _cleaveCounterCheckAreaPath;
	private Area2D _cleaveCounterCheckArea;
	
	public bool CanCleaveCounter = false;
	public CleaveAttack CurrentCleaveCounter = null;
	
	public void Initialize(Player player) { // TODO: Add punish for block spam
		_player = player;
	}
	
	public override void _Ready() {
		CanBlock = true;
		Blocking = false;
		
		_deflectBuffer = GetNode<Timer>(_deflectBufferPath);
		_deflectBuffer.WaitTime = 0.1f;
		
		_inputManager = GetNode<IInputManager>("/root/InputManager");
		_gameManager = GetNode<GameManager>("/root/GameManager");
		
		_deflectWindowTimer = GetNode<Timer>(_deflectWindowTimerPath);
		_deflectCancelTimer = GetNode<Timer>(_deflectCancelTimerPath);
		
		BlockParticlePool = GetNode<Node2D>(_blockParticlePoolPath);
		DeflectParticlePool = GetNode<Node2D>(_deflectParticlePoolPath);
		
		_deflectSFX = GetNode<AudioStreamPlayer2D>(_deflectSFXPath);
		_blockSFX = GetNode<AudioStreamPlayer2D>(_blockSFXPath);
		
		_cleaveCounterCheckArea = GetNode<Area2D>(_cleaveCounterCheckAreaPath);
	}
	
	public override void _Process(double delta) {
		// GD.Print(_deflectWindowTimer.TimeLeft + " " + _playerStats.DeflectWindow);
		bool hasHit = false;
		
		// GD.Print(CanCleaveCounter);
		
		foreach(Area2D hit in _cleaveCounterCheckArea.GetOverlappingAreas()) {
			// GD.Print(hit.Name);
			if(hit is CleaveCounterArea) {
				// GD.Print(((CleaveCounterArea)hit).CleaveAttack);
				if(((CleaveCounterArea)hit).CleaveAttack.Counterable) {
					CurrentCleaveCounter = ((CleaveCounterArea)hit).CleaveAttack;
					CanCleaveCounter = true;
					hasHit = true;
					return;
				}
			}
		}
		
		if(!hasHit) {
			CurrentCleaveCounter = null;
			CanCleaveCounter = false;
		}
	}
	
	public void CounterCleave() {
		if(CanCleaveCounter) {
			_player.Camera.Shake(_playerStats.DeflectShakeTime, _playerStats.DeflectShakeMagnitude);
			CurrentCleaveCounter.Counter(_player);
		}
	}
	
	public void StartDeflectWindow() {
		_deflectWindowTimer.Start(_playerStats.DeflectWindow);
		_deflectCancelTimer.Start(_playerStats.BlockMinimumTime);
	}
	
	public int Block(EnemyAttackData data, Enemy e) {
		// GD.Print(_deflectWindowTimer.TimeLeft + " deflect");
		if(InDeflectWindow()) {
			_deflectSFX.Play();
			_emitDeflectParticle();
			_player.Camera.Shake(_playerStats.DeflectShakeTime, _playerStats.DeflectShakeMagnitude);
			_gameManager.FreezeFrame(0.02f, 0.1f);
			
			BlockEvent?.Invoke(true, data.PostureDamage, e);
			e.TakePostureDamage(data.DeflectPostureDamage);
			GD.Print("TakePosture!");
			Counter = true;
			
			GetTree().CreateTimer(_playerStats.CounterWindow).Timeout += _FinishCounterWindow;
			
			e.ApplyKnockback(_player.GlobalPosition.X > e.GlobalPosition.X ? -1 : 1, _playerStats.DeflectKnockback * data.DeflectKnockbackMultiplier,
				_playerStats.DeflectKnockbackAcceleration, _playerStats.DeflectKnockbackTime);
			_player.MovementController.ApplyKnockback(_player.GlobalPosition.X > e.GlobalPosition.X ? 1 : -1, _playerStats.DeflectKnockback, _playerStats.DeflectKnockbackAcceleration, _playerStats.DeflectKnockbackTime);
			
			return 0;
		} else {
			if(data.Unstoppable && data.Type == EnemyAttackData.AttackType.Thrust) {
				_player.TakeTrueDamage(data, e);
				// _blockSFX.Play();
				_emitBlockParticle();
				BlockEvent?.Invoke(false, data.PostureDamage, e);
				return 0;
			}

			
			_blockSFX.Play();
			_emitBlockParticle();
			
			_player.Camera.Shake(_playerStats.BlockShakeTime, _playerStats.BlockShakeMagnitude);
			BlockEvent?.Invoke(false, data.PostureDamage, e);
			// _player.PlayerHealth.TakeDamage(damage/4);
			_player.PostureController.TakePostureDamage(data.PostureDamage);
			_player.PlayerHealth.TakeDamage(data.Damage);
			_player.HealController.TakeInternalDamage(data.Damage);
			
			_player.MovementController.ApplyKnockback(_player.GlobalPosition.X > e.GlobalPosition.X ? 1 : -1, _playerStats.BlockKnockback, _playerStats.BlockKnockbackAcceleration, _playerStats.BlockKnockbackTime);
			_gameManager.FreezeFrame(0.02f, 0.1f);
			return 0;
		}
	}
	
	public void StartDeflectBuffer() {
		_deflectBuffer.Start();
	}

	public bool GetDeflectBufferStop() {
		return _deflectBuffer.IsStopped();
	}
	
	private void _FinishCounterWindow() {
		Counter = false;
	}
	
	public void StartBlock() {
		// GD.Print("Start Block");
		Blocking = true;
	}
	
	public void EndBlock() {
		// GD.Print("End Block");
		Blocking = false;
	}
	
	private bool _CheckDesiredDeflect() {
		return _inputManager.GetDeflect();
	}
	
	private bool _GetDeflectActuation() {
		return _inputManager.GetDeflectActuation();
	}
	
	public bool InDeflectWindow() {
		return !_deflectWindowTimer.IsStopped();
	}
	
	private void _emitBlockParticle() {
		if(_player.MovementController.Direction < 0) {
			BlockParticlePool.GlobalRotation = Mathf.DegToRad(-180f);
			// _blockParticle.Position = new Vector2(-96, -32f);
		} else {
			BlockParticlePool.GlobalRotation = Mathf.DegToRad(0f);
			//_blockParticle.Position = new Vector2(96, -32f);
		}

		foreach(Node p in BlockParticlePool.GetChildren()) {
			if(p is GpuParticles2D) {
				if(!((GpuParticles2D)p).Emitting) {
					((GpuParticles2D)p).Restart();
					return;
				}
			}
		}
	}
	
	private void _emitDeflectParticle() {
		if(_player.MovementController.Direction < 0) {
			DeflectParticlePool.GlobalRotation = Mathf.DegToRad(-180f); // -90f
			// _deflectParticle.Position = new Vector2(-96, -32f);
		} else {
			DeflectParticlePool.GlobalRotation = Mathf.DegToRad(0f);
			// _deflectParticle.Position = new Vector2(96, -32f);
		}
		
		// GD.Print("DeflectParticles!");

		foreach(Node p in DeflectParticlePool.GetChildren()) {
			if(p is GpuParticles2D) {
				GD.Print(p.Name + " " + !((GpuParticles2D)p).Emitting);
				if(!((GpuParticles2D)p).Emitting) {
					((GpuParticles2D)p).Restart();
					return;
				}
			}
		}
	}
	
	public bool BlockCancellable() {
		return _deflectCancelTimer.IsStopped();
	}
	
	public float BlockCancelTime() {
		return (float)_deflectCancelTimer.TimeLeft;
	}
	
	public void StartBlockCooldown() {
		// CanBlock = false;
		//GD.Print("Cooldown Start");
		GetTree().CreateTimer(_playerStats.BlockCooldown).Timeout += _FinishBlockCooldown;
	}

	private void _FinishBlockCooldown() {
		//GD.Print("Cooldown Finish");
		CanBlock = true; 
	}
}
