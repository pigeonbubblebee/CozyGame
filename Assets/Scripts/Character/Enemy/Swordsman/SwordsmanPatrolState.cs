using Godot;
using System;

public partial class SwordsmanPatrolState : PatrolState
{
	protected SwordsmanStateMachine _swordsmanStateMachine => (SwordsmanStateMachine) ParentStateMachine;
	protected EnemySwordsmanAI _swordsmanAI;
	
	public override void Initialize(StateMachine stateMachine) {
		base.Initialize((PatrolStateMachine)stateMachine);
		this._swordsmanAI = ((SwordsmanStateMachine) stateMachine).SwordsmanAI;
		_swordsmanAI.TakeDamageEvent += _EnterAggroState;
	}
	
	private void _EnterAggroState(Player p, int d, int dir, int pD) {
		if(!ActiveState)
			return;
		_swordsmanAI.TargetPlayer = p;
		_swordsmanStateMachine.ChangeState(_swordsmanStateMachine.AggroState);
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
