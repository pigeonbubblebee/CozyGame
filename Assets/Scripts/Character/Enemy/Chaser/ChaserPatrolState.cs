using Godot;
using System;

public partial class ChaserPatrolState : PatrolState
{
	protected ChaserStateMachine _chaserStateMachine => (ChaserStateMachine) ParentStateMachine;
	protected EnemyChaserAI _chaserAI;
	
	public override void Initialize(StateMachine stateMachine) {
		base.Initialize((PatrolStateMachine)stateMachine);
		this._chaserAI = ((ChaserStateMachine) stateMachine).ChaserAI;
		_chaserAI.TakeDamageEvent += _EnterAggroState;
	}
	
	private void _EnterAggroState(Player p, int d, int dir, int pD) {
		if(!ActiveState)
			return;
		_chaserAI.TargetPlayer = p;
		_chaserStateMachine.ChangeState(_chaserStateMachine.AggroState);
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void PhysicsProcess(double delta)
	{
		base.PhysicsProcess(delta);
		
		if(_chaserAI.CheckAggro()) {
			// GD.Print("patrol switch");
			//_chaserAI.CanAggro = false;
			_chaserStateMachine.ChangeState(_chaserStateMachine.AggroState);
		}
	}
}
