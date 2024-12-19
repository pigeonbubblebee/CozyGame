using Godot;
using System;

public partial class EnemySwordsmanAI : EnemyPatrolAI
{
	[Export] public NodePath RayCastAggroForwardPath { get; private set; }
	private RayCast2D _rayCastAggroForward;
	[Export] public NodePath RayCastAggroUpPath { get; private set; }
	private RayCast2D _rayCastAggroUp;
	
	public event Action DeAggroEvent;
	[Export] public float DeAggroTime;
	[Export] public NodePath DeAggroTimerPath { get; private set; }
	private Timer _deAggroTimer;
	
	[Export] public bool CanAggro = true;
	
	public Vector2 LastKnownPlayerPosition { get; private set; }
	
	public Player TargetPlayer;

	[Export] public float StepRange;
	[Export] public float RetreatRange;
	
	public override void _Ready() {
		base._Ready();
		
		_rayCastAggroForward = GetNode<RayCast2D>(RayCastAggroForwardPath);
		_rayCastAggroUp = GetNode<RayCast2D>(RayCastAggroUpPath);
		_deAggroTimer = GetNode<Timer>(DeAggroTimerPath);
		_deAggroTimer.Timeout += DeAggroEvent;
	}
	
	public bool CheckAggro() {
		if(!CanAggro) {
			return false;
		}
		
		bool hit = false;
		
		if(GetRayCastPlayerHit(_rayCastAggroForward) || GetRayCastPlayerHit(_rayCastAggroUp)) {
			hit = true;
			LastKnownPlayerPosition = TargetPlayer.GlobalPosition;
		}
		// GD.Print(hit);
		
		if(!hit && _deAggroTimer.Paused) {
			_deAggroTimer.Start(DeAggroTime);
			_deAggroTimer.Paused = false;
		}
		if(hit) {
			_deAggroTimer.Paused = true;
		}
		
		return hit;
	}
	
	public override void _Process(double delta) {
		base._Process(delta);
		
		// GD.Print(_deAggroTimer.TimeLeft);
	}
	
	private bool GetRayCastPlayerHit(RayCast2D rayCast) {
		bool hit = false;
		if(rayCast.IsColliding()) {
			var collision = rayCast.GetCollider();
			if(collision is Node) {
				if(collision is Player) {
					TargetPlayer = (Player)collision;
					hit = true;
				}
			}
		}
		return hit;
	}
}
