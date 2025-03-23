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
		bool[] canExecute = new bool[_enemyAttacks.Length];

		int executables = 0;

		for(int i = 0; i < canExecute.Length; i++) {
			canExecute[i] = _enemyAttacks[i].GetCondition(p, e);
			executables ++;
		}

		Random random = new Random();
		int amount = random.Next(0, executables);
		int attackIndex;

		int j = 0;

		foreach(bool b in canExecute) {
			// GD.Print("Amount Left: "+ amount + " " + b);
			if(amount == 0) {
				attackIndex = j;

				_attack = _enemyAttacks[attackIndex];
		
				AnimationName = _attack.AnimationName;
				AttackLength = _attack.AttackLength;
				base.Execute(p, e);
				
				_attack.Execute(p, e);
				_attack.GetNextChainAttack().FinishAttackEvent += Finish;

				return;
			}

			if(b) {
				amount --;
			}	
			j++;
		}

		if(amount == 0) {
			attackIndex = j;

			if(attackIndex > _enemyAttacks.Length - 1)
				attackIndex = _enemyAttacks.Length - 1;
			if(attackIndex < 0)
				attackIndex = 0;

			_attack = _enemyAttacks[attackIndex];
	
			AnimationName = _attack.AnimationName;
			AttackLength = _attack.AttackLength;
			base.Execute(p, e);
			
			_attack.Execute(p, e);
			_attack.GetNextChainAttack().FinishAttackEvent += Finish;
		} else {
			Active = true;
			Finish();
		}
	}

	
	
	protected override void Finish() {
		// GD.Print("Finished");
		if(_attack!=null) {
			_attack.FinishAttackEvent -= Finish;
		}
		base.Finish();
	}
	
	public override void Interrupt() {
		_attack.FinishAttackEvent -= Finish;
		
		_attack.Interrupt();
	}
	
	public override bool GetCondition(Player p, Enemy e) {
		// bool res = true;
		// foreach(EnemyAttack a in _enemyAttacks) {
		// 	if(!a.GetCondition(p, e))
		// 		res = false;
		// }

		bool res = false;
		foreach(EnemyAttack a in _enemyAttacks) {
			if(a.GetCondition(p, e))
				res = true;
		}
		return res && CanAttack;
	}
}
