using Godot;
using System;

public partial class Retreat : EnemyAttack
{
	[Export] public float RetreatRange { get; private set; }
	// [Export] public float RetreatTime { get; private set; }
	[Export] public float RetreatSpeed { get; private set; }
	// private Player _p;
	private bool _retreating = false;
	
	[Export] public NodePath RayCastLedgePath { get; private set; }
	private RayCast2D _rayCastLedge;
	
	[Export] public NodePath RayCastWallPath { get; private set; }
	private RayCast2D _rayCastWall;
	
	[Export] public bool FinishEarly { get; private set; } = false;
	
	public override void _Ready() {
		base._Ready();
		_rayCastLedge = GetNode<RayCast2D>(RayCastLedgePath);
		_rayCastWall = GetNode<RayCast2D>(RayCastWallPath);
	}
	
	public override bool GetCondition(Player p, Enemy e) {
		return (Mathf.Abs(p.GlobalPosition.X - e.GlobalPosition.X) <= RetreatRange) && CanAttack;
	}
	
	public override void Execute(Player p, Enemy e) {
		base.Execute(p, e);
		//this._p = p;
		StartRetreat();
	}
	
	public override void _Process(double delta) {
		base._Process(delta);
		
		if(!Active)
			return;
		if(_retreating) {
			if((Mathf.Abs(((EnemyChaserAI)EnemyAI).TargetPlayer.GlobalPosition.X - EnemyAI.GlobalPosition.X) <= RetreatRange)) {
				bool facingPlayer = false;
				if(((EnemyPatrolAI)EnemyAI).GetMoveDirection() > 0 && ((EnemyChaserAI)EnemyAI).TargetPlayer.GlobalPosition.X > EnemyAI.GlobalPosition.X)
					facingPlayer = true;
				else if(((EnemyPatrolAI)EnemyAI).GetMoveDirection() < 0 && ((EnemyChaserAI)EnemyAI).TargetPlayer.GlobalPosition.X < EnemyAI.GlobalPosition.X)
					facingPlayer = true;
					
				if(facingPlayer && !CheckLedge()) {
					_playAnimation(true);
					((EnemyPatrolAI)EnemyAI).Accelerate(-RetreatSpeed);
				} else {
					_playAnimation(false);
					((EnemyPatrolAI)EnemyAI).Decelerate();
				}
			} else {
				if(FinishEarly) {
					_FinishRetreat();
				}
				_playAnimation(false);
				((EnemyPatrolAI)EnemyAI).Decelerate();
			}
		} else {
			_playAnimation(false);
			((EnemyPatrolAI)EnemyAI).Decelerate();
		}
	}
	
	private void _playAnimation(bool accel) {
		if(!Active)
			return;
		
		if(accel) {
			if(EnemyAI.Sprite.Animation != AnimationName) {
				EnemyAI.Sprite.Play(AnimationName);
			}
		} else {
			if(EnemyAI.Sprite.Animation == AnimationName) {
				EnemyAI.Sprite.Play("idle");
			}
		}
	}
	
	public void StartRetreat() {
		_retreating = true;
		GetTree().CreateTimer(AttackLength).Timeout += _FinishRetreat;
	}
	
	public void _FinishRetreat() {
		((EnemyPatrolAI)EnemyAI).Decelerate();
		_retreating = false;
		Finish();
	}
	
	public override void Interrupt() {
		_retreating = false;
		// GD.Print("Interrupt");
		base.Interrupt();
	}
	
	public bool CheckLedge() {
		bool wallFront = false;
		
		if(_rayCastWall.IsColliding()) {
			if(_rayCastWall.GetCollider() is Node) {
				wallFront = ((Node)_rayCastWall.GetCollider()).IsInGroup("Ground");
			}
		}
		
		if((!_rayCastLedge.IsColliding() && EnemyAI.IsOnFloor()) || wallFront) {
			return true;
		}
		
		return false;
	}
}
