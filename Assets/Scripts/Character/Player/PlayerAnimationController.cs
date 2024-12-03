using Godot;
using System;

public partial class PlayerAnimationController : Node
{
	[Export] public NodePath SpritePath { get; private set; }
	private AnimatedSprite2D _sprite;
	
	public AsymmetricalAnimationClip IdleAnimationClip { get; private set; }	
	public AsymmetricalAnimationClip RunAnimationClip { get; private set; }

	public AsymmetricalAnimationClip CurrentAnimation { get; private set; }
	
	private PlayerMovementController _movementController;
	private PlayerStateMachine _playerStateMachine;
	
	private bool _spriteFacingRight = true;
	
	public void Initialize(Player p) {
		_movementController = p.MovementController;
		_playerStateMachine = p.StateMachine;
	}
	
	public override void _Ready() {
		_sprite = GetNode<AnimatedSprite2D>(SpritePath);
		
		IdleAnimationClip = new AsymmetricalAnimationClip("idle_right", "idle_left");
		RunAnimationClip = new AsymmetricalAnimationClip("run_right", "run_left");
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

		
		CurrentAnimation.PlayAnimation(_sprite, _spriteFacingRight);
	}

	public void PlayAnimation(AsymmetricalAnimationClip animationClip) {
		_resetAnimationSpeed(); // Reset Animation Speed
		animationClip.PlayAnimation(_sprite, _movementController.Direction == 1);
		CurrentAnimation = animationClip;
	}

	public void PlayAnimation(AsymmetricalAnimationClip animationClip, float speed) {
		_setAnimationSpeed(speed); // Reset Animation Speed
		animationClip.PlayAnimation(_sprite, _movementController.Direction == 1);
		CurrentAnimation = animationClip;
	}

	private void _resetAnimationSpeed() {
		_setAnimationSpeed(1f);
	}

	private void _setAnimationSpeed(float speed) {
		_sprite.SpeedScale = 1/speed;
	}

	public struct AsymmetricalAnimationClip {
		public string RightClip { get; private set; }
		public string LeftClip { get; private set; }
		
		// public bool Right;

		public AsymmetricalAnimationClip(string r, string l) {
			RightClip = r;
			LeftClip = l;
			// Right = true;
		}

		public void PlayAnimation(AnimatedSprite2D animator, bool facingRight) {
			// Right = facingRight;
			animator.Play(facingRight ? RightClip : LeftClip);
		}
	}
}
