using Godot;
using System;

public partial class PlayerHealState : PlayerState
{
	public override void Enter(State previousState) {
		base.Enter(previousState);
		
		HealController.StartHeal();
		HealController.StartHealCooldown();
	}
	
	public override void Initialize(StateMachine playerStateMachine) {
		base.Initialize(playerStateMachine);

		HealController.FinishHealChargeEvent += _EnterDefaultState;
	}

	public override void PlayStateAnimation() {
		AnimationController.PlayAnimation(AnimationController.HealAnimationClip, Stats.HealTime);
	}


	public override void Process(double delta)
	{
		MovementController.AddFriction(delta);
		// GD.Print("HEAL");
	}

	public override void PhysicsProcess(double delta) {
		base.PhysicsProcess(delta);
	}
	
	protected override bool CheckStates()
	{
		if(!HealController.DesiredHeal) {
			HealController.InterruptHeal();
			ParentPlayerStateMachine.EnterDefaultState();
			return true;
		}
		
		return false;
	}
	
	private void _EnterDefaultState() {
		if(!ActiveState)
			return;
		ParentPlayerStateMachine.EnterDefaultState();
	}
}
