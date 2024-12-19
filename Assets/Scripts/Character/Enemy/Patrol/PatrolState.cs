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
	
	public override void Enter(State prev) {
		_patrolAI.Sprite.Play("patrol");
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void PhysicsProcess(double delta)
	{
		if(_patrolAI.Staggered) {
			_patrolStateMachine.ChangeState(_patrolStateMachine.PostureBreakState);
			return;
		}
		
		_patrolAI.CheckLedge(true);
		_patrolAI.Accelerate();
		_patrolAI.RegeneratePosture();
		
		if(!_patrolAI.CanPatrol) {
			PatrolStateMachine patrolStateMachine = ((PatrolStateMachine) ParentStateMachine);
			patrolStateMachine.ChangeState(patrolStateMachine.IdleState);
			return;
		}
	}
	
	
}
