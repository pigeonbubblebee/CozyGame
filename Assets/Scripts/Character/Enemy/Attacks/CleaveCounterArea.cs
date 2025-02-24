using Godot;
using System;

public partial class CleaveCounterArea : Area2D
{
	[Export] private NodePath _cleaveAttackPath;
	public CleaveAttack CleaveAttack;
	
	public override void _Ready() {
		CleaveAttack = GetNode<CleaveAttack>(_cleaveAttackPath);
	}
}
