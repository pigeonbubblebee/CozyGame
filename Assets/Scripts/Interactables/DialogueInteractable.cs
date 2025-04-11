using Godot;
using System;

public partial class DialogueInteractable : Interactable
{
	[Export] private DialogueData _dialogueData;

	public override bool GetCondition(Player player) {
		return !GetNode<UIManager>("/root/UIManager").DialogueOpen && !GetNode<UIManager>("/root/UIManager").InventoryOpen
			&& !GetNode<UIManager>("/root/UIManager").MerchantOpen;
	}

	public override void OnInteract(Player player) {
		GetNode<UIManager>("/root/UIManager").LoadDialogue(_dialogueData);
	}
}
