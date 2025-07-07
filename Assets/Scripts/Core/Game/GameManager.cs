using Godot;
using Microsoft.Extensions.DependencyInjection;
using System;

public partial class GameManager : Node2D
{
	public async void FreezeFrame(float time) {
		GetTree().Paused = true;
		await ToSignal(GetTree().CreateTimer(time), "timeout");
		GetTree().Paused = false;
	}
	
	public async void FreezeFrame(double time, float delay) {
		await ToSignal(GetTree().CreateTimer(delay), "timeout");
		GetTree().Paused = true;
		Engine.TimeScale = 0.1f;
		await ToSignal(GetTree().CreateTimer(time / 10), "timeout");
		GetTree().Paused = false;
		Engine.TimeScale = 1.0f;
	}

	// public void FreezeFrame(float time, float delay) {
	// 	GetTree().CreateTimer(delay).Timeout += delegate { 
	// 		Engine.TimeScale = 0.0f;
	// 		GetTree().Paused = true; 
	// 		GetTree().CreateTimer(time).Timeout += delegate { GetTree().Paused = false; Engine.TimeScale = 1.0f; };	
	// 	};
	// }
}
