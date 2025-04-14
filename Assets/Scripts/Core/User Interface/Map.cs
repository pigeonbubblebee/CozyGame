using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using RoomData = SaveFile.RoomData;

public partial class Map : Control
{
	[Export] private NodePath[] _minimapRoomsContainerPaths;
	private Dictionary<string, TextureRect> _minimapRooms = new Dictionary<string, TextureRect>();
	[Export] private NodePath _minimapMarkerContainerPath;
	private Dictionary<string, Control[]> _minimapMarkers = new Dictionary<string, Control[]>();

	[Export] private NodePath _playerIconPath;
	private Control _playerIcon;
	private SaveLoader _saveLoader;
	private Player _player;

	private readonly float REAL_TO_MAP_SPACE_RATIO = 162.89f;
	private readonly float OFFSET_H = 54.1224139f + 70f;
	private readonly float OFFSET_V = 186f - 21.5605623f;

    public override void _Ready()
    {
        base._Ready();

		_saveLoader = GetNode<SaveLoader>("/root/SaveLoader");
		_playerIcon = GetNode<Control>(_playerIconPath);
		_player = GetNode<Player>("/root/Player");
		
		foreach(NodePath path in _minimapRoomsContainerPaths) {
			Node parent = GetNode<Node>(path);

			// _minimapRooms = new TextureRect[parent.GetChildren().Count];
			foreach(Node n in parent.GetChildren()) {
				((TextureRect)n).Visible = false;
				_minimapRooms.Add(n.Name, (TextureRect)n);
			}
		}

		Node markerParent = GetNode<Node>(_minimapMarkerContainerPath);

		foreach(Node area in markerParent.GetChildren()) {
			foreach(Node room in area.GetChildren()) {
				Control[] markers = new Control[room.GetChildren().Count];
				for(int i = 0; i < room.GetChildren().Count; i++) {
					markers[i] = (Control)(room.GetChildren()[i]);
					markers[i].Visible = false;

					if(markers[i] is RestPointMapMarker) {
						((RestPointMapMarker)markers[i]).path = area.Name + ";" + room.Name + ";" + markers[i].Name;
						((RestPointMapMarker)markers[i]).OnClickEvent += _fastTravel;
					}
				}
				_minimapMarkers.Add(area.Name + ";" + room.Name, markers);
			}
		}
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
		if(Input.IsActionJustPressed("debug")) { // TODO: Delete this before export
			GD.Print(_playerIcon.Position);
		}
    }

	public void UpdateMapVisibility() {
		_playerIcon.Position = new Vector2(_player.GlobalPosition.X / REAL_TO_MAP_SPACE_RATIO + OFFSET_H, 
			_player.GlobalPosition.Y / REAL_TO_MAP_SPACE_RATIO + OFFSET_V);

		foreach(string id in _saveLoader.GetExploredRooms()) {
			if(_minimapRooms.ContainsKey(id)) {
				_minimapRooms[id].Visible = true;
			}
		}

		foreach(string marker in _saveLoader.GetMapMarkers()) {
			string[] code = marker.Split(';');
			if(_minimapMarkers.ContainsKey(code[0] + ";" + code[1])) {
				foreach(Control c in _minimapMarkers[code[0] + ";" + code[1]]) {
					if(c.Name.Equals(code[2])) {
						c.Visible = true;
					}
				}
			}
		}
	}

	private void _fastTravel(string path) {
		GetNode<MainHandler>("/root/MainHandler").FastTravel(path);
	}
}
