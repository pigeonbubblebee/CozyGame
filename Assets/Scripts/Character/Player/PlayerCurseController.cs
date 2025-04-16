using Godot;
using System;

public partial class PlayerCurseController : Node
{
	private Player _player;
	public int CurrentCurse { get; private set; }
	public int MaxCurse;
	
	public event Action<int> CurseChangeEvent;
	// public event Action PostureBreakEvent;
	// public event Action PostureRecoverEvent;

	[Export]
	private double _damageTickTime;
	[Export]
	private int _curseDamage;

	private double _damageTickCounter;

	[Export]
	private NodePath _maxCurseParticlePath;
	private GpuParticles2D _maxCurseParticle;
	
	public void Initialize(Player p) {
		_player = p;
	}

    public override void _Ready()
    {
        base._Ready();
		_maxCurseParticle = GetNode<GpuParticles2D>(_maxCurseParticlePath);
		_maxCurseParticle.Emitting = false;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

		if(CurrentCurse >= MaxCurse) {
			_maxCurseParticle.Emitting = true;
			if(_damageTickCounter >= 0) {
				_damageTickCounter -= delta;
			} else {
				// _player.TakeCurseDamage(_curseDamage, _curseDamage);
				_damageTickCounter = _damageTickTime;
			}
		} else {
			_maxCurseParticle.Emitting = false;
			_damageTickCounter = _damageTickTime;
		}
    }

    public void ResetCurse() {
		int temp = CurrentCurse;
		CurrentCurse = 0;
		CurseChangeEvent?.Invoke(_player.CurrentPlayerStats.MaxCurse - temp);
	}

	public void SetCurse(int amt) {
		int temp = CurrentCurse;
		CurrentCurse = amt;
		CurseChangeEvent?.Invoke(_player.CurrentPlayerStats.MaxCurse - temp);
	}

	public void ReduceCurse(int amt) {
		CurrentCurse -= amt;
		if(CurrentCurse <= 0)
			CurrentCurse = 0;

		CurseChangeEvent?.Invoke(amt);
	}
	
	public void AddCurse(int amt) {
		CurrentCurse += amt;
		if(CurrentCurse >= MaxCurse) {
			// PostureBreakEvent?.Invoke();
			CurrentCurse = MaxCurse;
		}
		CurseChangeEvent?.Invoke(amt);
	}
}
