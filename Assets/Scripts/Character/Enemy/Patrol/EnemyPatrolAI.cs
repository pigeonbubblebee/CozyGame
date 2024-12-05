using Godot;
using System;

public partial class EnemyPatrolAI : Enemy
{
	
	
	[Export] public NodePath RayCastLedgePath { get; private set; }
	private RayCast2D _rayCastLedge;
	
	[Export] public NodePath RayCastWallPath { get; private set; }
	private RayCast2D _rayCastWall;
	
	[Export] public float Speed;
	[Export] public float WaitTimeBetweenFlips;
	private int moveDirection = 1;
	
	public bool CanPatrol = true;
	[Export] public bool Idle = false;
	
	public override void _Ready() {
		base._Ready();
		
		if(Idle) {
			CanPatrol = false;
			// Switch State To Idle
		}
		
		_rayCastLedge = GetNode<RayCast2D>(RayCastLedgePath);
		_rayCastWall = GetNode<RayCast2D>(RayCastWallPath);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		
		MoveAndSlide();
	}
	
	public bool CheckLedge(bool flip) {
		bool wallFront = false;
		
		if(_rayCastWall.IsColliding()) {
			if(_rayCastWall.GetCollider() is Node) {
				wallFront = ((Node)_rayCastWall.GetCollider()).IsInGroup("Ground");
			}
		}
		
		if((!_rayCastLedge.IsColliding() && IsOnFloor()) || wallFront) {
			if(!flip) {
				return true;
			}
			if(WaitTimeBetweenFlips > 0f) {
				CanPatrol = false;
				GetTree().CreateTimer(WaitTimeBetweenFlips).Timeout += _FinishWait;
			}
			return true;
		}
		
		return false;
	}
	
	private void _FinishWait() {
		Flip();
		CanPatrol = true; 
	}
	
	public void Accelerate() {
		this.Velocity = new Vector2(Speed * moveDirection, Velocity.Y);
	}
	
	public void Decelerate() {
		this.Velocity = new Vector2(0f, Velocity.Y);
	}
	
	public void Flip() {
		// GD.Print("Flip!");
		moveDirection = 0 - moveDirection;	
		// GD.Print(moveDirection);
		Scale = new Vector2(Mathf.Abs(Scale.X) * -1, Scale.Y);
	}
	
	public int GetMoveDirection() {
		return moveDirection;
	}
}
