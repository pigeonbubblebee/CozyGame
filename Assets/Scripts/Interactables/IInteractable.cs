using Godot;
using System;

public interface IInteractable
{
	void OnInteract(Player player);
	void OnInteractHold(Player player);
}
