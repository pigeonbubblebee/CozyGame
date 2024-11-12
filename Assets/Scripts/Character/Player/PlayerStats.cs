using Godot;
using System;

[GlobalClass]
public partial class PlayerStats : Resource
{
	[ExportCategory("Running")]
	[Export] public float Speed { get; private set; }
	[Export] public float Acceleration { get; private set; }
	[Export] public float DirectionSwitchSpeed { get; private set; }
	[Export] public float Friction { get; private set; }
	[ExportCategory("Jumping")]
	[Export] public float JumpVelocity { get; private set; }
	[Export] public int MaxJumps { get; private set; }
	[Export] public float JumpingGravity { get; private set; }
	[Export] public float CoyoteTime { get; private set; }
	[Export] public float JumpBuffer { get; private set; }
	[ExportCategory("Dashing")]
	[Export] public float DashSpeed { get; private set; }
	[Export] public float DashFriction { get; private set; }
	[Export] public float DashTime { get; private set; }
	[Export] public bool CanAirDash { get; private set; }
	[Export] public int MaxAirDash { get; private set; }
	[Export] public float DashCooldown { get; private set; }
	[Export] public float DashBuffer { get; private set; }
	[ExportCategory("Slashing")]
	[Export] public float SlashTime { get; private set; }
	[Export] public int SlashDamage { get; private set; }
	[Export] public int SlashRange { get; private set; }
	[Export] public float SlashCooldown { get; private set; }
	[Export] public float SlashBuffer { get; private set; }
	[ExportCategory("Health")]
	[Export] public int MaxHealth { get; private set; }
	[Export] public int MaxHeals { get; private set; }
	[Export] public float HealTime { get; private set; }
	[Export] public float HealCooldown { get; private set; }
	[ExportCategory("Shooting")]
	[Export] public int MaxMana { get; private set; }
	[Export] public int ManaUsage { get; private set; }
	[Export] public float ShootTime { get; private set; }
	[Export] public float ShootCooldown { get; private set; }
	[Export] public int ShootDamage { get; private set; }
	[Export] public float ShootBuffer { get; private set; }
	[Export] public int BulletObjectPool { get; private set; }
	[Export] public float ShootMaxRecoil { get; private set; }
	[Export] public float ShootRecoil { get; private set; }

	// Make sure you provide a parameterless constructor.
	// In C#, a parameterless constructor is different from a
	// constructor with all default values.
	// Without a parameterless constructor, Godot will have problems
	// creating and editing your resource via the inspector.
	public PlayerStats() {}

	// public PlayerStats(int health, Resource subResource, string[] strings)
	// {
	//     Health = health;
	//     SubResource = subResource;
	//     Strings = strings ?? System.Array.Empty<string>();
	// }
}
