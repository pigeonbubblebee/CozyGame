using Godot;
using System;

public partial class QuickMapUI : Control
{
	[Export] private NodePath _mapPath;
	private Map _map;

    public override void _Ready()
    {
        base._Ready();
		_map = GetNode<Map>(_mapPath);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
		if(Visible)
			_map.UpdateMapVisibility();
    }


}
