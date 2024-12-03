using Godot;
using System;

public partial class Tilemap : Godot.TileMap, IHittable
{
	[Export] private NodePath _groundCanvasItemPath;
	private CanvasItem GroundCanvasItem;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GroundCanvasItem = GetNode<CanvasItem>(_groundCanvasItemPath);
		GroundCanvasItem.ZIndex = RenderingLayers.GroundLayer;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	
	public void OnHit(Player player, int damage, int direction, int postureDamage) {
		GD.Print("DONG!");
	}

	public Vector2 GetSlashEffectPosition(Vector2 playerPos) {
		return this.GlobalPosition; // tODO: Add player pos to input
	}
}
