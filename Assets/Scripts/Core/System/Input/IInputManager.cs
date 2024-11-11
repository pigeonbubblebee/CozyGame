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

	public abstract bool GetInventoryActuation();
	public abstract bool GetEscapeActuation();
	
	public abstract bool GetInteracting();
	public abstract bool GetInteractActuation();
	public abstract bool GetInteractReleaseActuation();
	
	public abstract bool GetHeal();
	public abstract bool GetHealActuation();
	public abstract bool GetHealReleaseActuation();
	
	public abstract bool GetShoot();
	public abstract bool GetShootActuation();
	public abstract bool GetShootReleaseActuation();
}
