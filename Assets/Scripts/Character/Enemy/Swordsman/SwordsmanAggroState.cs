using Godot;
using System;

public partial class SwordsmanAggroState : State
{
	protected SwordsmanStateMachine _swordsmanStateMachine => (SwordsmanStateMachine) ParentStateMachine;
	protected EnemySwordsmanAI _swordsmanAI;
	
	protected bool Running;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	public override void Initialize(StateMachine stateMachine) {
		base.Initialize(stateMachine);
		this._swordsmanAI = ((SwordsmanStateMachine) stateMachine).SwordsmanAI;
		_swordsmanAI.DeAggroEvent += _enterDefaultState;
	}
	
	public override void Enter(State prev) {
		_swordsmanAI.Sprite.Play("aggro");
		Running = true;
	}
	
	private void _enterDefaultState() {
		// GD.Print("DEAGGRO");
		_swordsmanStateMachine.ChangeState(_swordsmanStateMachine.IdleState);
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void PhysicsProcess(double delta)
	{
		if(_swordsmanAI.Staggered) {
			_swordsmanStateMachine.ChangeState(_swordsmanStateMachine.PostureBreakState);
			return;
		}
		
		_swordsmanAI.CheckAggro();
		EnemyAttack e = _swordsmanAI.GetCurrentAttack(_swordsmanAI.TargetPlayer);
		if(e != null) {
			ChangeDirection();
			_swordsmanStateMachine.ChangeState(_swordsmanStateMachine.AttackState);
			_swordsmanAI.ExecuteAttack(e, _swordsmanAI.TargetPlayer);
			_swordsmanAI.Decelerate();
			return;
		}
		// GD.Print(Mathf.Abs(_swordsmanAI.LastKnownPlayerPosition.X - _swordsmanAI.GlobalPosition.X) <= _swordsmanAI.SlashRange);
		//if(Mathf.Abs(_swordsmanAI.TargetPlayer.GlobalPosition.X - _swordsmanAI.GlobalPosition.X) <= _swordsmanAI.SlashRange && _swordsmanAI.CanSlash) { // Switch to Timer
		//	_swordsmanStateMachine.ChangeState(_swordsmanStateMachine.SlashState);
		//}
		
		/*
		if(Mathf.Abs(_swordsmanAI.TargetPlayer.GlobalPosition.X - _swordsmanAI.GlobalPosition.X) <= _swordsmanAI.SlashRange) {
			GD.Print("SLASH");
			_swordsmanStateMachine.ChangeState(_swordsmanStateMachine.IdleState);
		}
		*/
		if (Mathf.Abs(_swordsmanAI.GlobalPosition.X - _swordsmanAI.TargetPlayer.GlobalPosition.X) >= _swordsmanAI.StepRange) { // TODO: switch to step length
			ChangeDirection();
		}
		
		if(Mathf.Abs(_swordsmanAI.LastKnownPlayerPosition.X - _swordsmanAI.GlobalPosition.X) <= _swordsmanAI.RetreatRange) {
			if(Running) {
				_swordsmanAI.Sprite.Play("idle");
			}
			_swordsmanAI.Decelerate();
			Running = false;
		} else {
			if(!Running) {
				_swordsmanAI.Sprite.Play("aggro");
			}
			_swordsmanAI.Accelerate();
			Running = true;
		}
		
		if(_swordsmanAI.CheckLedge(false)) {
			if(Running) {
				_swordsmanAI.Sprite.Play("idle");
			}
			_swordsmanAI.Decelerate();
			Running = false;
			
		}
	}
	
	protected void ChangeDirection() {
		if(_swordsmanAI.GlobalPosition.X < _swordsmanAI.TargetPlayer.GlobalPosition.X && _swordsmanAI.GetMoveDirection() < 0) {
			_swordsmanAI.Flip();
		} else if(_swordsmanAI.GlobalPosition.X > _swordsmanAI.TargetPlayer.GlobalPosition.X && _swordsmanAI.GetMoveDirection() > 0) {
			_swordsmanAI.Flip();
			// _swordsmanAI.Scale = new Vector2(-Mathf.Abs(_swordsmanAI.Scale.X), _swordsmanAI.Scale.Y);
		}
	}
}
