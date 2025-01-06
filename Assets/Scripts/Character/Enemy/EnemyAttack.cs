using Godot;
using System;

public partial class EnemyAttack : Node2D
{
	[Export] public string AnimationName;
	[Export] public float AttackLength;
	
	protected GameManager EnemyGameManager;
	
	protected Enemy EnemyAI;
	protected Player TargetPlayer;
	
	public event Action FinishAttackEvent;
	[Export] private NodePath _chainedAttackPath;
	private EnemyAttack _chainedAttack;
	[Export] public bool NotChainAttack { get; private set; }
	
	protected bool Active;
	
	public EnemyAttack GetNextChainAttack() {
		if(NotChainAttack) {
			return this;
		} else {
			return _chainedAttack.GetNextChainAttack();
		}
	}
	
	public override void _Ready() {
		this.EnemyGameManager = GetNode<GameManager>("/root/GameManager");
	}
	
	public virtual bool GetCondition(Player p, Enemy e) {
		return false;
	}
	
	public virtual void Execute(Player p, Enemy e) {
		EnemyAI.Sprite.Play(AnimationName);
		TargetPlayer = p;
		GD.Print("executing!");
		Active = true;
	}
	
	public virtual void Initialize(Enemy e) {
		EnemyAI = e;
		
		if(_chainedAttackPath != null) {
			_chainedAttack = GetNode<EnemyAttack>(_chainedAttackPath);
			_chainedAttack.Initialize(EnemyAI);
		}
	}
	
	protected virtual void Finish() {
		Active = false;
		FinishAttackEvent?.Invoke();
		
		if(!NotChainAttack) {
			GD.Print("Chaining!");
			_chainedAttack.Execute(TargetPlayer, EnemyAI);
		} else {
			
		}
	}
	
	public virtual void Interrupt() {
		Active = false;
		// FinishAttackEvent?.Invoke();
	}
}
