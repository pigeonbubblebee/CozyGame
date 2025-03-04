using Godot;
using System;

public partial class SceneManager : Node
{
	[Export] public Player ScenePlayer;
	
	public override void _Ready() {
		GD.Randomize();
	}
}
