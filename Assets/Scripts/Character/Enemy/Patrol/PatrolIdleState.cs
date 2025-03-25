using Godot;
using System;

public partial class PatrolIdleState : EnemyIdleState
{
	protected PatrolStateMachine _patrolStateMachine => (PatrolStateMachine) ParentStateMachine;
	protected EnemyPatrolAI _patrolAI;

	public override void Initialize(StateMachine stateMachine) {
		base.Initialize(stateMachine);
		this._patrolAI = ((PatrolStateMachine) stateMachine).PatrolAI;
	}

    public override void Enter(State prev)
    {
        base.Enter(prev);

		// _patrolStateMachine.PatrolState.GetPatrolSFX().StreamPaused = false;
    }

    public override void Exit()
    {
        base.Exit();
		// _patrolStateMachine.PatrolState.GetPatrolSFX().StreamPaused = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void PhysicsProcess(double delta)
	{
		base.PhysicsProcess(delta);
		
		_patrolAI.Decelerate();
		_patrolAI.RegeneratePosture();
		
		if(_patrolAI.CanPatrol) {
			PatrolStateMachine patrolStateMachine = ((PatrolStateMachine) ParentStateMachine);
			patrolStateMachine.ChangeState(patrolStateMachine.PatrolState);
			return;
		}
	}
	
	
}
