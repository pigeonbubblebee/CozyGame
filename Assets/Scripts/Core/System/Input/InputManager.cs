using Godot;
using System;

public partial class InputManager : Node, IInputManager
{
	private Vector2 _movementDirection;
	private bool _jumping;
	private bool _jumpActuation;
	private bool _jumpReleaseActuation;

	private bool _attacking;
	private bool _attackActuation;
	private bool _attackReleaseActuation;

	private bool _dashing;
	private bool _dashActuation;
	private bool _dashReleaseActuation;

	private bool _inventoryActuation;
	private bool _escapeActuation;

	private bool _interacting;
	private bool _interactActuation;
	private bool _interactReleaseActuation;
	
	private bool _healing;
	private bool _healActuation;
	private bool _healReleaseActuation;
	
	private bool _shooting;
	private bool _shootActuation;
	private bool _shootReleaseActuation;

	// Vars for control bind names on input maps
	private readonly string MOVE_LEFT = "move_left";
	private readonly string MOVE_RIGHT = "move_right";
	private readonly string JUMP = "jump";
	private readonly string ATTACK = "attack";
	private readonly string DASH = "dash";
	private readonly string INVENTORY = "menu";
	private readonly string ESCAPE = "escape";
	private readonly string INTERACT = "interact";
	private readonly string HEAL = "heal";
	private readonly string SHOOT = "shoot";

	public override void _Process(double delta) {
		GetInput();
	}

	public void GetInput()
	{
		Vector2 inputDirection = Vector2.Zero;
		inputDirection.X = Input.GetAxis(MOVE_LEFT, MOVE_RIGHT);

		_movementDirection = inputDirection;

		_jumping = Input.IsActionPressed(JUMP);
		_jumpActuation = Input.IsActionJustPressed(JUMP);
		_jumpReleaseActuation = Input.IsActionJustReleased(JUMP);

		_attacking = Input.IsActionPressed(ATTACK);
		_attackActuation = Input.IsActionJustPressed(ATTACK);
		_attackReleaseActuation = Input.IsActionJustReleased(ATTACK);

		_dashing = Input.IsActionPressed(DASH);
		_dashActuation = Input.IsActionJustPressed(DASH);
		_dashReleaseActuation = Input.IsActionJustReleased(DASH);

		_inventoryActuation = Input.IsActionJustPressed(INVENTORY);
		_escapeActuation = Input.IsActionJustPressed(ESCAPE);

		_interacting = Input.IsActionPressed(INTERACT);
		_interactActuation = Input.IsActionJustPressed(INTERACT);
		_interactReleaseActuation = Input.IsActionJustReleased(INTERACT);
		
		_healing = Input.IsActionPressed(HEAL);
		_healActuation = Input.IsActionJustPressed(HEAL);
		_healReleaseActuation = Input.IsActionJustReleased(HEAL);
		
		_shooting = Input.IsActionPressed(SHOOT);
		_shootActuation = Input.IsActionJustPressed(SHOOT);
		_shootReleaseActuation = Input.IsActionJustReleased(SHOOT);
	}

	public Vector2 GetMovementDirection() {
		return _movementDirection;
	}
	public bool GetJumping() {
		return _jumping;
	}
	public bool GetJumpActuation() {
		return _jumpActuation;
	}
	public bool GetJumpReleaseActuation() {
		return _jumpReleaseActuation;
	}

	public bool GetAttacking() {
		return _attacking;
	}
	public bool GetAttackActuation() {
		return _attackActuation;
	}
	public bool GetAttackReleaseActuation() {
		return _attackReleaseActuation;
	}

	public bool GetDashing() {
		return _dashing;
	}
	public bool GetDashActuation() {
		return _dashActuation;
	}
	public bool GetDashReleaseActuation() {
		return _dashReleaseActuation;
	}

	public bool GetInventoryActuation() {
		return _inventoryActuation;
	}
	public bool GetEscapeActuation() {
		return _escapeActuation;
	}

	public bool GetInteracting() {
		return _interacting;
	}
	public bool GetInteractActuation() {
		return _interactActuation;
	}
	public bool GetInteractReleaseActuation() {
		return _interactReleaseActuation;
	}
	
	public bool GetHeal() {
		return _healing;
	}
	public bool GetHealActuation() {
		return _healActuation;
	}
	public bool GetHealReleaseActuation() {
		return _healReleaseActuation;
	}
	
	public bool GetShoot() {
		return _shooting;
	}
	public bool GetShootActuation() {
		return _shootActuation;
	}
	public bool GetShootReleaseActuation() {
		return _shootReleaseActuation;
	}
}
