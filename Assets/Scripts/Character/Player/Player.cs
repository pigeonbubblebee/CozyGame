using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public PlayerStateMachine StateMachine;
	[Export] public PlayerMovementController MovementController;
	public override void _EnterTree()
	{
		base._EnterTree();

		StateMachine.Initialize(MovementController);
		MovementController.Initialize(this);
	}
}
