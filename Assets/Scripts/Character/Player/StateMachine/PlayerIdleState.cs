using Godot;
using System;

public partial class PlayerIdleState : PlayerState
{
	private bool _deflectTransition = false;
	private bool _hitTransition = false;
	
	public override void Enter(State previousState) {
		_deflectTransition = previousState is PlayerBlockState;
		_hitTransition = false;
		if(previousState is PlayerBlockState) {
			if(((PlayerBlockState)previousState).HitTransition) {
				_hitTransition = true;
			}
		}
		
		base.Enter(previousState);

		// TODO: Reset Colliders
	}

	public override void PlayStateAnimation() {
		if(_deflectTransition){
			AnimationController.PlayAnimation(AnimationController.BlockTransAnimationClip);
			GetTree().CreateTimer(0.3528f).Timeout += _PlayIdleAnimation;
		} else {
			AnimationController.PlayAnimation(AnimationController.IdleAnimationClip);
		}
		// TODO: Start Idle Animation
	}
	
	private void _PlayIdleAnimation() {
		if(ActiveState)
			AnimationController.PlayAnimation(AnimationController.IdleAnimationClip);
	}

	public override void PhysicsProcess(double delta) {
		base.PhysicsProcess(delta);
		
		MovementController.AddFriction(delta);
	}

	protected override bool CheckStates()
	{
		if(PostureController.CurrentPosture <= 0) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.StaggerState);
			return true;
		}
		if(DeflectController.DeflectActuation || (!DeflectController.GetDeflectBufferStop() && DeflectController.CanBlock)) {
			DeflectController.StartBlock();
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.BlockState);
			return true;
		}
		if(HealController.DesiredHeal && HealController.HealCooldownOff) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.HealState);
			return true;
		}
		// GD.Print(MovementController.DesiredDashRaw + " " + DeflectController.CanCleaveCounter);
		if(MovementController.DesiredDashRaw && DeflectController.CanCleaveCounter) {
			GD.Print("Cleave Coutner!");
			DeflectController.CounterCleave();
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.GrabState);
			return true;
		}
		if(MovementController.DesiredDash || (!MovementController.GetDashBufferStop() && MovementController.CanDash)) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.DashState);
			return true;
		}
		if(SpellController.DesiredShoot || (!SpellController.GetShootBufferStop()  && SpellController.CanShoot)) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.SpellState);
			return true;
		}
		if(AttackController.DesiredAttack || (!AttackController.GetSlashBufferStop() && AttackController.CanSlash)) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.AttackState);
			return true;
		}
		if(!MovementController.Grounded) {
			MovementController.StartCoyoteTimer();
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.FallState);
			return true;
		}
		if(MovementController.DesiredJump || !MovementController.GetJumpBufferStop()) {
			ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.JumpState);
			return true;
		}
		if(MovementController.InputVector != Vector2.Zero) {
		 	ParentPlayerStateMachine.ChangeState(ParentPlayerStateMachine.RunState);
			return true;
		}

		return false;
	}
}
