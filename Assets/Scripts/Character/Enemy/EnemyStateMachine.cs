using Godot;
using System;

public partial class EnemyStateMachine : StateMachine
{
	[Export] private NodePath _attackStatePath;
	public AttackState AttackState { get; private set; }
	
	public Enemy EnemyAI { get; private set; }
	public AnimatedSprite2D Sprite { get; private set; }
	/*
	public int NextPatrolPoint() {
		return PatrolAI.NextPatrolPoint();
	}
	*/
	
	public virtual void Initialize(Enemy e) {
		// ase.Initialize(e);
		EnemyAI = e;
		Sprite = EnemyAI.Sprite;
	}
	
	public override void _Ready() {
		// _inputManager = GetNode<IInputManager>("/root/InputManager");
		base._Ready();
		
		AttackState = GetNode<AttackState>(_attackStatePath);
		AttackState.Initialize(this);
		
		// EnterDefaultState();
	}
	
	public void PlayStateAnimation() {
		
	}
	
	public virtual bool EnterDefaultState() {
		return false;
	}
}
