using Godot;
using System;

public partial class AttackState : State
{
	//protected SwordsmanStateMachine _swordsmanStateMachine => (SwordsmanStateMachine) ParentStateMachine;
	//protected EnemySwordsmanAI _swordsmanAI;
	protected Enemy EnemyAI;
	
	public override void Initialize(StateMachine stateMachine) {
		base.Initialize(stateMachine);
		EnemyAI = ((EnemyStateMachine) stateMachine).EnemyAI;
		EnemyAI.FinishAttackEvent += _enterDefaultState;
		//this._swordsmanAI = ((SwordsmanStateMachine) stateMachine).SwordsmanAI;
		//_swordsmanAI.FinishSlashEvent += _enterDefaultState;
	}
	
	public override void Enter(State prev) {
		
	}
	
	private void _enterDefaultState() {
		if(ActiveState)
			((EnemyStateMachine) ParentStateMachine).EnterDefaultState(true);
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void PhysicsProcess(double delta)
	{
		if(EnemyAI.Staggered) {
			// GD.Print(EnemyAI.CurrentAttack);
			if(EnemyAI.CurrentAttack != null)
				EnemyAI.CurrentAttack.Interrupt();
			((EnemyStateMachine) ParentStateMachine).ChangeState(((EnemyStateMachine) ParentStateMachine).PostureBreakState);
			return;
		}
	}
}
