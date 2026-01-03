using Godot;
using System;

[GlobalClass]
public partial class Equippable : Item
{
	public override string GetItemPath() {
		return "Charms/" + ID;
	}
	public virtual bool CanEquip(Player p) {
		return true;
	}

	public virtual PlayerBuffs ApplyStatic(PlayerBuffs p, Player player, int slot) {
		return p;
	}

	public virtual void OnShoot(Player p) {

	}

	public virtual void OnHeal(int amt, Player p) {

	}

	public virtual void OnShootHit(IHittable hittable, int dmg, int dir, int posture)
	{
		
	}
	public virtual void OnBlock(bool perfect, int postureDamage, Enemy e)
	{
		
	}

	public virtual void OnSwordHit(IHittable hittable, int dmg, int dir, int posture, Player p) {}

	public virtual void OnSummonHit(Enemy e, int damage, Summon s, Player p)
	{
		
	}

	public virtual void OnEquip(Player p)
	{
		p.SpellController.ShootEvent += OnShoot;
		p.PlayerHealEvent += OnHeal;
		p.SpellController.HitEvent += OnShootHit;
		p.DeflectController.BlockEvent += OnBlock;
		p.AttackController.HitEvent += OnSwordHit;
		p.SummonController.SummonHitEvent += OnSummonHit;
	}

	public virtual void OnUnequip(Player p) {
		p.SpellController.ShootEvent -= OnShoot;
		p.PlayerHealEvent -= OnHeal;
		p.SpellController.HitEvent -= OnShootHit;
		p.DeflectController.BlockEvent -= OnBlock;
		p.AttackController.HitEvent -= OnSwordHit;
		p.SummonController.SummonHitEvent -= OnSummonHit;
	}
}
