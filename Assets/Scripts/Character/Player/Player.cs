using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public NodePath StateMachinePath;
	public PlayerStateMachine StateMachine  { get; private set; }
	[Export] public NodePath MovementControllerPath;
	public PlayerMovementController MovementController  { get; private set; }
	[Export] public NodePath AttackControllerPath;
	public PlayerAttackController AttackController  { get; private set; }
	[Export] public NodePath PlayerSpritePath;
	public Sprite2D PlayerSprite  { get; private set; }

	public override void _EnterTree()
	{
		base._EnterTree();

		MovementController = GetNode<PlayerMovementController>(MovementControllerPath);
		AttackController = GetNode<PlayerAttackController>(AttackControllerPath);
		StateMachine = GetNode<PlayerStateMachine>(StateMachinePath);
		PlayerSprite = GetNode<Sprite2D>(PlayerSpritePath);

		StateMachine.Initialize(this);
		MovementController.Initialize(this);
		AttackController.Initialize(this);

		PlayerSprite.ZIndex = RenderingLayers.PlayerLayer;
	}
}
