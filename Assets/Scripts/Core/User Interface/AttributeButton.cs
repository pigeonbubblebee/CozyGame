using Godot;
using System;

public partial class AttributeButton : Panel
{
	[Export] private bool _increment;
	public event Action IncrementEvent;
	public event Action DecrementEvent;

	public bool Selected;
	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mb)
		{
			if (mb.ButtonIndex == MouseButton.Left && mb.Pressed)
			{
				GD.Print("Clicked!");

				Selected = true;

				if(_increment)
					IncrementEvent?.Invoke();
				else
					DecrementEvent?.Invoke();
			}
		}
	}

    public override void _Ready()
    {
        base._Ready();
		MouseExited += delegate() {
			Selected = false;
		};
    }

}
