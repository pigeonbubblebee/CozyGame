using Godot;
using System;

public partial class Tilemap : Node
{
	[Export] public NodePath GroundCanvasItemPath;
	private CanvasItem GroundCanvasItem;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GroundCanvasItem = GetNode<CanvasItem>(GroundCanvasItemPath);
		GroundCanvasItem.ZIndex = RenderingLayers.GroundLayer;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
