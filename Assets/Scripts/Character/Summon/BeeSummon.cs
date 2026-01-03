using Godot;
using System;

public partial class BeeSummon : DashSummon
{
	[Export]
	private int _PoisonBuildup;
	protected override void _OnHitEffect(Enemy e)
	{
		e.AddBleed(_PoisonBuildup);
	}
}
