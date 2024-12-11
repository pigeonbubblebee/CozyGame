using Godot;
using System;

public partial class PlayerDeflectController : Node
{
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
	
	public void Initialize(Player player) { // TODO: Add punish for block spam
		_player = player;
	}
	
	public override void _Ready() {
		_inputManager = GetNode<IInputManager>("/root/InputManager");
		_deflectWindowTimer = GetNode<Timer>(_deflectWindowTimerPath);
	}
	
	public override void _Process(double delta) {
		if(_inputManager.GetDeflectActuation()) {
			_deflectWindowTimer.Start(_playerStats.DeflectWindow);
		}
	}
	
	public int Block(int damage, int postureDamage, Enemy e) {
		if(InDeflectWindow()) {
			BlockEvent?.Invoke(true, postureDamage, e);
			GD.Print("Deflect!");
			e.TakePostureDamage(postureDamage);
			Counter = true;
			GetTree().CreateTimer(_playerStats.CounterWindow).Timeout += _FinishCounterWindow;
			return 0;
		} else {
			BlockEvent?.Invoke(false, postureDamage, e);
			_player.PlayerHealth.TakeDamage(damage/4);
			_player.PostureController.TakePostureDamage(postureDamage);
			return 0;
		}
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
}
