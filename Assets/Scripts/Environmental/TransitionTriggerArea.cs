using Godot;
using System;

public partial class TransitionTriggerArea : Area2D
{
	[Export] public string SpawnArea;
	[Export] public string Area;
	[Export] public string Level;
	public override void _Ready()
	{
		this.BodyEntered += TransitionScene;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void TransitionScene(Node hit) {
		if(hit is Player) {
			GetNode<SaveLoader>("/root/SaveLoader").Save();
			GetNode<MainHandler>("/root/MainHandler").LoadLevel(Area, Level, SpawnArea);
		}
	}
}
