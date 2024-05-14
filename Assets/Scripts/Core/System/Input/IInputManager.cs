using Godot;
using System;

public interface IInputManager
{
    public abstract void GetInput();
    public abstract Vector2 GetMovementDirection();
    public abstract bool GetJumping();
    public abstract bool GetJumpActuation();
    public abstract bool GetJumpReleaseActuation();
}