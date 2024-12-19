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
	
	public override void Enter(State prev) {
		// GD.Print(_patrolAI.Sprite);
		_patrolAI.Sprite.Play("idle");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void PhysicsProcess(double delta)
	{
		if(_patrolAI.Staggered) {
			_patrolStateMachine.ChangeState(_patrolStateMachine.PostureBreakState);
			return;
		}
		
		_patrolAI.Decelerate();
		_patrolAI.RegeneratePosture();
		
		if(_patrolAI.CanPatrol) {
			PatrolStateMachine patrolStateMachine = ((PatrolStateMachine) ParentStateMachine);
			patrolStateMachine.ChangeState(patrolStateMachine.PatrolState);
			return;
		}
	}
	
	
}
