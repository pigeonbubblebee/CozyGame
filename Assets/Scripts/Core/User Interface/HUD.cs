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
	[Export] private NodePath _internalHealthBarPath;
	private TextureProgressBar _internalHealthBar;
	
	[Export] private NodePath _healthHUDPath;
	private HealthHUD _healthHUD;
	
	private Player _currentScenePlayer;
	
	private RichTextLabel _moneyText;
	[Export] private NodePath _moneyTextPath;

	public override void _Ready() {
		_healBar = GetNode<TextureProgressBar>(_healBarPath);
		_manaBar = GetNode<TextureProgressBar>(_manaBarPath);
		_postureBar = GetNode<TextureProgressBar>(_postureBarPath);
		_internalHealthBar = GetNode<TextureProgressBar>(_internalHealthBarPath);
		_healthHUD = GetNode<HealthHUD>(_healthHUDPath);

		_moneyText = GetNode<RichTextLabel>(_moneyTextPath);
	}
	
	public void SetCurrentPlayerInstance(Player p) {
		_currentScenePlayer = p;
		
		p.PlayerHealth.DamageEvent += _UpdateManaBar;
		p.PlayerHealth.HealEvent += _UpdateManaBar;
		
		p.HealController.HealEvent += _UpdateHealBar;
		p.HealController.AddHealEvent += _UpdateHealBar;
		
		p.SpellController.ManaUseEvent += _UpdateHealthHUD;
		p.SpellController.AddManaEvent += _UpdateHealthHUD;
		
		p.CurseController.CurseChangeEvent += _UpdatePostureBar;
		
		p.HealController.InternalHealthChangeEvent += _UpdateInternalHealthBar;
		// p.SpellController.AddManaEvent += _UpdateHealthHUD;
		
		_UpdateHealthHUD(0);
		_UpdateHealBar(p);
		_UpdateManaBar(0);
		_UpdatePostureBar(0);
		_UpdateInternalHealthBar(0);
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
		if(_currentScenePlayer != null) {
			_moneyText.Text = "[right]" + _currentScenePlayer.InventoryManager.GetItemCount("acorn").ToString();
		}
    }

	
	private void _UpdateHealthHUD(int amt) {
		if(_currentScenePlayer != null) {
			// _healthHUD.UpdateHealth(_currentScenePlayer.PlayerHealth.CurrentHealthPoints, _currentScenePlayer.CurrentPlayerStats.MaxHealth);
			_healthHUD.UpdateHealth(_currentScenePlayer.SpellController.CurrentMana, _currentScenePlayer.CurrentPlayerStats.MaxMana);
		}
	}
	
	private void _UpdateHealBar(Player p) {
		if(_currentScenePlayer != null) {
			
			float Ratio = ((float) _currentScenePlayer.HealController.CurrentHeals) / ((float) _currentScenePlayer.CurrentPlayerStats.MaxHeals);
			Ratio = Mathf.Max(Ratio, 0);
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(_healBar, "value", Ratio * 100, 0.075f);
			// o_healBar.Value = Ratio * 100;
		}
	}
	
	private void _UpdateManaBar(int amt) {
		if(_currentScenePlayer != null) {
			// float Ratio = ((float) _currentScenePlayer.SpellController.CurrentMana) / ((float) _currentScenePlayer.CurrentPlayerStats.MaxMana);
			float Ratio = ((float) _currentScenePlayer.PlayerHealth.CurrentHealthPoints) / ((float) _currentScenePlayer.PlayerHealth.MaxHealthPoints);
			Ratio = Mathf.Max(Ratio, 0);
			
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(_manaBar, "value", Ratio * 100, 0.075f);

			_manaBar.Size = new Vector2(_currentScenePlayer.PlayerHealth.MaxHealthPoints * 1.5f, _manaBar.Size.Y);
			
			// _manaBar.Value = Ratio * 100;
			
			_UpdateInternalHealthBar(0);
		}
	}
	
	private void _UpdateInternalHealthBar(int amt) {
		if(_currentScenePlayer != null) {
			// float Ratio = ((float) _currentScenePlayer.SpellController.CurrentMana) / ((float) _currentScenePlayer.CurrentPlayerStats.MaxMana);
			float Ratio = ((float) _currentScenePlayer.PlayerHealth.CurrentHealthPoints) / ((float) _currentScenePlayer.PlayerHealth.MaxHealthPoints);
			Ratio = Mathf.Max(Ratio, 0);
			
			float SecondRatio = ((float) _currentScenePlayer.HealController.InternalDamage) / ((float) _currentScenePlayer.PlayerHealth.MaxHealthPoints);
			SecondRatio = Mathf.Max(SecondRatio, 0);

			_internalHealthBar.Size = new Vector2(_currentScenePlayer.PlayerHealth.MaxHealthPoints * 1.5f, _internalHealthBar.Size.Y);
			
			// Tween tween = GetTree().CreateTween();
			// tween.TweenProperty(_internalHealthBar, "value", (Ratio+SecondRatio) * 100, 0.075f);
			_internalHealthBar.Value = (Ratio+SecondRatio) * 100;
		}
	}
	
	private void _UpdatePostureBar(int amt) {
		if(_currentScenePlayer != null) {
			// float Ratio = ((float) _currentScenePlayer.SpellController.CurrentMana) / ((float) _currentScenePlayer.CurrentPlayerStats.MaxMana);
			double Ratio = ((((double) _currentScenePlayer.CurseController.CurrentCurse))) / (((double) _currentScenePlayer.CurseController.MaxCurse));
			//GD.Print(Ratio);
			// Ratio = Mathf.Max(Ratio, 0);
			//Ratio =  Ratio;
			// Ratio *= 100.0;
			
			Tween tween = GetTree().CreateTween();
			
			tween.TweenProperty(_postureBar, "value", (Ratio) * 100, 0.075f);

			_postureBar.Size = new Vector2(_currentScenePlayer.CurseController.MaxCurse, _postureBar.Size.Y);
			// _postureBar.Value = Ratio * 100;
		}
	}
}
