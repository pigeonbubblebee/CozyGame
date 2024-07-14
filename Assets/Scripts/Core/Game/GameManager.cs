using Godot;
using Microsoft.Extensions.DependencyInjection;
using System;
public partial class GameManager : Node2D
{
	// public IInputManager GameInputManager { get; set; }
	// [Export] public SceneManager StartingSceneManager; // Move to Resource
	// private SceneManager _currentSceneManager;
	// private Player _currentScenePlayer;

	// public override void _Ready()
	// {
	// 	// var startup = GetNode<DependencyInjectionManager>("/root/DependencyInjectionManager");
	// 	// GameInputManager = startup.ServiceProvider.GetService<IInputManager>();
	// 	// GD.Print("Sucessful!" + GameInputManager);
	// }
	// // public override void _Process(double delta) {
	// // 	GD.Print(GameInputManager);
	// // }
	// public void LoadNewScene() {
	// 	// Get current Scene Manager, load player into that
	// }
}
