using Godot;
using System;

public partial class UnicornSummon : DashSummon
{
	[Export]
	private int _HealAmount;
	
	protected override void _OnHitEffect(Enemy e)
	{
		_player.PlayerHealth.AddHealth(_HealAmount, false);
	}
}
