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
	
	public event Action<int> InternalHealthChangeEvent;
	
	public int InternalDamage { get; private set; }
	[Export] private NodePath _internalDamageHealDelayTimerPath;
	private Timer InternalDamageHealDelayTimer;
	[Export] private NodePath _internalDamageHealTimerPath;
	private Timer InternalDamageHealTimer;
	
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
		
		// InternalDamage = _playerStats.MaxHealth;
		InternalDamageHealDelayTimer = GetNode<Timer>(_internalDamageHealDelayTimerPath);
		InternalDamageHealTimer = GetNode<Timer>(_internalDamageHealTimerPath);
		_player.PlayerHealth.DamageEvent += ResetInternalDamageHealDelay;
		
		_inputManager = GetNode<IInputManager>("/root/InputManager");
	}
	
	public void ConvertInternalDamage() {
		_player.PlayerHealth.TakeDamage(InternalDamage);
		InternalDamage = 0;
	}
	
	public void TakeInternalDamage(int n) {
		InternalDamage += n;
		ResetInternalDamageHealDelay(n);
		InternalHealthChangeEvent?.Invoke(n);
	}
	
	public void ResetInternalDamageHealDelay(int dmg) {
		InternalDamageHealDelayTimer.Start(_playerStats.InternalDamageHealDelay);
	}
	
	public override void _Process(double delta) {
		if(InternalDamage <= 0) {
			return;
		}
		
		if(!InternalDamageHealDelayTimer.IsStopped()) {
			return;
		}
		
		if(InternalDamageHealTimer.IsStopped()) {
			_player.PlayerHealth.AddHealth(Mathf.Min(InternalDamage, _playerStats.InternalDamageHealRate), false);
			InternalDamage -= Mathf.Min(InternalDamage, _playerStats.InternalDamageHealRate);
			InternalHealthChangeEvent?.Invoke(-Mathf.Min(InternalDamage, _playerStats.InternalDamageHealRate));
			InternalDamageHealTimer.Start(_playerStats.InternalDamageHealCooldown);
		}
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
