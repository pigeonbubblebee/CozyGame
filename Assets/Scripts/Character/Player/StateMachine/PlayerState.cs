using Godot;
using System;

public abstract partial class PlayerState : State
{
	protected PlayerStateMachine ParentPlayerStateMachine => (ParentStateMachine as PlayerStateMachine);
	protected PlayerMovementController MovementController => ParentPlayerStateMachine.MovementController;
	protected PlayerAttackController AttackController => ParentPlayerStateMachine.AttackController;
	
	// protected Player _player => ParentPlayerStateMachine.Player;

	// protected PlayerMovementController _playerMovementController => _player.MovementController;
	// protected PlayerJumpController _playerJumpController => _player.JumpController;
	// protected PlayerStatsController _playerStatsController => _player.StatsController;
	// protected PlayerRigidBodyController _playerRigidBodyController => _player.RigidBodyController;

	public bool CanFlip { get; protected set; }
	
	public override void Initialize(StateMachine playerStateMachine) {
		base.Initialize(playerStateMachine);
	}

	public override void Enter(State previousState) {
		CanFlip = true;
		PlayStateAnimation();
	}

	public virtual void PlayStateAnimation() {

	}

	protected virtual bool CheckStates() {
		return false;
	}

    public override void Process(double delta)
    {
        base.Process(delta);

		if(CheckStates()) {
			return;
		}
    }

	public override void PhysicsProcess(double delta)
    {
        base.Process(delta);

		if(CheckStates()) {
			return;
		}
    }
}
