using Godot;
using System;

public partial class HealthSystem : Node
{
	[Export] public int MaxHealthPoints { get; set; }
	[Export] public bool HealthPointsAreTrueHitAmount { get; set; }
	[Export] public int CurrentHealthPoints { get; private set; }

	[Export] public bool Invincible { get; set; }

	public event Action DeathEvent;
	public event Action<int> DamageEvent;
	public event Action<int> HealEvent;

	// TODO: implement health changes

	public void ResetHealth() {
		int temp = CurrentHealthPoints;
		CurrentHealthPoints = MaxHealthPoints;
		HealEvent?.Invoke(MaxHealthPoints - temp);
	}

	public void SetHealth(int health) {
		DamageEvent?.Invoke(CurrentHealthPoints - health);
		CurrentHealthPoints = health;
	}

	public void AddHealth(int health, bool overflow) {
		CurrentHealthPoints += health;
		HealEvent?.Invoke(health);

		if(!overflow) {
			CurrentHealthPoints = Mathf.Min(CurrentHealthPoints, MaxHealthPoints);
		}
	}

	public void TakeDamage(int damage) {
		if(Invincible) {
			return;
		}

		CurrentHealthPoints -= HealthPointsAreTrueHitAmount ? 1 : damage;
		DamageEvent?.Invoke(damage);
		if(CurrentHealthPoints <= 0) {
			DeathEvent?.Invoke();
		}
	}
}
