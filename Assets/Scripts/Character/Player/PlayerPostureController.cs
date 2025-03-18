using Godot;
using System;

public partial class PlayerPostureController : Node
{
	private Player _player;
	public int CurrentPosture { get; private set; }
	
	public event Action<int> PostureChangeEvent;
	public event Action PostureBreakEvent;
	public event Action PostureRecoverEvent;

	[Export]
	private double _damageTickTime;
	[Export]
	private int _curseDamage;

	private double _damageTickCounter;
	
	public void Initialize(Player p) {
		_player = p;
	}

    public override void _Process(double delta)
    {
        base._Process(delta);

		if(CurrentPosture <= 0) {
			if(_damageTickCounter >= 0) {
				_damageTickCounter -= delta;
			} else {
				_player.TakeCurseDamage(_curseDamage, _curseDamage);
				_damageTickCounter = _damageTickTime;
			}
		} else {
			_damageTickCounter = _damageTickTime;
		}
    }

    public void ResetPosture() {
		int temp = CurrentPosture;
		CurrentPosture = _player.CurrentPlayerStats.MaxCurse;
		PostureChangeEvent?.Invoke(_player.CurrentPlayerStats.MaxCurse - temp);
	}

	public void ReduceCurse(int amt) {
		CurrentPosture += amt;
		if(CurrentPosture >= _player.CurrentPlayerStats.MaxCurse)
			CurrentPosture = _player.CurrentPlayerStats.MaxCurse;

		PostureChangeEvent?.Invoke(amt);
	}
	
	public void TakePostureDamage(int amt) {
		
		CurrentPosture -= amt;
		if(CurrentPosture <= 0) {
			PostureBreakEvent?.Invoke();
			CurrentPosture = 0;
		}
		PostureChangeEvent?.Invoke(amt);
	}
}
