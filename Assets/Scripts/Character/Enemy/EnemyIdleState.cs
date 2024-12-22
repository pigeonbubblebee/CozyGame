using Godot;
using System;

public partial class EnemyIdleState : State
{
	protected EnemyStateMachine _enemyStateMachine => (EnemyStateMachine) ParentStateMachine;
	protected Enemy EnemyAI;
	
	public override void Initialize(StateMachine stateMachine) {
		base.Initialize(stateMachine);
		EnemyAI = ((EnemyStateMachine) stateMachine).EnemyAI;
	}
	
	public override void Enter(State prev) {
		// GD.Print(_patrolAI.Sprite);
		EnemyAI.Sprite.Play("idle");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void PhysicsProcess(double delta)
	{
		if(EnemyAI.Staggered) {
			_enemyStateMachine.ChangeState(_enemyStateMachine.PostureBreakState);
			return;
		}
	}
}
