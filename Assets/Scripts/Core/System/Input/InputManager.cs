using Godot;
using System;

public partial class InputManager : Node, IInputManager
{
	private Vector2 _movementDirection;
	private bool _jumping;
	private bool _jumpActuation;
	private bool _jumpReleaseActuation;

	private readonly string MOVE_LEFT = "move_left";
	private readonly string MOVE_RIGHT = "move_right";
	private readonly string JUMP = "jump";

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
}
