using Godot;
using System;

[GlobalClass]
public partial class Equippable : Item
{
	public override string GetItemPath() {
		return "Charms/" + ID;
	}
	public virtual bool CanEquip(Player p) {
		return true;
	}

	public virtual PlayerBuffs ApplyStatic(PlayerBuffs p, Player player, int slot) {
		return p;
	}

	public virtual void OnEquip(Player p) {

	}

	public virtual void OnUnequip(Player p) {

	}
}
