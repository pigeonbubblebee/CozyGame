using Godot;
using System;

public partial class DialogueTriggerArea : Area2D
{
	[Export] private DialogueData _dialogueData;

    public override void _Ready()
    {
        base._Ready();

		BodyEntered += _loadDialogue;
    }

	private void _loadDialogue(Node2D hit) {
		if(hit is Player) {
			GetNode<UIManager>("/root/UIManager").LoadDialogue(_dialogueData); QueueFree();
		}
	}

}
