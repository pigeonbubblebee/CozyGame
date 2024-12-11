using Godot;
using System;

public partial class PlayerHealController : Node // TODO: Refactor to controller w/ inventory
{
	private int _maxHeals => _playerStats.MaxHeals;
	public bool CanHeal => CurrentHeals > 0;
	[Export] public int CurrentHeals { get; private set; }
	
	public event Action FinishHealChargeEvent;
	public event Action HealEvent;
	public event Action AddHealEvent;
	
	private Player _player;
	private PlayerStats _playerStats => _player.CurrentPlayerStats;
	
	public bool DesiredHeal => _CheckDesiredHeal() && CanHeal;
	private IInputManager _inputManager;
	
	[Export] private NodePath _healTimerPath;
	private Timer _healTimer;
	
	public bool HealCooldownOff { get; private set; }
	// TODO: Heal Cooldown
	// TODO: implement health changes
	
	public override void _Ready()
	{
		HealCooldownOff = true;
		_healTimer = GetNode<Timer>(_healTimerPath);
		_healTimer.WaitTime = _playerStats.HealTime;
		_healTimer.Timeout += _FinishHeal;
		
		_inputManager = GetNode<IInputManager>("/root/InputManager");
	}
	
	public void Initialize(Player player) {
		_player = player;
	}

	public void ResetHeals() {
		CurrentHeals = _maxHeals;
	}
	
	public void AddHeals(int amt) {
		CurrentHeals += amt;
		CurrentHeals = Mathf.Min(CurrentHeals, _maxHeals);
		AddHealEvent?.Invoke();
	}

	public void UseHeal() {
		if(CurrentHeals <= 0) {
			return;
		}
		
		CurrentHeals --;
		_player.PlayerHealth.AddHealth(_playerStats.HealPower, false);
		HealEvent?.Invoke();
	}
	
	private bool _CheckDesiredHeal() {
		return _inputManager.GetHeal();
	}
	
	public void StartHeal() {
		_healTimer.Start();
	}
	
	public void InterruptHeal() {
		// _healTimer.TimeLeft = _playerStats.HealTime;
		_healTimer.Stop();
	}

	private void _FinishHeal() {
		UseHeal();
		FinishHealChargeEvent?.Invoke();
	}
	
	public void StartHealCooldown() {
		HealCooldownOff = false;
		GetTree().CreateTimer(_playerStats.HealCooldown).Timeout += _FinishHealCooldown;
	}

	private void _FinishHealCooldown() {
		HealCooldownOff = true; 
	}
}
