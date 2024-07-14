using Godot;
using System;

public interface IInputManager
{
    public abstract void GetInput();
    public abstract Vector2 GetMovementDirection();

    public abstract bool GetJumping();
    public abstract bool GetJumpActuation();
    public abstract bool GetJumpReleaseActuation();

    public abstract bool GetAttacking();
    public abstract bool GetAttackActuation();
    public abstract bool GetAttackReleaseActuation();

    public abstract bool GetDashing();
    public abstract bool GetDashActuation();
    public abstract bool GetDashReleaseActuation();
}