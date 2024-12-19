using Godot;
using System;

public partial class PlayerAnimationController : Node
{
	[Export] public NodePath SpritePath { get; private set; }
	public AnimatedSprite2D Sprite { get; private set; }
	
	public AsymmetricalAnimationClip IdleAnimationClip { get; private set; }	
	public AsymmetricalAnimationClip RunAnimationClip { get; private set; }
	public AsymmetricalAnimationClip SlashAnimationClip { get; private set; }
	public AsymmetricalAnimationClip Slash2AnimationClip { get; private set; }
	public AsymmetricalAnimationClip BlockAnimationClip { get; private set; }
	public AsymmetricalAnimationClip JumpAnimationClip { get; private set; }
	public AsymmetricalAnimationClip FallAnimationClip { get; private set; }
	public AsymmetricalAnimationClip CounterAnimationClip { get; private set; }
	public AsymmetricalAnimationClip DeathBlowAnimationClip { get; private set; }

	public AsymmetricalAnimationClip CurrentAnimation { get; private set; }
	
	private PlayerMovementController _movementController;
	private PlayerStateMachine _playerStateMachine;
	
	private bool _spriteFacingRight = true;
	
	[Export] private bool _logAnimations = true;
	
	public void Initialize(Player p) {
		_movementController = p.MovementController;
		_playerStateMachine = p.StateMachine;
	}
	
	public override void _Ready() {
		
		Sprite = GetNode<AnimatedSprite2D>(SpritePath);
		
		IdleAnimationClip = new AsymmetricalAnimationClip("idle");
		RunAnimationClip = new AsymmetricalAnimationClip("run");
		SlashAnimationClip = new AsymmetricalAnimationClip("slash");
		Slash2AnimationClip = new AsymmetricalAnimationClip("slash2");
		BlockAnimationClip = new AsymmetricalAnimationClip("block");
		JumpAnimationClip = new AsymmetricalAnimationClip("jump");
		FallAnimationClip = new AsymmetricalAnimationClip("fall");
		CounterAnimationClip = new AsymmetricalAnimationClip("counter");
		DeathBlowAnimationClip = new AsymmetricalAnimationClip("deathblow");
	}
	
	public override void _Process(double delta) {		
		_flipCheck();
	}
	
	private void _flipCheck() {
		if(_movementController.Direction > 0 && !_spriteFacingRight) {
			_flip();
		} else if(_movementController.Direction < 0 && _spriteFacingRight) {
			_flip();
		}
	}

	// <summary>
	// Flips sprite.
	// <summary/>
	private void _flip() {
		// GD.Print((_playerStateMachine.CurrentState as PlayerState).CanFlip + "Animation");
		if(!(_playerStateMachine.CurrentState as PlayerState).CanFlip) {
			return;
		}
		_spriteFacingRight=!_spriteFacingRight;
		// FacingDirection *= -1;

		
		CurrentAnimation.PlayAnimation(Sprite, _spriteFacingRight);
	}

	public void PlayAnimation(AsymmetricalAnimationClip animationClip) {
		if(_logAnimations) {
			GD.Print(animationClip.RightClip);
		}
		_resetAnimationSpeed(); // Reset Animation Speed
		animationClip.PlayAnimation(Sprite, _movementController.Direction == 1);
		CurrentAnimation = animationClip;
	}

	public void PlayAnimation(AsymmetricalAnimationClip animationClip, float speed) {
		if(_logAnimations) {
			GD.Print(animationClip.RightClip);
		}
		_setAnimationSpeed(speed); // Reset Animation Speed
		animationClip.PlayAnimation(Sprite, _movementController.Direction == 1);
		CurrentAnimation = animationClip;
	}

	private void _resetAnimationSpeed() {
		_setAnimationSpeed(1f);
	}

	private void _setAnimationSpeed(float speed) {
		Sprite.SpeedScale = 1/speed;
	}

	public class AsymmetricalAnimationClip {
		public string RightClip { get; private set; }
		public string LeftClip { get; private set; }
		
		// public bool Right;

		public AsymmetricalAnimationClip(string r, string l) {
			RightClip = r;
			LeftClip = l;
			// Right = true;
		}
		
		public AsymmetricalAnimationClip(string n) {
			RightClip = n + "_right";
			LeftClip = n + "_left";
			// Right = true;
		}

		public virtual void PlayAnimation(AnimatedSprite2D animator, bool facingRight) {
			// Right = facingRight;
			animator.Play(facingRight ? RightClip : LeftClip);
		}
	}
	/*
	public class AsymmetricalTransition : AsymmetricalAnimationClip {
		public string RightTransClip { get; private set; }
		public string LeftTransClip { get; private set; }
		
		public AsymmetricalAnimationClip(string initial, string trans) {
			RightClip = initial + "_right";
			LeftClip = initial + "_left";
			// Right = true;
			
			RightTransClip = trans + "_right";
			LeftTransClip = trans + "_left";
		}
		
		public async void PlayAnimationTranstition(AnimatedSprite2D animator, bool facingRight) {
			//animator.AnimationFinished += _playTrans(animator, facingRight);
			// Right = facingRight;
			animator.Play(facingRight ? RightClip : LeftClip);
			await GetTree().CreateTimer(animTime).Timeout;
			animator.Play(facingRight ? RightTransClip : LeftTransClip);
		}
	}
	*/
}
