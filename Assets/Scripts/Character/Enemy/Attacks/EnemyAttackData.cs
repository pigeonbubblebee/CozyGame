using Godot;
using System;

[GlobalClass]
public partial class EnemyAttackData : Resource
{
	[Export] public int Damage;
	[Export] public int InternalDamage;
	[Export] public int PostureDamage;
	[Export] public int DeflectPostureDamage;
	[Export] public bool Unstoppable = false;
	[Export] public AttackType Type;
	[Export] public float DeflectKnockbackMultiplier = 1;
	
	public enum AttackType {
		Normal,
		Cleave,
		Thrust,
		Sweep,
		Grab
	}
}
