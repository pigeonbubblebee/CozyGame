using Godot;
using System;
[GlobalClass]
public partial class HealVigDamageSpirit : Equippable
{
	[Export] public int damageScale;

	public override void OnHeal(int amt, Player p)
	{
		p.CreateCustomTimer("Heal_Vigor_Scale", 7f);
	}
	
	public override PlayerBuffs ApplyStatic(PlayerBuffs p, Player player, int slot) {
		if(player.GetCustomTimerActive("Heal_Vigor_Scale")) {
			p.BonusSlashDamage += Mathf.CeilToInt(0.5f * p.Vitality);
		}	
		
		return p;
	}
}