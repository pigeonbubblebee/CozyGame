using Godot;
using System;

public partial class HUD : Control
{	
	[Export] private NodePath _healBarPath;
	private TextureProgressBar _healBar;
	[Export] private NodePath _manaBarPath;
	private TextureProgressBar _manaBar;
	[Export] private NodePath _postureBarPath;
	private TextureProgressBar _postureBar;
	
	[Export] private NodePath _healthHUDPath;
	private HealthHUD _healthHUD;
	
	private Player _currentScenePlayer;
	
	public override void _Ready() {
		_healBar = GetNode<TextureProgressBar>(_healBarPath);
		_manaBar = GetNode<TextureProgressBar>(_manaBarPath);
		_postureBar = GetNode<TextureProgressBar>(_postureBarPath);
		_healthHUD = GetNode<HealthHUD>(_healthHUDPath);
	}
	
	public void SetCurrentPlayerInstance(Player p) {
		_currentScenePlayer = p;
		
		p.PlayerHealth.DamageEvent += _UpdateManaBar;
		p.PlayerHealth.HealEvent += _UpdateManaBar;
		
		p.HealController.HealEvent += _UpdateHealBar;
		p.HealController.AddHealEvent += _UpdateHealBar;
		
		p.SpellController.ManaUseEvent += _UpdateHealthHUD;
		p.SpellController.AddManaEvent += _UpdateHealthHUD;
		
		p.PostureController.PostureChangeEvent += _UpdatePostureBar;
		// p.SpellController.AddManaEvent += _UpdateHealthHUD;
		
		_UpdateHealthHUD(0);
		_UpdateHealBar();
		_UpdateManaBar(0);
		_UpdatePostureBar(0);
	}
	
	private void _UpdateHealthHUD(int amt) {
		if(_currentScenePlayer != null) {
			// _healthHUD.UpdateHealth(_currentScenePlayer.PlayerHealth.CurrentHealthPoints, _currentScenePlayer.CurrentPlayerStats.MaxHealth);
			_healthHUD.UpdateHealth(_currentScenePlayer.SpellController.CurrentMana, _currentScenePlayer.CurrentPlayerStats.MaxMana);
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
			// float Ratio = ((float) _currentScenePlayer.SpellController.CurrentMana) / ((float) _currentScenePlayer.CurrentPlayerStats.MaxMana);
			float Ratio = ((float) _currentScenePlayer.PlayerHealth.CurrentHealthPoints) / ((float) _currentScenePlayer.CurrentPlayerStats.MaxHealth);
			Ratio = Mathf.Max(Ratio, 0);
			
			_manaBar.Value = Ratio * 100;
		}
	}
	
	private void _UpdatePostureBar(int amt) {
		if(_currentScenePlayer != null) {
			// float Ratio = ((float) _currentScenePlayer.SpellController.CurrentMana) / ((float) _currentScenePlayer.CurrentPlayerStats.MaxMana);
			double Ratio = ((((double) _currentScenePlayer.PostureController.CurrentPosture))) / (((double) _currentScenePlayer.CurrentPlayerStats.MaxPosture));
			GD.Print(Ratio);
			// Ratio = Mathf.Max(Ratio, 0);
			Ratio = 1.0 - Ratio;
			// Ratio *= 100.0;
			
			_postureBar.Value = Ratio * 100;
			
			if(Ratio == 0f) {
				_postureBar.Visible = false;
			} else {
				_postureBar.Visible = true;
			}
		}
	}
}
