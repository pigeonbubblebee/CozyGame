using Godot;
using System;

public partial class ChaserStateMachine : PatrolStateMachine
{
	[Export] private NodePath _aggroStatePath;
	public ChaserAggroState AggroState { get; private set; }
	
	public EnemyChaserAI ChaserAI => (EnemyChaserAI)PatrolAI;
	
	public void Initialize(EnemyChaserAI e) {
		base.Initialize((EnemyPatrolAI)e);
	}
	
	public override void _Ready() {
		AggroState = GetNode<ChaserAggroState>(_aggroStatePath);
		AggroState.Initialize(this);
		
		base._Ready();
	}
	
	public override bool EnterDefaultState(bool fromAttack) {
		if(fromAttack) {
			// GD.Print("from attack switch");
			ChangeState(AggroState);
			return true;
		}
		ChangeState(IdleState);
		return false;
	}
}
