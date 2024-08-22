using Godot;
using System;

[GlobalClass]
public abstract partial class Item : Resource {
    [Export] public string ID { get; set; }
    [Export] public string Description { get; set; }
    [Export] public Texture Image { get; set; }
}