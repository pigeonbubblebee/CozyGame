using Godot;
using System;

public partial class EnemyAttack : Node
{
	[Export] public string AnimationName;
	[Export] public float AttackLength;
	
	protected Enemy EnemyAI;
	
	public event Action FinishAttackEvent;
	[Export] public bool NotChainAttack { get; private set; }
	
	public virtual bool GetCondition(Player p, Enemy e) {
		return false;
	}
	
	public virtual void Execute(Player p, Enemy e) {
		EnemyAI.Sprite.Play(AnimationName);
	}
	
	public virtual void Initialize(Enemy e) {
		EnemyAI = e;
	}
	
	protected void Finish() {
		FinishAttackEvent?.Invoke();
	}
}
