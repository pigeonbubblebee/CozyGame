using Godot;
using System;

public partial class PatrolStateMachine : StateMachine
{
	// private IInputManager _inputManager;
	
	[Export] private NodePath _patrolStatePath;
	public PatrolState PatrolState { get; private set; }
	[Export] private NodePath _idleStatePath;
	public PatrolIdleState IdleState { get; private set; }
	
	public EnemyPatrolAI PatrolAI { get; private set; }
	/*
	public int NextPatrolPoint() {
		return PatrolAI.NextPatrolPoint();
	}
	*/
	
	public void Initialize(EnemyPatrolAI e) {
		PatrolAI = e;
	}
	
	public override void _Ready() {
		// _inputManager = GetNode<IInputManager>("/root/InputManager");

		PatrolState = GetNode<PatrolState>(_patrolStatePath);
		PatrolState.Initialize(this);
		IdleState = GetNode<PatrolIdleState>(_idleStatePath);
		IdleState.Initialize(this);
		
		EnterDefaultState();
	}
	
	public void PlayStateAnimation() {
		
	}
	
	public bool EnterDefaultState() {
		ChangeState(PatrolState);

		return true;
	}
}
