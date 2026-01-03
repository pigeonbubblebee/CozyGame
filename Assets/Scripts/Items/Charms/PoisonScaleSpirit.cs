using Godot;
using System;

[GlobalClass]
public partial class PoisonScaleSpirit : Equippable
{
	public override PlayerBuffs ApplyStatic(PlayerBuffs p, Player player, int slot) {
		p.BonusBleedDamage += 1;
		
		return p;
	}
}
