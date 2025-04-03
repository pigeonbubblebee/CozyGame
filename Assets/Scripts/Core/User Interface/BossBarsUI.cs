using Godot;
using System;

public partial class BossBarsUI : Control
{
	[Export] private NodePath _postureBarPath;
	private TextureProgressBar _postureBar;
	[Export] private NodePath _healthBarPath;
	private TextureProgressBar _healthBar;
	
	[Export] private NodePath _bossNamePath;
	private RichTextLabel _bossName;
	
	public Enemy Boss;
	
	private double tweeningValueHealth;
	private double tweeningValuePosture;
	
	public override void _Ready() {
		_postureBar = GetNode<TextureProgressBar>(_postureBarPath);
		_healthBar = GetNode<TextureProgressBar>(_healthBarPath);
		_bossName = GetNode<RichTextLabel>(_bossNamePath);
	}
	
	public void SetBoss(Enemy e) {
		Boss = e;
		tweeningValueHealth = ((double)Boss.GetCurrentHealth() / (double)Boss.MaxHealth) * 100;
		tweeningValuePosture =  100 - (((double)Boss.CurrentPosture / (double)Boss.MaxPosture) * 100);

		_healthBar.Value = tweeningValueHealth;
		_postureBar.Value = tweeningValuePosture;
	}
	
	public override void _Process(double delta) {
		// GD.Print(Boss);
		if(Boss == null) {
			return;
		}
		double Ratio = ((double)Boss.GetCurrentHealth() / (double)Boss.MaxHealth) * 100;
		
		if(Ratio != tweeningValueHealth) {
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(_healthBar, "value", Ratio, 0.075f);
			tweeningValueHealth = Ratio;
		}
		
		Ratio = 100 - (((double)Boss.CurrentPosture / (double)Boss.MaxPosture) * 100);
		
		if(Ratio != tweeningValuePosture) {
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(_postureBar, "value", Ratio, 0.075f);
			tweeningValuePosture = Ratio;
		}
		// _healthBar.Value = ((double)Boss.GetCurrentHealth() / (double)Boss.MaxHealth) * 100;
		// _postureBar.Value = 100 - (((double)Boss.CurrentPosture / (double)Boss.MaxPosture) * 100);
		_bossName.Text =  "[center]" + Tr(Boss.EnemyNameLocalizationKey) + "[/center]"; // switch to localized value
	}
}
