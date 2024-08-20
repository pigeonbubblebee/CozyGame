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
    [Export] public float SlashTime;
	[Export] public int SlashDamage;
	[Export] public int SlashRange;
	[Export] public float SlashCooldown;
	[Export] public float SlashBuffer;

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
