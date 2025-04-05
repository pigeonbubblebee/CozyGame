using Godot;
using System;

public partial class TransitionTriggerArea : Area2D
{
	[Export] public string SpawnArea;
	[Export] public string Area;
	[Export] public string Level;
	[Export] public Vector2 ChunkPos;
	[Export] public Vector2 Direction;
	public override void _Ready()
	{
		this.BodyEntered += TransitionScene;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void TransitionScene(Node2D hit) {
		if(hit is Player) {
			

			// GetNode<MainHandler>("/root/MainHandler").LoadLevelChunk(Area, Level, ChunkPos);
			//GD.Print("transing scene");
			//GD.Print(Direction.X + " " + hit.GlobalPosition.X + " " + GlobalPosition.X);
			if((Direction.X > 0 && hit.GlobalPosition.X < this.GlobalPosition.X) || (Direction.X < 0 && hit.GlobalPosition.X > this.GlobalPosition.X)) {
				//GD.Print("load");
				GetNode<SaveLoader>("/root/SaveLoader").Save();
				GetNode<MainHandler>("/root/MainHandler").LoadLevelChunk(Area, Level);
			}
			// else if((Direction.X > 1 && hit.GlobalPosition.X > this.Position.X) || (Direction.X < 1 && hit.GlobalPosition.X < this.Position.X)) {
			// 	GetNode<MainHandler>("/root/MainHandler").QueueFreePrevScene();
			// }
			
			// GetNode<MainHandler>("/root/MainHandler").LoadLevel(Area, Level, SpawnArea);

			// GetNode<MainHandler>("/root/MainHandler").LoadLevelChunk(Area, Level);
		}
	}
}
