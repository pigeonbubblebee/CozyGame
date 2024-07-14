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

	private readonly string MOVE_LEFT = "move_left";
	private readonly string MOVE_RIGHT = "move_right";
	private readonly string JUMP = "jump";
	private readonly string ATTACK = "attack";
	private readonly string DASH = "dash";

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
}
