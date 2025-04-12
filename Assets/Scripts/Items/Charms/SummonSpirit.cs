using Godot;
using System;

[GlobalClass]
public partial class SummonSpirit : Equippable
{
	[Export] public string SummonID;

    public override void OnEquip(Player p)
    {
        base.OnEquip(p);
		p.SummonController.AddSummon(SummonID);
    }

    public override void OnUnequip(Player p)
    {
        base.OnUnequip(p);
		p.SummonController.RemoveSummon(SummonID);
    }
}
