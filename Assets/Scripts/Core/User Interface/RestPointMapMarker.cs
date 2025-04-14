using Godot;
using System;

public partial class RestPointMapMarker : TextureRect
{
	public event Action<string> OnClickEvent;
	public string path;
	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mb)
		{
			if (mb.ButtonIndex == MouseButton.Left && mb.Pressed)
			{
				GD.Print("clicked");
				OnClickEvent?.Invoke(path);
			}
		}
	}
}
