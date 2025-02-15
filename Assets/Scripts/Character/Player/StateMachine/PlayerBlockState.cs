using Godot;
using System;

public partial class PlayerBlockState : PlayerState
{
	private bool _released = true;
	private bool _deflecting = false;
	
	public bool HitTransition = false;
	/*
	private readonly int FALLING = -1;
	private readonly int NEUTRAL = 0;
	private readonly int JUMPING = 1;
	private int _subState;
	*/
		
	[Export] private NodePath _deflectTimerPath;
	private Timer _deflectTimer;
	
	public override void Initialize(StateMachine playerStateMachine) {
		base.Initialize(playerStateMachine);
		
		_deflectTimer = GetNode<Timer>(_deflectTimerPath);
		DeflectController.BlockEvent += _checkDeflect;
		
		_deflectTimer.Timeout += _EnterDefaultState;
	}
	
	private void _checkDeflect(bool deflect, int postureDamage, Enemy e) {
		if(deflect)
			_deflecting = true;
		//if(!deflect)
		// 	return;
		//_timeoutBlock(deflect);
		//_released = true;
		// DeflectController.StartBlockCooldown();
		HitTransition = true;
		GetTree().CreateTimer(DeflectController.BlockCancelTime()).Timeout += _EnterDefaultState;
	}
	
	private void _timeoutBlock() {
		// GD.Print("Timeout");
		_deflectTimer.Start(Stats.BlockTimeout);
		
		/*if(deflect) {
			GetTree().CreateTimer(Stats.DeflectTimeout).Timeout += _EnterDefaultState; // Move to stats
		} else {
			// if(deflect) {
			GetTree().CreateTimer(Stats.BlockTimeout).Timeout += _EnterDefaultState; // Move to stats
		}*/
	}
	
	public override void Exit() {
		base.Exit();
		// DeflectController.StartBlockCooldown();
		DeflectController.EndBlock();
	}
	
	public override void PlayStateAnimation() {
		AnimationController.PlayAnimation(AnimationController.BlockAnimationClip);
		// TODO: Start Idle Animation
	}
	
	
	protected override bool CheckStates()
	{
		if(PostureController.CurrentPosture <= 0) {
			MovementController.CanSwitchDirections = true;
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.StaggerState);
			return true;
		}
		if(DeflectController.BlockCancellable()) {
			if(DeflectController.DeflectActuation || (!DeflectController.GetDeflectBufferStop())) {
				// GD.Print("BlockCancellable");
				_deflectTimer.Stop();
				ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.BlockState);
				DeflectController.StartBlock();
				AnimationController.PlayAnimation(AnimationController.BlockAnimationClip);
				return true;
			}
			if(HealController.DesiredHeal && HealController.HealCooldownOff) {
				MovementController.CanSwitchDirections = true;
				_deflectTimer.Stop();
				ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.HealState);
				return true;
			}
			if(MovementController.DesiredDash || (!MovementController.GetDashBufferStop() && MovementController.CanDash)) {
				MovementController.CanSwitchDirections = true;
				_deflectTimer.Stop();
				ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.DashState);
				return true;
			}
			if(SpellController.DesiredShoot || (!SpellController.GetShootBufferStop() && SpellController.CanShoot)) {
				MovementController.CanSwitchDirections = true;
				_deflectTimer.Stop();
				ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.SpellState);
				return true;
			}
			if(AttackController.DesiredAttack || (!AttackController.GetSlashBufferStop() && AttackController.CanSlash)) {
				MovementController.CanSwitchDirections = true;
				//GD.Print("Block Cancelled!");
				_deflectTimer.Stop();
				ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.AttackState);
				return true;
			}
			if(MovementController.DesiredJump || !MovementController.GetJumpBufferStop()) {
				MovementController.CanSwitchDirections = true;
				_deflectTimer.Stop();
				ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.JumpState);
				return true;
			}
		}
		/*
		if(!DeflectController.DesiredDeflect) {
			DeflectController.EndBlock();
			_timeoutBlock(_deflecting);
			_released = true;
			return true;
		}
		*/
		return false;
	}

	public override void Enter(State previousState) {
		base.Enter(previousState);
		_timeoutBlock();
		DeflectController.StartDeflectWindow();
		
		HitTransition = false;
		
		_released = false;
		CanFlip = false;
		MovementController.CanSwitchDirections = false;
		_deflecting = false;
		/*
		if(previousState is PlayerJumpState) {
			_subState = JUMPING;
		} else if (previousState is PlayerFallState) {
			_subState = FALLING;
		} else {
			_subState = NEUTRAL;
		}
		*/
	}

	public override void Process(double delta)
	{
		if(AttackController.DesiredAttack) {
			AttackController.StartSlashBuffer();
		}
		if(MovementController.DesiredDash) {
			MovementController.StartDashBuffer();
		}
		if(SpellController.DesiredShoot) {
			SpellController.StartShootBuffer();
		}
		if(DeflectController.DeflectActuation) {
			DeflectController.StartDeflectBuffer();
		}
		/*
		_HandleVerticalChanges();
	
		if(_subState == JUMPING || _subState == FALLING) {
			if(MovementController.DesiredJump) {
				MovementController.StartJumpBuffer();
			}
		}

		if(AttackController.DesiredAttack) {
			AttackController.StartSlashBuffer();
		}
		if(MovementController.DesiredDash) {
			MovementController.StartDashBuffer();
		}
		if(SpellController.DesiredShoot) {
			SpellController.StartShootBuffer();
		}
		*/
	}

	 public override void PhysicsProcess(double delta) {
		base.PhysicsProcess(delta);
		
		_HandleHorizontalMovement(delta);
		// _HandleVerticalMovement(delta);
		
		if(!MovementController.Grounded) {
			MovementController.Fall(delta);
		}
	}
	
	private void _HandleHorizontalMovement(double delta) {
		Vector2 inputDir = MovementController.InputVector;
		
		if(inputDir != Vector2.Zero) {
			if(!((inputDir.X > 0 && MovementController.Velocity.X < 0) || (inputDir.X < 0 && MovementController.Velocity.X > 0))) {
				MovementController.AddDeflectFriction(delta);
			} else {
				MovementController.AddFriction(delta);
			}
		} else {
			MovementController.AddFriction(delta);
		}
	}

	private void _EnterDefaultState() {
		MovementController.CanSwitchDirections = true;
		if(!ActiveState)
			return;
		_deflectTimer.Stop();
		DeflectController.StartBlockCooldown();
		ParentPlayerStateMachine.EnterDefaultState();
	}
}
