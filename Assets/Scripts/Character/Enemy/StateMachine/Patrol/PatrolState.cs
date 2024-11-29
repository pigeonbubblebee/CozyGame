using Godot;
using System;

public partial class PatrolState : State
{
	protected PatrolStateMachine _patrolStateMachine => (PatrolStateMachine) ParentStateMachine;
	protected EnemyPatrolAI _patrolAI;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	public override void Initialize(StateMachine stateMachine) {
		base.Initialize(stateMachine);
		this._patrolAI = ((PatrolStateMachine) stateMachine).PatrolAI;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void PhysicsProcess(double delta)
	{
		_patrolAI.CheckLedge();
		_patrolAI.Accelerate();
		
		if(!_patrolAI.CanPatrol) {
			PatrolStateMachine patrolStateMachine = ((PatrolStateMachine) ParentStateMachine);
			patrolStateMachine.ChangeState(patrolStateMachine.IdleState);
		}
	}
	
	
}
