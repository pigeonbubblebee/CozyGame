using Godot;
using System;

public partial class EnemyPatrolAI : Enemy
{
	[Export] public NodePath StateMachinePath;
	private PatrolStateMachine _stateMachine;
	
	[Export] public NodePath RayCastLedgePath;
	private RayCast2D _rayCastLedge;
	
	[Export] public float Speed;
	[Export] public float WaitTimeBetweenFlips;
	private int moveDirection = 1;
	
	public bool CanPatrol = true;
	[Export] public bool Idle = false;
	
	public override void _EnterTree() {
		_stateMachine = GetNode<PatrolStateMachine>(StateMachinePath);
		_stateMachine.Initialize(this);
	}
	
	public override void _Ready() {
		base._Ready();
		
		if(Idle) {
			CanPatrol = false;
			// Switch State To Idle
		}
		
		_rayCastLedge = GetNode<RayCast2D>(RayCastLedgePath);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		
		MoveAndSlide();
	}
	
	public void CheckLedge() {
		if(!_rayCastLedge.IsColliding() && IsOnFloor()) {
			Flip();
			
			if(WaitTimeBetweenFlips > 0f) {
				CanPatrol = false;
				GetTree().CreateTimer(WaitTimeBetweenFlips).Timeout += _FinishWait;
			}
		}
	}
	
	private void _FinishWait() {
		CanPatrol = true; 
	}
	
	public void Accelerate() {
		this.Velocity = new Vector2(Speed * moveDirection, Velocity.Y);
	}
	
	public void Decelerate() {
		this.Velocity = new Vector2(0f, Velocity.Y);
	}
	
	protected void Flip() {
		moveDirection = 0 - moveDirection;	
		GD.Print(moveDirection);
		Scale = new Vector2(Mathf.Abs(Scale.X) * -1, Scale.Y);
	}
}
