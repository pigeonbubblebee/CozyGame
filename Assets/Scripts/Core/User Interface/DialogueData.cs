using Godot;
using System;

[GlobalClass]
public partial class DialogueData : Resource
{
	[Export] public Texture2D Portrait;
	[Export] public string[] DialogueKey;
	[Export] public string NameKey;
	[Export] public DialogueData NextDialogue;
}
