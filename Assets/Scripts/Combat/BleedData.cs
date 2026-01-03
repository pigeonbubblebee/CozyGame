using Godot;
using System;

public class BleedData {
	public float timeLeft;
	public bool doneTick;

	public int totalTicksDone;

	public int bonusDamage;

	public BleedData(int bonus) {
		timeLeft = 0.5f;
		doneTick = false;
		totalTicksDone = 0;

		bonusDamage = bonus;
	}
}
