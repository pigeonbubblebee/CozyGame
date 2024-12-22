using Godot;
using System;

public partial class VunerableAttack : EnemyAttack
{
	[Export] public float Cooldown;
	public bool CanVunerable = true;
	
	public override void Initialize(Enemy e) {
		base.Initialize(e);
		// EnemyAI.Sprite.FrameChanged += _checkAnimationEvent;
	}
	
	public override bool GetCondition(Player p, Enemy e) {
		return CanVunerable;
	}
	
	public override void Execute(Player p, Enemy e) {
		base.Execute(p, e);

		StartVunerableCooldown();
	}
	
	public void BecomeVunerable() {
		GetTree().CreateTimer(AttackLength).Timeout += _Finish;
	}
	
	private void _Finish() {
		GD.Print("Finished");
		Finish();
	}

	public void StartVunerableCooldown() {
		CanVunerable = false;
		GetTree().CreateTimer(Cooldown).Timeout += _FinishVunerableCooldown;
	}

	private void _FinishVunerableCooldown() {
		CanVunerable = true; 
	}
}
