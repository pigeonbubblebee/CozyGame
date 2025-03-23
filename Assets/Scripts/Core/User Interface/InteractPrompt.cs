using Godot;
using System;

public partial class InteractPrompt : Node2D
{
	[Export] public NodePath _textPath;
	[Export] public string PromptText;
	public override void _Ready()
	{
		GetNode<RichTextLabel>(_textPath).Text = "[center]" + PromptText;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
