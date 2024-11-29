using Godot;
using System;

public partial class PatrolIdleState : State
{
	protected PatrolStateMachine _patrolStateMachine => (PatrolStateMachine) ParentStateMachine;
	protected EnemyPatrolAI _patrolAI;

	public override void Initialize(StateMachine stateMachine) {
		base.Initialize(stateMachine);
		this._patrolAI = ((PatrolStateMachine) stateMachine).PatrolAI;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void PhysicsProcess(double delta)
	{
		_patrolAI.Decelerate();
		
		if(_patrolAI.CanPatrol) {
			PatrolStateMachine patrolStateMachine = ((PatrolStateMachine) ParentStateMachine);
			patrolStateMachine.ChangeState(patrolStateMachine.PatrolState);
		}
	}
	
	
}
