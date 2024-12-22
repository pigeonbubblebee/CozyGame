using Godot;
using System;

public partial class ChaserAggroState : State
{
	protected ChaserStateMachine _chaserStateMachine => (ChaserStateMachine) ParentStateMachine;
	protected EnemyChaserAI _chaserAI;
	
	protected bool Running;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	public override void Initialize(StateMachine stateMachine) {
		base.Initialize(stateMachine);
		this._chaserAI = ((ChaserStateMachine) stateMachine).ChaserAI;
		_chaserAI.DeAggroEvent += _enterDefaultState;
	}
	
	public override void Enter(State prev) {
		_chaserAI.Sprite.Play("aggro");
		Running = true;
	}
	
	private void _enterDefaultState() {
		// GD.Print("DEAGGRO");
		_chaserStateMachine.ChangeState(_chaserStateMachine.IdleState);
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void PhysicsProcess(double delta)
	{
		if(_chaserAI.Staggered) {
			_chaserStateMachine.ChangeState(_chaserStateMachine.PostureBreakState);
			return;
		}
		
		
		_chaserAI.CheckAggro();
		EnemyAttack e = _chaserAI.GetCurrentAttack(_chaserAI.TargetPlayer);
		if(e != null) {
			ChangeDirection();
			_chaserStateMachine.ChangeState(_chaserStateMachine.AttackState);
			_chaserAI.ExecuteAttack(e, _chaserAI.TargetPlayer);
			_chaserAI.Decelerate();
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
		if (Mathf.Abs(_chaserAI.GlobalPosition.X - _chaserAI.TargetPlayer.GlobalPosition.X) >= _chaserAI.StepRange) { // TODO: switch to step length
			ChangeDirection();
		}
		
		if(Mathf.Abs(_chaserAI.LastKnownPlayerPosition.X - _chaserAI.GlobalPosition.X) <= _chaserAI.RetreatRange) {
			if(Running) {
				_chaserAI.Sprite.Play("idle");
			}
			_chaserAI.Decelerate();
			Running = false;
		} else {
			if(!Running) {
				_chaserAI.Sprite.Play("aggro");
			}
			_chaserAI.Accelerate();
			
			Running = true;
		}
		
		if(Mathf.Abs(_chaserAI.LastKnownPlayerPosition.X - _chaserAI.GlobalPosition.X) >= _chaserAI.PostureRegenerateDistance)
			_chaserAI.RegeneratePosture();
		
		if(_chaserAI.CheckLedge(false)) {
			if(Running) {
				_chaserAI.Sprite.Play("idle");
			}
			_chaserAI.Decelerate();
			Running = false;
			
		}
	}
	
	protected void ChangeDirection() {
		if(_chaserAI.GlobalPosition.X < _chaserAI.TargetPlayer.GlobalPosition.X && _chaserAI.GetMoveDirection() < 0) {
			_chaserAI.Flip();
		} else if(_chaserAI.GlobalPosition.X > _chaserAI.TargetPlayer.GlobalPosition.X && _chaserAI.GetMoveDirection() > 0) {
			_chaserAI.Flip();
			// _swordsmanAI.Scale = new Vector2(-Mathf.Abs(_swordsmanAI.Scale.X), _swordsmanAI.Scale.Y);
		}
	}
}
