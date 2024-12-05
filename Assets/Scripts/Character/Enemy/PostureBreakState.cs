using Godot;
using System;

public partial class PostureBreakState : State
{
	protected Enemy EnemyAI;
	
	public override void Initialize(StateMachine stateMachine) {
		base.Initialize(stateMachine);
		EnemyAI = ((EnemyStateMachine) stateMachine).EnemyAI;
		EnemyAI.StaggerRecoveryEvent += _enterDefaultState;
	}
	
	public override void Enter(State prev) {
		EnemyAI.Sprite.Play("stagger");
	}
	
	private void _enterDefaultState() {
		((EnemyStateMachine) ParentStateMachine).EnterDefaultState();
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void PhysicsProcess(double delta)
	{
		
	}
}
