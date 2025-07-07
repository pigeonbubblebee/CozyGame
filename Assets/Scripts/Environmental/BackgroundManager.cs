using Godot;
using System;

public partial class BackgroundManager : Node2D
{
	[Export] private NodePath[] bgPaths;
	private Node2D[] _bg;

	public override void _Ready()
    {
        base._Ready();
		_bg = new Node2D[bgPaths.Length];

		for(int i = 0; i < bgPaths.Length; i++) {
			_bg[i] = GetNode<Node2D>(bgPaths[i]);
		}

		GetNode<MainHandler>("/root/MainHandler").BackgroundChangeEvent += _ChangeBG;
    }

	private void _ChangeBG(string name) {
		foreach(Node2D n in _bg) {
			if(n.Name.Equals(name))
				n.Visible = true;
			else
				n.Visible = false;
		}
	}
}
