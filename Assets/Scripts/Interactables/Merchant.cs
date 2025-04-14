using Godot;
using System;

public partial class Merchant : DialogueInteractable
{
	public override void OnEnter(Player player)
    {
        base.OnEnter(player);

		GetNode<SaveLoader>("/root/SaveLoader").AddMapMarker(GetNode<MainHandler>("/root/MainHandler").GetMarkerID(this.Name));
    }
}
