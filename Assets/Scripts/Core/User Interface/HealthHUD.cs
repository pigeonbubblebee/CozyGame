using Godot;
using System;
using System.Collections.Generic;

public partial class HealthHUD : Node
{
	private List<TextureRect> healthImages = new List<TextureRect>();
	
	[Export] private Texture2D _healthImageHit;
	[Export] private Texture2D _healthImageUnhit;
	
	public override void _Ready() {

	}
	
	public void UpdateHealth(int curHealth, int maxHealth) {
		int addCount = 0;
		int removeCount = 0;
		
		// Get amount of health nodes needed to add/remove
		if(healthImages.Count > maxHealth) {
			removeCount = healthImages.Count - maxHealth;
		} else {
			addCount = maxHealth - healthImages.Count;
		}
		
		// Remove all unecessary health texture nodes
		for(int i = 0; i < removeCount; i++) {
			// this.RemoveChild(n);
			healthImages[i].QueueFree();
		}
		healthImages.RemoveRange(0, removeCount);
		
		// Add necessary nodes
		for(int i = 0; i < addCount; i++) {
			TextureRect instance = new TextureRect();
			AddChild(instance);
			healthImages.Add(instance);
		}
		
		// Correct the textures to make sure the health is displaying an empty or full heart
		for(int i = 0; i < healthImages.Count; i++) {
			if(i < curHealth) {
				healthImages[i].Texture = _healthImageUnhit;
			} else {
				healthImages[i].Texture = _healthImageHit;
			}
		}
	}
}
