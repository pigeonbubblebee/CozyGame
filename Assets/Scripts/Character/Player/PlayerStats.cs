using Godot;
using System;

[GlobalClass]
public partial class PlayerStats : Resource
{
	[ExportCategory("Running")]
	[Export] public float Speed { get; set; }
	[Export] public float Acceleration { get; set; }
	[Export] public float DirectionSwitchSpeed { get; set; }
	[Export] public float Friction { get; set; }
	[ExportCategory("Jumping")]
	[Export] public float JumpVelocity { get; set; }
	[Export] public int MaxJumps { get; set; }
	[Export] public float JumpingGravity { get; set; }
	[Export] public float CoyoteTime { get; set; }
	[Export] public float JumpBuffer { get; set; }
	[ExportCategory("Dashing")]
	[Export] public float DashSpeed { get; set; }
	[Export] public float DashFriction { get; set; }
	[Export] public float DashTime { get; set; }
	[Export] public bool CanAirDash { get; set; }
	[Export] public int MaxAirDash { get; set; }
	[Export] public float DashCooldown { get; set; }
	[Export] public float DashBuffer { get; set; }
	[ExportCategory("Slashing")]
	[Export] public float SlashTime { get; set; }
	[Export] public int SlashDamage { get; set; }
	[Export] public int SlashPostureDamage { get; set; }
	[Export] public int SlashRange { get; set; }
	[Export] public float SlashCooldown { get; set; }
	[Export] public float SlashBuffer { get; set; }
	[Export] public float SlashComboTime { get; set; }
	[Export] public float SlashKnockback { get; set; }
	[Export] public float SlashKnockbackAcceleration { get; set; }
	[Export] public float SlashKnockbackTime { get; set; }
	[Export] public float SlashShakeTime { get; set; }
	[Export] public float SlashShakeMagnitude { get; set; }
	[Export] public float SlashFreezeTime { get; set; }
	[Export] public float SlashFreezeDelay { get; set; }
	[Export] public float DeathBlowFreezeTime { get; set; }
	[Export] public float SlashSpeedMultiplier { get; set; }
	[ExportCategory("Counter")]
	[Export] public int SlashCounterDamage { get; set; }
	[Export] public float CounterShakeTime { get; set; }
	[Export] public float CounterShakeMagnitude { get; set; }
	[ExportCategory("Health")]
	[Export] public int MaxHealth { get; set; }
	[Export] public int MaxHeals { get; set; }
	[Export] public int InternalDamageHealRate { get; set; }
	[Export] public float InternalDamageHealCooldown { get; set; }
	[Export] public float InternalDamageHealDelay { get; set; }
	[Export] public float HealTime { get; set; }
	[Export] public int HealPower { get; set; }
	[Export] public float HealCooldown { get; set; }
	[ExportCategory("Shooting")]
	[Export] public int MaxMana { get; set; }
	[Export] public int ManaUsage { get; set; }
	[Export] public float ShootTime { get; set; }
	[Export] public float ShootCooldown { get; set; }
	[Export] public int ShootDamage { get; set; }
	[Export] public int ShootPostureDamage { get; set; }
	[Export] public float ShootBuffer { get; set; }
	[Export] public int BulletObjectPool { get; set; }
	[Export] public float ShootMaxRecoil { get; set; }
	[Export] public float ShootRecoil { get; set; }
	[ExportCategory("Deflecting")]
	[Export] public float DeflectWindow { get; set; }
	[Export] public float DeflectFriction { get; set; }
	[Export] public float BlockMinimumTime { get; set; }
	[Export] public float BlockCooldown { get; set; }
	[Export] public float BlockTimeout { get; set; }
	[Export] public float CounterWindow { get; set; }
	[Export] public float BlockKnockback { get; set; }
	[Export] public float BlockKnockbackAcceleration { get; set; }
	[Export] public float BlockKnockbackTime { get; set; }
	[Export] public float DeflectKnockback { get; set; }
	[Export] public float DeflectKnockbackAcceleration { get; set; }
	[Export] public float DeflectKnockbackTime { get; set; }
	[Export] public float BlockShakeTime { get; set; }
	[Export] public float BlockShakeMagnitude { get; set; }
	[Export] public float DeflectShakeTime { get; set; }
	[Export] public float DeflectShakeMagnitude { get; set; }
	[ExportCategory("Curse")]
	[Export] public int MaxCurse { get; set; }
	[Export] public int CurseBuildRate { get; set; }
	[Export] public int CurseUsePerSpell { get; set; }
	// [Export] public int PostureRegeneration { get; set; }
	// [Export] public float PostureRegenerationDisengagementTime { get; set; }
	// [Export] public float PostureRegenerationCooldown { get; set; }
	// [Export] public float StaggerRecoveryTime { get; set; }
	[ExportCategory("Powerups")]
	[Export] public bool CanCounterCleave { get; set; }
	[Export] public int EquipSlots { get; set; }

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
