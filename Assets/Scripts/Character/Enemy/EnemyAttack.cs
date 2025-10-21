using Godot;
using System;

public partial class EnemyAttack : Node2D
{
	public bool CanAttack = true;
	[Export] public float AttackCooldown;
	
	[Export] public string AnimationName;
	[Export] public float AttackLength;
	
	protected GameManager EnemyGameManager;
	
	protected Enemy EnemyAI;
	protected Player TargetPlayer;
	
	public event Action FinishAttackEvent;
	[Export] private NodePath _chainedAttackPath;
	private EnemyAttack _chainedAttack;
	[Export] public bool NotChainAttack { get; private set; }
	
	[Export] protected EnemyAttackData _attackData;
	
	protected bool _initialized = false;
	
	protected bool Active;

	public bool CanChain = true;
	
	public EnemyAttack GetNextChainAttack()
	{
		if (NotChainAttack)
		{
			return this;
		}
		else
		{
			return _chainedAttack.GetNextChainAttack();
		}
	}
	
	public override void _Ready() {
		this.EnemyGameManager = GetNode<GameManager>("/root/GameManager");
		CanAttack = true;
	}
	
	public virtual bool GetCondition(Player p, Enemy e) {
		return CanAttack;
	}
	
	public virtual void Execute(Player p, Enemy e) {
		// GD.Print(this.Name);
		EnemyAI.Sprite.Play(AnimationName);
		StartAttackCooldown();
		TargetPlayer = p;
		// GD.Print("executing!");
		Active = true;
		
		
	}
	
	public void StartAttackCooldown() {
		CanAttack = false;
		GetTree().CreateTimer(AttackCooldown).Timeout += _FinishAttackCooldown;
	}

	private void _FinishAttackCooldown() {
		CanAttack = true; 
	}
	
	public EnemyAttackData GetAttackData() {
		return _attackData;
	}
	
	public virtual void Initialize(Enemy e) {
		if(_initialized)
			return;
		EnemyAI = e;
		_initialized = true;
		if(_chainedAttackPath != null) {
			_chainedAttack = GetNode<EnemyAttack>(_chainedAttackPath);
			_chainedAttack.Initialize(EnemyAI);
		}
	}
	
	protected virtual void Finish() {
		if(!Active)
			return;
		
		Active = false;
		
		if(NotChainAttack)
			FinishAttackEvent?.Invoke();
			
		// GD.Print("Finished!");
		
		if(!NotChainAttack && CanChain) {
			// GD.Print("Chaining!");
			// _chainedAttack.Execute(TargetPlayer, EnemyAI);
			EnemyAI.ExecuteAttack(_chainedAttack, TargetPlayer);
		} else {
			
		}
	}
	
	public virtual void Interrupt() {
		Active = false;
		// FinishAttackEvent?.Invoke();
	}
}
