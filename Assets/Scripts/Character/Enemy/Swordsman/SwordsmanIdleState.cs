using Godot;
using System;

public partial class SwordsmanIdleState : PatrolIdleState
{
	protected SwordsmanStateMachine _swordsmanStateMachine => (SwordsmanStateMachine) ParentStateMachine;
	protected EnemySwordsmanAI _swordsmanAI;
	
	public override void Initialize(StateMachine stateMachine) {
		base.Initialize((PatrolStateMachine)stateMachine);
		this._swordsmanAI = ((SwordsmanStateMachine) stateMachine).SwordsmanAI;
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void PhysicsProcess(double delta)
	{
		base.PhysicsProcess(delta);
		
		if(_swordsmanAI.CheckAggro()) {
			//_swordsmanAI.CanAggro = false;
			_swordsmanStateMachine.ChangeState(_swordsmanStateMachine.AggroState);
		}
	}
}
