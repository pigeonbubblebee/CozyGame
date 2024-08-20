using Godot;
using System;

public partial class TestingItem : Interactable
{
	public override void OnInteract(Player player) {
		GD.Print("Interacted!");
	}
}
