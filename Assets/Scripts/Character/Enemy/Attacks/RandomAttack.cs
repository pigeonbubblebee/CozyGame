using Godot;
using System;

public partial class RandomAttack : EnemyAttack
{
	[Export]
	public NodePath[] AttackPaths { get; set; }
	private EnemyAttack[] _enemyAttacks;
	
	private EnemyAttack _attack;
	
	public override void Initialize(Enemy e) {
		base.Initialize(e);
		
		_enemyAttacks = new EnemyAttack[AttackPaths.Length];
		
		for(int i = 0; i < AttackPaths.Length; i++) {
			_enemyAttacks[i] = GetNode<EnemyAttack>(AttackPaths[i]);
			_enemyAttacks[i].Initialize(e);
		}
	}
	
	public override void Execute(Player p, Enemy e) {
		Random random = new Random();
		int attackIndex = random.Next(0, _enemyAttacks.Length);
		_attack = _enemyAttacks[attackIndex];
		
		AnimationName = _attack.AnimationName;
		AttackLength = _attack.AttackLength;
		base.Execute(p, e);
		
		_attack.Execute(p, e);
		_attack.GetNextChainAttack().FinishAttackEvent += Finish;
	}
	
	protected override void Finish() {
		_attack.FinishAttackEvent -= Finish;
		base.Finish();
	}
	
	public override void Interrupt() {
		_attack.FinishAttackEvent -= Finish;
		
		_attack.Interrupt();
	}
	
	public override bool GetCondition(Player p, Enemy e) {
		bool res = true;
		foreach(EnemyAttack a in _enemyAttacks) {
			if(!a.GetCondition(p, e))
				res = false;
		}
		return res && CanAttack;
	}
}
