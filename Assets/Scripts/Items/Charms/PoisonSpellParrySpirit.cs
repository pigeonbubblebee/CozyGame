using Godot;
using System;

[GlobalClass]
public partial class PoisonSpellParrySpirit : Equippable
{
	[Export]
	private int _Buildup;
	public override void OnShootHit(IHittable hittable, int dmg, int dir, int posture)
	{
		if(hittable is Enemy)
		{
			((Enemy)hittable).AddBleed(_Buildup);
		}
	}
	public override void OnBlock(bool perfect, int postureDamage, Enemy e)
	{
		if(perfect) {
			e.AddBleed(_Buildup);
		}
	}
}
