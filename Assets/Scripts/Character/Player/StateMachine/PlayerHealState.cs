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

	public override void Process(double delta)
	{
		MovementController.AddFriction();
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
		ParentPlayerStateMachine.EnterDefaultState();
	}
}
