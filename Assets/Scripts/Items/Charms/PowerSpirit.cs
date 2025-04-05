using Godot;
using System;

[GlobalClass]
public partial class PowerSpirit : Equippable
{
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

		if(player.CurseController.CurrentCurse >= 50) {
			p.Strength += 4;
		}	
		
		return p;
	}
}
