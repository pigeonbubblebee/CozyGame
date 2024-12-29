using Godot;
using System;

[GlobalClass]
public partial class  EnemyAttackData : Resource
{
	[Export] public int Damage;
	[Export] public int InternalDamage;
	[Export] public int PostureDamage;
	[Export] public int DeflectPostureDamage;
	[Export] public float DeflectKnockbackMultiplier = 1;
}
