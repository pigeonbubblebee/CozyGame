using Godot;
using System;
using System.Threading.Tasks;

public partial class PlayerAttackState : PlayerState
{
	private readonly int FALLING = -1;
	private readonly int NEUTRAL = 0;
	private readonly int JUMPING = 1;
	private int _subState;

	public override void Initialize(StateMachine playerStateMachine) {
		base.Initialize(playerStateMachine);

		AttackController.FinishSlashEvent += _EnterDefaultState;
	}

	public override void Enter(State previousState) {
		base.Enter(previousState);
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
	}

	 public override void PhysicsProcess(double delta) {
		base.PhysicsProcess(delta);
		
		_HandleHorizontalMovement();
		_HandleVerticalMovement();
	}

	private void _HandleHorizontalMovement() {
		Vector2 inputDir = MovementController.InputVector;
		
		if(inputDir != Vector2.Zero) {
			if((inputDir.X > 0 && MovementController.Velocity.X < 0) || (inputDir.X < 0 && MovementController.Velocity.X > 0)) {
				MovementController.SwitchDirection(inputDir);
			} else {
				MovementController.Accelerate(inputDir);
			}
		} else {
			MovementController.AddFriction();
		}
	}

	private void _HandleVerticalMovement() {
		if(_subState == JUMPING) {
			MovementController.JumpFall();
		}

		if(_subState == FALLING) {
			MovementController.Fall();
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
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.FallState);
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
		ParentPlayerStateMachine.EnterDefaultState();
	}
}
