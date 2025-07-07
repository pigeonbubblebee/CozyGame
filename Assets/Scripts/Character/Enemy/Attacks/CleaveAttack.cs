using Godot;
using System;

public partial class CleaveAttack : SlashAttack
{
	[Export] private NodePath _counterCheckAreaPath;
	private Area2D _counterCheckArea;
	
	[Export] private NodePath _counterExitPath;
	private Node2D _counterExit;
	
	[Export] private NodePath _cleaveCounteredSFXPath;
	private AudioStreamPlayer2D _cleaveCounteredSFX;
	
	[Export]
	public int CounterableFrame;
	[Export]
	public int CounterableOffFrame;
	
	public bool Counterable { get; private set; }
	
	private bool _countered = false;
	[Export]
	public float CounterLength { get; private set; }
	[Export]
	public float ExitGrabLength { get; private set; }
	
	private Player _currentCounterPlayer;
	
	public override void _Ready() {
		base._Ready();
		_counterExit = GetNode<Node2D>(_counterExitPath);
		_cleaveCounteredSFX = GetNode<AudioStreamPlayer2D>(_cleaveCounteredSFXPath);
	}
	
	protected override void _checkAnimationEvent() {
		base._checkAnimationEvent();
		if(EnemyAI.Sprite.Frame == CounterableFrame && EnemyAI.Sprite.Animation == AnimationName) {
			Counterable = true;
		}
		
		if(EnemyAI.Sprite.Frame == CounterableOffFrame && EnemyAI.Sprite.Animation == AnimationName) {
			Counterable = false;
		}
		
		// GD.Print("Counterable: " + Counterable); 
	}
	
	public override void Execute(Player p, Enemy e) {
		_countered = false;
		base.Execute(p, e);
	}
	
	public void Counter(Player p) {
		_cleaveCounteredSFX.Play();
		_accelerating = false;
		// _canHit = false;
		_canHit = false;
		 EnemyAI.Sprite.Play(AnimationName + "_countered");
		Counterable = false;
		_countered = true;
		_currentCounterPlayer = p;
		
		_currentCounterPlayer.GlobalPosition = GlobalPosition;
		
		GetTree().CreateTimer(CounterLength).Timeout += _FinishCounter;
		GetTree().CreateTimer(ExitGrabLength).Timeout += _PlayerExitGrab;
	}
	
	private void _PlayerExitGrab() {
		_currentCounterPlayer.GlobalPosition = _counterExit.GlobalPosition;
		_currentCounterPlayer.ExitGrab();
		
	}
	
	private void _FinishCounter() {
		EnemyAI.TakePostureDamage(_attackData.DeflectPostureDamage);
		_accelerating = false;
			// _canHit = false;
		_canHit= false;
			// _rightAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		_attackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
			// CanSwitchAttackDirection = true;
		Finish();
	}
	
	protected override void _FinishSlash() {
		if(!_countered) {
			// GD.Print("Finish!");
			_accelerating = false;
			// _canHit = false;
			_canHit= false;
			// _rightAttackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
			_attackAreaCollider.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
			// CanSwitchAttackDirection = true;
			Finish();
		}
	}
}
