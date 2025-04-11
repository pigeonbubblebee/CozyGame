using Godot;
using System;

public interface IInteractable
{
	void OnInteract(Player player);
	void OnInteractHold(Player player);
	void OnEnter(Player player);
	void OnExit(Player player);
	bool GetCondition(Player player);
}
