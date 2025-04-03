using Godot;
using System;

public partial class InventorySlot : Panel
{
	private TextureRect _itemImage;
	private RichTextLabel _label;
	private Item _item;
	public event Action<Item> OnItemClickEvent;
	private Texture2D _texture;
	private Font _numberFont;
	private int _stack;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.CustomMinimumSize = new Vector2(128, 128);
		StyleBoxTexture t = new StyleBoxTexture();
		t.Texture = _texture;
		// GD.Print(t + " texture: " + _texture);
		this.AddThemeStyleboxOverride("panel", t);
		
		TextureRect instance = new TextureRect();
		_itemImage = instance;
		
		AddChild(_itemImage);
		_itemImage.CustomMinimumSize = new Vector2(128, 128);
		if(_item != null) {
			_itemImage.Texture = _item.Image;
		}

		RichTextLabel textLabel = new RichTextLabel();
		_label = textLabel;
		AddChild(_label);

		textLabel.MouseFilter = MouseFilterEnum.Ignore;

		textLabel.CustomMinimumSize = new Vector2(72, 40);
		textLabel.Position = new Vector2(8, 96);
		textLabel.AddThemeFontOverride("normal_font", _numberFont);
		textLabel.AddThemeFontSizeOverride("normal_font_size", 16);
		textLabel.AddThemeColorOverride("default_color", new Color("cbb36a"));
		if(_item != null) {
			textLabel.Text = "[right]" + (!_item.Stackable ? "" : _stack.ToString());
		}
		textLabel.ZIndex = 2;
		textLabel.BbcodeEnabled = true;
		textLabel.Scale = new Vector2(1.5f, 1.5f);
		
		// GD.Print("text: " + _stack.ToString() + _numberFont);

		
	}
	
	public void ChangeItem(Item i, int stack) {
		_itemImage.Visible = true;
		_itemImage.Texture = i.Image;
		_item = i;
		_stack = stack;

		// GD.Print(i.Image);
		
		_label.Text = "[right]" +
			 (!_item.Stackable ? "" : _stack.ToString());
	}

	public void RemoveItem() {
		_itemImage.Visible = false;
		_item = null;
		_stack = 0;

		GD.Print("remove");

		//GD.Print(_label);
		
		_label.Text = "";
	}
	
	public InventorySlot(Item i, int stack, Texture2D texture, Font font) {
		_item = i;
		_texture = texture;
		_numberFont = font;
		_stack = stack;
	}

	public InventorySlot(Texture2D texture, Font font) {
		_texture = texture;
		_numberFont = font;
	}
	
	public InventorySlot() {
		// TODO: Add default blank texture
	}
	
	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mb)
		{
			if (mb.ButtonIndex == MouseButton.Left && mb.Pressed)
			{
				if(_item != null) {
					OnItemClickEvent?.Invoke(_item);
				}
			}
		}
	}
}
