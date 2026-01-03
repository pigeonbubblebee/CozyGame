using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerSummonController : Node
{
	private List<Summon> summons = new List<Summon>();
    private Player _player;
    public event Action<Enemy, int, Summon, Player> SummonHitEvent;

    public void Initialize(Player p) {
        _player = p;
    }

    public void AddSummon(string summonID) {
        PackedScene _summonScene = ResourceLoader.Load<PackedScene>("res://Assets/Scene/Summons/" + summonID + ".tscn");
        Summon summon = (Summon)_summonScene.Instantiate();
		summons.Add(summon);
        summon.Initialize(_player);
        AddChild(summon);
		summon.Position = _player.GlobalPosition;
        summon.OnHitEvent += _InvokeSummonHitEvent;
    }

    private void _InvokeSummonHitEvent(Enemy e, int damage, Summon s)
    {
        SummonHitEvent?.Invoke(e, damage, s, _player);
    }

    public void ClearSummons() {
        foreach(Summon s in summons) {
            s.OnHitEvent -= _InvokeSummonHitEvent;
            s.QueueFree();
        }

        summons = new List<Summon>();
    }

    public void RemoveSummon(string summonID) {
        Summon remove = null;
        foreach(Summon s in summons) {
            if(s.SummonID.Equals(summonID)) {
                s.OnHitEvent -= _InvokeSummonHitEvent;
                remove = s;
            }
        }

        if(remove != null) {
            remove.QueueFree();
            summons.Remove(remove);
        }
    }
}
