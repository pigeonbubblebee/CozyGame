using Godot;
using System;

public partial class PatrolStateMachine : EnemyStateMachine
{
	// private IInputManager _inputManager;
	
	[Export] private NodePath _patrolStatePath;
	public PatrolState PatrolState { get; private set; }
	
	public EnemyPatrolAI PatrolAI => (EnemyPatrolAI) EnemyAI;

	/*
	public int NextPatrolPoint() {
		return PatrolAI.NextPatrolPoint();
	}
	*/
	
	public override void Initialize(Enemy e) {
		base.Initialize(e);
		// PatrolAI = (EnemyPatrolAI) e;
		// Sprite = PatrolAI.Sprite;
	}
	
	public override void _Ready() {
		// _inputManager = GetNode<IInputManager>("/root/InputManager");
		base._Ready();
		
		PatrolState = GetNode<PatrolState>(_patrolStatePath);
		PatrolState.Initialize(this);
		
		EnterDefaultState();
	}
	
	public override bool EnterDefaultState() {
		ChangeState(IdleState);

		return true;
	}
}
