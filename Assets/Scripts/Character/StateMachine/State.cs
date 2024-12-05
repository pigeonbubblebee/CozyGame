using Godot;
using System;

public abstract partial class State : Node
{
	protected StateMachine ParentStateMachine;
	public bool ActiveState = false;
	
	public virtual void Initialize(StateMachine stateMachine) {
		ParentStateMachine = stateMachine;
	}

	public virtual void Enter(State previousState) {
			
	}

	public virtual void Process(double delta) {

	}

	public virtual void PhysicsProcess(double delta) {

	}

	public virtual void Exit() {

	}
}
