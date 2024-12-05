using Godot;
using System;

public abstract partial class StateMachine : Node
{
	public State CurrentState { get; private set; }
	[Export] private bool _logStates;

	public override void _Process(double delta) {
		if(CurrentState != null)
			CurrentState.Process(delta);
	}

	public override void _PhysicsProcess(double delta) {
		if(CurrentState != null)
			CurrentState.PhysicsProcess(delta);
	}

	public void ChangeState(State newState) {
		if(_logStates)
			GD.Print(newState.Name);

		State previousState = CurrentState;

		if(CurrentState != null) {
			CurrentState.Exit();
			CurrentState.ActiveState = false;
		}

		CurrentState = newState;
		
		CurrentState.Enter(previousState);
		CurrentState.ActiveState = true;
	}
}
