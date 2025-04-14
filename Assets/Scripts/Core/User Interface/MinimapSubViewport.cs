using Godot;
using System;

public partial class MinimapSubViewport : SubViewport
{
	[Export] private NodePath _cameraPath;
	private Camera2D _camera;

	private Player _player;
	public override void _Ready()
	{
		_camera = GetNode<Camera2D>(_cameraPath);
		_player = GetNode<Player>("/root/Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_camera.Position = _player.Position;
	}
}
