using Godot;
using System;

public partial class PatrolStaggerState : PostureBreakState
{
	public override void PhysicsProcess(double delta)
	{
		((EnemyPatrolAI)EnemyAI).Decelerate();
	}
}
