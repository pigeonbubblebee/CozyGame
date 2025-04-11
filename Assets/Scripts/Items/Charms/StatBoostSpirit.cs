using Godot;
using System;

[GlobalClass]
public partial class StatBoostSpirit : Equippable
{
	[Export] public int strength_gain;
	[Export] public int dexterity_gain;
	[Export] public int vitality_gain;
	[Export] public int focus_gain;
	[Export] public int harmony_gain;
	public override PlayerBuffs ApplyStatic(PlayerBuffs p, Player player, int slot) {
		//int mystic_threshold = 0;
		// for(int i = 0; i < _attributeNames.Length; i++) {
		// 	if(_attributeNames[i].Equals("mystic_threshold")) {
		// 		mystic_threshold = _attributeValues[i];
		// 	}
		// }

		// if(player.PostureController.CurrentPosture <= player.CurrentPlayerStats.MaxCurse - mystic_threshold) {
		// 	for(int i = 0; i < _attributeNames.Length; i++) {
		// 		if(_attributeNames[i].Equals("strength_gain")) {
		// 			p.Strength += _attributeValues[i];
		// 		}
		// 	}
		// }

		p.Strength += strength_gain;
		p.Dexterity += dexterity_gain;
		p.Vitality += vitality_gain;
		p.Focus += focus_gain;
		p.Harmony += harmony_gain;
		
		return p;
	}

    public override void OnEquip(Player p)
    {
        base.OnEquip(p);
		p.PlayerHealth.AddHealth((int)Math.Ceiling(2.5*(double)vitality_gain), true);
    }

    public override void OnUnequip(Player p)
    {
        base.OnUnequip(p);
		p.TakeCurseDamage((int)Math.Ceiling(2.5*(double)vitality_gain), 0);
    }


}
