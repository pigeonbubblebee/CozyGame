using Godot;
using System;

public partial class PlayerDeflectController : Node
{
	[Export] private NodePath _deflectBufferPath;
	private Timer _deflectBuffer;
	
	public bool DesiredDeflect => _CheckDesiredDeflect();
	public bool DeflectActuation => _GetDeflectActuation();
	
	private Player _player;
	private PlayerStats _playerStats => _player.CurrentPlayerStats;
	
	[Export] private NodePath _deflectWindowTimerPath;
	private Timer _deflectWindowTimer;
	
	public bool Counter = false;
	
	private IInputManager _inputManager;
	public bool Blocking = false;
	
	public event Action<bool, int, Enemy> BlockEvent;
	
	private GameManager _gameManager;
	
	[Export] private NodePath _blockParticlePath;
	private GpuParticles2D _blockParticle;
	
	[Export] private NodePath _deflectParticlePath;
	private GpuParticles2D _deflectParticle;
	
	public void Initialize(Player player) { // TODO: Add punish for block spam
		_player = player;
	}
	
	public override void _Ready() {
		_deflectBuffer = GetNode<Timer>(_deflectBufferPath);
		_deflectBuffer.WaitTime = 0.2f;
		
		_inputManager = GetNode<IInputManager>("/root/InputManager");
		_gameManager = GetNode<GameManager>("/root/GameManager");
		
		_deflectWindowTimer = GetNode<Timer>(_deflectWindowTimerPath);
		
		_blockParticle = GetNode<GpuParticles2D>(_blockParticlePath);
		_deflectParticle = GetNode<GpuParticles2D>(_deflectParticlePath);
	}
	
	public override void _Process(double delta) {
		if(_inputManager.GetDeflectActuation()) {
			_deflectWindowTimer.Start(_playerStats.DeflectWindow);
		}
	}
	
	public int Block(int damage, int postureDamage, Enemy e) {
		if(InDeflectWindow()) {
			_emitDeflectParticle();
			_player.Camera.Shake(_playerStats.DeflectShakeTime, _playerStats.DeflectShakeMagnitude);
			BlockEvent?.Invoke(true, postureDamage, e);
			// GD.Print("Deflect!");
			e.TakePostureDamage(postureDamage);
			Counter = true;
			GetTree().CreateTimer(_playerStats.CounterWindow).Timeout += _FinishCounterWindow;
			e.ApplyKnockback(_player.GlobalPosition.X > e.GlobalPosition.X ? -1 : 1, _playerStats.DeflectKnockback, _playerStats.DeflectKnockbackAcceleration, _playerStats.DeflectKnockbackTime);
			_player.MovementController.ApplyKnockback(_player.GlobalPosition.X > e.GlobalPosition.X ? 1 : -1, _playerStats.DeflectKnockback, _playerStats.DeflectKnockbackAcceleration, _playerStats.DeflectKnockbackTime);
			_gameManager.FreezeFrame(0.02f, 0.1f);
			return 0;
		} else {
			_emitBlockParticle();
			
			_player.Camera.Shake(_playerStats.BlockShakeTime, _playerStats.BlockShakeMagnitude);
			BlockEvent?.Invoke(false, postureDamage, e);
			// _player.PlayerHealth.TakeDamage(damage/4);
			_player.PostureController.TakePostureDamage(postureDamage);
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
		Blocking = true;
	}
	
	public void EndBlock() {
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
			_blockParticle.GlobalRotation = Mathf.DegToRad(-90f);
			_blockParticle.Position = new Vector2(-96, -32f);
		} else {
			_blockParticle.GlobalRotation = Mathf.DegToRad(0f);
			_blockParticle.Position = new Vector2(96, -32f);
		}

		_blockParticle.Emitting = true;
	}
	
	private void _emitDeflectParticle() {
		if(_player.MovementController.Direction < 0) {
			_deflectParticle.GlobalRotation = Mathf.DegToRad(-90f);
			_deflectParticle.Position = new Vector2(-96, -32f);
		} else {
			_deflectParticle.GlobalRotation = Mathf.DegToRad(0f);
			_deflectParticle.Position = new Vector2(96, -32f);
		}

		_deflectParticle.Emitting = true;
	}
}
