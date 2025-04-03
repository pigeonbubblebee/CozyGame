using Godot;
using System;

public class PlayerBuffs
{
	public int Strength = 0;
	public int Dexterity = 0;
	public int Vitality = 0;
	public int Harmony = 0;
	public int Focus = 0;
	public int Prowess = 0;

	public PlayerBuffs(PlayerBuffs p) {
		Strength = p.Strength;
		Dexterity = p.Dexterity;
		Vitality = p.Vitality;
		Harmony = p.Harmony;
		Focus = p.Focus;
		Prowess = p.Prowess;
	}
	public PlayerBuffs() {
	}

	public void ResetBuffs(PlayerBuffs p) {
		Strength = p.Strength;
		Dexterity = p.Dexterity;
		Vitality = p.Vitality;
		Harmony = p.Harmony;
		Focus = p.Focus;
		Prowess = p.Prowess;
	}
}
