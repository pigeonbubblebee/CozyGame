using Godot;
using System;

public partial class HUD : Control
{	
	[Export] private NodePath _healBarPath;
	private TextureProgressBar _healBar;
	[Export] private NodePath _manaBarPath;
	private TextureProgressBar _manaBar;
	
	[Export] private NodePath _healthHUDPath;
	private HealthHUD _healthHUD;
	
	private Player _currentScenePlayer;
	
	public override void _Ready() {
		_healBar = GetNode<TextureProgressBar>(_healBarPath);
		_manaBar = GetNode<TextureProgressBar>(_manaBarPath);
		_healthHUD = GetNode<HealthHUD>(_healthHUDPath);
	}
	
	public void SetCurrentPlayerInstance(Player p) {
		_currentScenePlayer = p;
		
		p.PlayerHealth.DamageEvent += _UpdateHealthHUD;
		p.PlayerHealth.HealEvent += _UpdateHealthHUD;
		
		p.HealController.HealEvent += _UpdateHealBar;
		p.HealController.AddHealEvent += _UpdateHealBar;
		
		p.SpellController.ManaUseEvent += _UpdateManaBar;
		p.SpellController.AddManaEvent += _UpdateManaBar;
		
		_UpdateHealthHUD(0);
		_UpdateHealBar();
		_UpdateManaBar(0);
	}
	
	private void _UpdateHealthHUD(int amt) {
		if(_currentScenePlayer != null) {
			_healthHUD.UpdateHealth(_currentScenePlayer.PlayerHealth.CurrentHealthPoints, _currentScenePlayer.CurrentPlayerStats.MaxHealth);
		}
	}
	
	private void _UpdateHealBar() {
		if(_currentScenePlayer != null) {
			float Ratio = ((float) _currentScenePlayer.HealController.CurrentHeals) / ((float) _currentScenePlayer.CurrentPlayerStats.MaxHeals);
			Ratio = Mathf.Max(Ratio, 0);
			
			_healBar.Value = Ratio * 100;
		}
	}
	
	private void _UpdateManaBar(int amt) {
		if(_currentScenePlayer != null) {
			float Ratio = ((float) _currentScenePlayer.SpellController.CurrentMana) / ((float) _currentScenePlayer.CurrentPlayerStats.MaxMana);
			Ratio = Mathf.Max(Ratio, 0);
			GD.Print(Ratio);
			_manaBar.Value = Ratio * 100;
		}
	}
}
