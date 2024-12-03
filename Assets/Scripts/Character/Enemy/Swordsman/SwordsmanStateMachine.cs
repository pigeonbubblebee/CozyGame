using Godot;
using System;

public partial class SwordsmanStateMachine : PatrolStateMachine
{
	[Export] private NodePath _aggroStatePath;
	public SwordsmanAggroState AggroState { get; private set; }
	
	public EnemySwordsmanAI SwordsmanAI => (EnemySwordsmanAI)PatrolAI;
	
	public void Initialize(EnemySwordsmanAI e) {
		base.Initialize((EnemyPatrolAI)e);
	}
	
	public override void _Ready() {
		AggroState = GetNode<SwordsmanAggroState>(_aggroStatePath);
		AggroState.Initialize(this);
		
		base._Ready();
	}
}
