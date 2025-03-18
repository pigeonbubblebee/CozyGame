using Godot;
using System;
using System.Threading.Tasks;

public partial class PlayerAttackState : PlayerState
{
	private readonly int FALLING = -1;
	private readonly int NEUTRAL = 0;
	private readonly int JUMPING = 1;
	private int _subState;
	
	[Export] private NodePath _attackMoveTimerPath;
	private Timer _attackMoveTimer;

	public override void Initialize(StateMachine playerStateMachine) {
		base.Initialize(playerStateMachine);
		
		_attackMoveTimer = GetNode<Timer>(_attackMoveTimerPath);

		AttackController.FinishSlashEvent += _EnterDefaultState;
	}
	
	public override void PlayStateAnimation() {
		GD.Print("Animation Play!");
		if(AttackController.DesiredDown) {
			AnimationController.PlayAnimation(AnimationController.SlashDownAnimationClip, Stats.SlashTime);
			return;
		}
		if(AttackController.CanDeathBlow) {
			AnimationController.PlayAnimation(AnimationController.DeathBlowAnimationClip);
			return;
		}
		
		GD.Print(AttackController.CurrentSlashComboAttack);
		
		// AnimationController.PlayAnimation(DeflectController.Counter ? AnimationController.CounterAnimationClip : AnimationController.SlashAnimationClip, Stats.SlashTime);
		// TODO: Start Idle Animation
		
		switch(AttackController.CurrentSlashComboAttack) {
			case 0:
				AnimationController.PlayAnimation(AnimationController.SlashAnimationClip, Stats.SlashTime);
				break;
			case 1:
				AnimationController.PlayAnimation(AnimationController.Slash2AnimationClip, Stats.SlashTime);
				break;
		}
	}

	public override void Enter(State previousState) {
		_attackMoveTimer.Start(0.15f);
		
		
		// TODO: Reset Colliders
		AttackController.Slash();
		AttackController.StartSlashCooldown();

		if(previousState is PlayerJumpState) {
			_subState = JUMPING;
		} else if (previousState is PlayerFallState) {
			_subState = FALLING;
		} else {
			_subState = NEUTRAL;
		}
		
		base.Enter(previousState);
		
		CanFlip = false;
	}

	public override void Process(double delta)
	{
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
		if(DeflectController.DeflectActuation) { // Cancellable Attack
			DeflectController.StartBlock();
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.BlockState);
			AttackController.CancelSlash();
			// return true;
			// DeflectController.StartDeflectBuffer();
		}
	}

	 public override void PhysicsProcess(double delta) {
		base.PhysicsProcess(delta);
		
		
		_HandleHorizontalMovement(delta);	
		_HandleVerticalMovement(delta);
	}

	private void _HandleHorizontalMovement(double delta) {
		if(_subState == JUMPING || _subState == FALLING) 
			return;
			
		if(!_attackMoveTimer.IsStopped())
			MovementController.Accelerate(new Vector2(MovementController.Direction * Stats.SlashSpeedMultiplier, MovementController.Velocity.Y), delta);
		else
			MovementController.AddDeflectFriction(delta);
		/*
		if(AttackController.CanDeathBlow) {
			MovementController.AddFriction(delta);
			return;
		}
		
		Vector2 inputDir = MovementController.InputVector;
		
		if(inputDir != Vector2.Zero) {
			if((inputDir.X > 0 && MovementController.Velocity.X < 0) || (inputDir.X < 0 && MovementController.Velocity.X > 0)) {
				MovementController.SwitchDirection(inputDir * Stats.SlashSpeedMultiplier, delta);
			} else {
				MovementController.Accelerate(inputDir * Stats.SlashSpeedMultiplier, delta);
			}
		} else {
			MovementController.AddFriction(delta);
		}
		*/
	}

	private void _HandleVerticalMovement(double delta) {
		if(_subState == JUMPING) {
			MovementController.JumpFall(delta);
		}

		if(_subState == FALLING) {
			MovementController.Fall(delta);
		}
	}

	private void _HandleVerticalChanges() {
		if(_subState == NEUTRAL) {
			_HandleNeutralChanges();
		} else if(_subState == JUMPING) {
			_HandleJumpingChanges();
		} else if(_subState == FALLING) {
			_HandleFallingChanges();
		}
	}

	private void _HandleNeutralChanges() {
		if(!MovementController.Grounded) {
			MovementController.StartCoyoteTimer();
			_subState = FALLING;
			return;
		} else if(MovementController.DesiredJump) {
			MovementController.Jump();
			_subState = JUMPING;
			return;
		}
		else if(!MovementController.GetJumpBufferStop()) {
			MovementController.Jump();
			_subState = JUMPING;
			return;
		}
	}

	private void _HandleJumpingChanges() {
		if(MovementController.DesiredFall) {
			MovementController.JumpFallTransition();
			_subState = FALLING;
			return;
		}

		if(MovementController.Velocity.Y > 0) {
			// ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.FallState);
			_subState = FALLING;
			return;
		}
	}

	private void _HandleFallingChanges() {
		if(MovementController.Grounded) {
			_subState = NEUTRAL;
			return;
		} else if(MovementController.DesiredJump && !MovementController.GetCoyoteStop()) {
			MovementController.Jump();
			_subState = JUMPING;
			return;
		}
	}

	private void _EnterDefaultState() {
		if(!ActiveState) {
			return;
		}
		DeflectController.Counter = false;
		ParentPlayerStateMachine.EnterDefaultState();
	}
}
