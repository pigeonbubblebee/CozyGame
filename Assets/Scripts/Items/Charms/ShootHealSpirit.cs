using Godot;
using System;

[GlobalClass]
public partial class ShootHealSpirit : Equippable
{
	[Export] public int healAmt;
	public override void OnShoot(Player p) {
		p.PlayerHealth.AddHealth(healAmt, false);
	}
}
