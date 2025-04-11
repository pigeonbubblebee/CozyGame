using Godot;
using System;

public partial class Dialogue : Control
{
	[Export] private NodePath _portraitPath;
	private TextureRect _portrait;

	[Export] private NodePath _namePath;
	private RichTextLabel _name;

	[Export] private NodePath _textPath;
	private RichTextLabel _text;

	private DialogueData _currentDialogue;
	private int _currentIndex;

	private IInputManager _inputManager;

	[Export]
	private NodePath _namePanelPath;
	private Control _namePanel;
	[Export]
	private NodePath _panelPath;
	private Control _panel;

	private UIManager _uiManager;

    public override void _Ready()
    {
        base._Ready();
		_portrait = GetNode<TextureRect>(_portraitPath);
		_name = GetNode<RichTextLabel>(_namePath);
		_text = GetNode<RichTextLabel>(_textPath);

		_namePanel = GetNode<Control>(_namePanelPath);
		_panel = GetNode<Control>(_panelPath);

		_inputManager = GetNode<IInputManager>("/root/InputManager");
		_uiManager = GetNode<UIManager>("/root/UIManager");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

		if(_currentDialogue == null) {
			Visible = false;
			return;
		}

		if(_inputManager.GetInteractActuation()) {
			_currentIndex ++;
			if(_currentIndex >= _currentDialogue.DialogueKey.Length) {
				DialogueData temp = _currentDialogue.NextDialogue;
				bool merchant = _currentDialogue is MerchantDialogueData;

				if(merchant) {
					_uiManager.LoadMerchant(((MerchantDialogueData)_currentDialogue).Merchant);
				}

				_currentDialogue = null;
				
				LoadDialogue(temp);
			} else {
				_text.Text = Tr("dialogue_" + _currentDialogue.DialogueKey[_currentIndex]);
			}
		}
    }


	public void LoadDialogue(DialogueData d) {
		if(_currentDialogue != null) 
			return;
		if(d == null) {
			_currentDialogue = d;
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(GetNode<Player>("/root/Player").Camera, "zoom", GetNode<UIManager>("/root/UIManager").OriginalCamZoom, 0.15f);
			Tween tween2 = GetTree().CreateTween();
			tween2.TweenProperty(GetNode<Player>("/root/Player").Camera, "_Offset", GetNode<UIManager>("/root/UIManager").OriginalOffset, 0.075f);
			return;
		}
		
		_currentDialogue = d;
		_currentIndex = 0;

		_portrait.Texture = d.Portrait;
		_text.Text = Tr("dialogue_" + d.DialogueKey[0]);
		_name.Text = Tr("name_" + d.NameKey);
		_name.Visible = !d.NameKey.Equals("");
		_namePanel.Visible = !d.NameKey.Equals("");
		
		_panel.Position = new Vector2(d.NameKey.Equals("") ? 360 : 544, _panel.Position.Y);
		
	}
}
