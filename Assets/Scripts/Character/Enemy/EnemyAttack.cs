using Godot;
using System;

public partial class EnemyAttack : Node
{
	[Export] public string AnimationName;
	[Export] public float AttackLength;
	
	protected GameManager EnemyGameManager;
	
	protected Enemy EnemyAI;
	
	public event Action FinishAttackEvent;
	[Export] public bool NotChainAttack { get; private set; }
	
	protected bool Active;
	
	public override void _Ready() {
		this.EnemyGameManager = GetNode<GameManager>("/root/GameManager");
	}
	
	public virtual bool GetCondition(Player p, Enemy e) {
		return false;
	}
	
	public virtual void Execute(Player p, Enemy e) {
		EnemyAI.Sprite.Play(AnimationName);
		Active = true;
	}
	
	public virtual void Initialize(Enemy e) {
		EnemyAI = e;
	}
	
	protected void Finish() {
		Active = false;
		FinishAttackEvent?.Invoke();
	}
	
	public virtual void Interrupt() {
		Active = false;
		// FinishAttackEvent?.Invoke();
	}
}
