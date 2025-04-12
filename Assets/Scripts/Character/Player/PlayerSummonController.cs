using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerSummonController : Node
{
	private List<Summon> summons = new List<Summon>();
    private Player _player;

    public void Initialize(Player p) {
        _player = p;
    }

    public void AddSummon(string summonID) {
        PackedScene _summonScene = GD.Load<PackedScene>("res://Assets/Scene/Summons/" + summonID + ".tscn");
        Summon summon = (Summon)_summonScene.Instantiate();
		summons.Add(summon);
        summon.Initialize(_player);
        AddChild(summon);
		summon.Position = _player.GlobalPosition;
    }

    public void ClearSummons() {
        foreach(Summon s in summons) {
            s.QueueFree();
        }

        summons = new List<Summon>();
    }

    public void RemoveSummon(string summonID) {
        Summon remove = null;
        foreach(Summon s in summons) {
            if(s.SummonID.Equals(summonID)) {
                remove = s;
            }
        }

        if(remove != null) {
            remove.QueueFree();
            summons.Remove(remove);
        }
    }
}
