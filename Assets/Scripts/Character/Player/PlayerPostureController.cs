using Godot;
using System;

public partial class PlayerPostureController : Node
{
	private Player _player;
	public int CurrentPosture { get; private set; }
	
	public event Action<int> PostureChangeEvent;
	
	public void Initialize(Player p) {
		_player = p;
	}
	
	public void ResetPosture() {
		PostureChangeEvent?.Invoke(_player.CurrentPlayerStats.MaxPosture - CurrentPosture);
		CurrentPosture = _player.CurrentPlayerStats.MaxPosture;
	}
	
	public void TakePostureDamage(int amt) {
		
		CurrentPosture -= amt;
		if(CurrentPosture < 0) {
			CurrentPosture = 0;
		}
		PostureChangeEvent?.Invoke(amt);
	}
}
