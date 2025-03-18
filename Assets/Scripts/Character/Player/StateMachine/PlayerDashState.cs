using Godot;
using System;
using System.Threading.Tasks;

public partial class PlayerDashState : PlayerState
{
	private bool _canCancelDash = false;
	public override void Initialize(StateMachine playerStateMachine) {
		base.Initialize(playerStateMachine);

		MovementController.FinishDashEvent += _EnterDefaultState;
	}

	public override void Enter(State previousState) {
		base.Enter(previousState);

		// TODO: Reset Colliders
		
		// MovementController.DashAccelerate();

		MovementController.Dash();
		MovementController.StartDashCooldown();

		_canCancelDash = false;
		GetTree().CreateTimer(0.1f).Timeout += _setDashCancel;
	}

	private void _setDashCancel() {
		_canCancelDash = true;
	}

	public override void PlayStateAnimation() {
		// TODO: Start Idle Animation
		AnimationController.PlayAnimation(AnimationController.DashAnimationClip);
	}

	public override void Process(double delta)
	{
		if(AttackController.DesiredAttack) {
			AttackController.StartSlashBuffer();
		}
		if(MovementController.DesiredDash) {
			MovementController.StartDashBuffer();
		}
		if(MovementController.DesiredJump) {
			MovementController.StartJumpBuffer();
		}
		if(SpellController.DesiredShoot) {
			SpellController.StartShootBuffer();
		}
		if(DeflectController.DeflectActuation) {
			DeflectController.StartDeflectBuffer();
		}
	}

	public override void PhysicsProcess(double delta) {
		base.PhysicsProcess(delta);

		// MovementController.AddDashFriction();
		MovementController.DashAccelerate();

		// Vector2 inputDir = MovementController.InputVector;
		
		// if(inputDir != Vector2.Zero) {
		// 	if((inputDir.X > 0 && MovementController.Velocity.X < 0) || (inputDir.X < 0 && MovementController.Velocity.X > 0)) {
		// 		MovementController.SwitchDirection(inputDir);
		// 	} else {
				
		// 	}
		// } else {
		// 	ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.IdleState);
		// }
	}

	protected override bool CheckStates()
	{
		if(MovementController.Velocity.X == 0 && _canCancelDash) {
			_EnterDefaultState();
			return true;
		}
		return false;
	}

	private void _EnterDefaultState() {
		if(!ActiveState)
			return;
		ParentPlayerStateMachine.EnterDefaultState();
	}
}
