using Godot;
using System;

public abstract partial class Interactable : Area2D, IInteractable
{
	[Export] private NodePath _outlinedSpritePath;
	private Sprite2D _outlinedSprite;

	[Export] private NodePath _interactPromptPath;
	private Node2D _interactPrompt;

	public override void _Ready()
	{
		CollisionLayer = (uint) PhysicsLayers.InteractableLayer;
		_outlinedSprite = GetNode<Sprite2D>(_outlinedSpritePath);
		_interactPrompt = GetNode<Node2D>(_interactPromptPath);
		// _interactArea.CollisionLayer = (uint) PhysicsLayers.UntouchableLayer;
	}

	public virtual void OnInteract(Player player) {}
	public virtual void OnInteractHold(Player player) {}
	public virtual void OnEnter(Player player) {
		// GD.Print("Shader ON");
		//GD.Print(_outlinedSprite.GetMaterial());
		((ShaderMaterial)_outlinedSprite.GetMaterial()).SetShaderParameter("shaderon", true);
		_interactPrompt.Visible = true;
	}
	public virtual void OnExit(Player player) {
		// GD.Print("Shader OFF");
		//GD.Print(_outlinedSprite.GetMaterial());
		((ShaderMaterial)_outlinedSprite.GetMaterial()).SetShaderParameter("shaderon", false);
		_interactPrompt.Visible = false;
	}
}
