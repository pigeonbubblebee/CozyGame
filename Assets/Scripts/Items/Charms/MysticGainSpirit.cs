using Godot;
using System;

[GlobalClass]
public partial class MysticGainSpirit : Equippable
{
	[Export]
	private int _buildRate;
	public override void OnSwordHit(IHittable hittable, int dmg, int dir, int posture, Player p)
	{
		p.CurseController.AddCurse(_buildRate);
	}
	public override void OnSummonHit(Enemy e, int damage, Summon s, Player p)
	{
		p.CurseController.AddCurse(_buildRate);
	}
}
