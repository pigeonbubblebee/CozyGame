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
	
	public async void FreezeFrame(float time, float delay) {
		await ToSignal(GetTree().CreateTimer(delay), "timeout");
		GetTree().Paused = true;
		await ToSignal(GetTree().CreateTimer(time), "timeout");
		GetTree().Paused = false;
	}
}
