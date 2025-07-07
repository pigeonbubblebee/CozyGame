using Godot;
using System;

public partial class SpikeZone : Area2D
{
	[Export]
	public EnemyAttackData SpikeData { get; private set;}

	[Export]
	private NodePath _SafePointLocation;
	private Vector2 _SafePointPosition;

	public override void _Ready()
	{
		this._SafePointPosition = GetNode<Node2D>(_SafePointLocation).GlobalPosition;

		this.BodyEntered += _CheckPlayerCollision;
	}

	private void _CheckPlayerCollision(Node hit) {
		if(hit is Player) {
			HitPlayer((Player)hit);
		}
	}

	public void HitPlayer(Player p) {
		p.TakeObstacleDamage(SpikeData, this, 0f);
		p.GlobalPosition = _SafePointPosition;
	}
}
