using Godot;
using System;

public abstract partial class Interactable : Area2D, IInteractable
{
	[Export] private NodePath _outlinedSpritePath;
	private Sprite2D _outlinedSprite;

	[Export] private NodePath _outlinedAnimatedSpritePath;
	private AnimatedSprite2D _outlinedAnimatedSprite;

	[Export] private NodePath _interactPromptPath;
	private Node2D _interactPrompt;

	public override void _Ready()
	{
		CollisionLayer = (uint) PhysicsLayers.InteractableLayer;
		if(_outlinedSpritePath != null)
			_outlinedSprite = GetNode<Sprite2D>(_outlinedSpritePath);
		if(_outlinedAnimatedSpritePath != null) {
			_outlinedAnimatedSprite = GetNode<AnimatedSprite2D>(_outlinedAnimatedSpritePath);
			_outlinedAnimatedSprite.Play("default");
		}
		_interactPrompt = GetNode<Node2D>(_interactPromptPath);

		_interactPrompt.Visible = false;
		// _interactArea.CollisionLayer = (uint) PhysicsLayers.UntouchableLayer;

		if(_outlinedSprite != null)
			((ShaderMaterial)_outlinedSprite.GetMaterial()).SetShaderParameter("shaderon", false);
		if(_outlinedAnimatedSprite != null)
			((ShaderMaterial)_outlinedAnimatedSprite.GetMaterial()).SetShaderParameter("shaderon", false);
		_interactPrompt.Visible = false;
	}

	public virtual void OnInteract(Player player) {}
	public virtual void OnInteractHold(Player player) {}
	public virtual bool GetCondition(Player player) {
		return true;
	}
	public virtual void OnEnter(Player player) {
		// GD.Print("Shader ON");
		//GD.Print(_outlinedSprite.GetMaterial());
		if(_outlinedSprite != null)
			((ShaderMaterial)_outlinedSprite.GetMaterial()).SetShaderParameter("shaderon", true);
		if(_outlinedAnimatedSprite != null)
			((ShaderMaterial)_outlinedAnimatedSprite.GetMaterial()).SetShaderParameter("shaderon", true);
		_interactPrompt.Visible = true;
	}
	public virtual void OnExit(Player player) {
		// GD.Print("Shader OFF");
		//GD.Print(_outlinedSprite.GetMaterial());
		if(_outlinedSprite != null)
			((ShaderMaterial)_outlinedSprite.GetMaterial()).SetShaderParameter("shaderon", false);
		if(_outlinedAnimatedSprite != null)
			((ShaderMaterial)_outlinedAnimatedSprite.GetMaterial()).SetShaderParameter("shaderon", false);
		_interactPrompt.Visible = false;
	}
}
