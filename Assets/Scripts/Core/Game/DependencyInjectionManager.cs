using Godot;
using System;
using Microsoft.Extensions.DependencyInjection;

public partial class DependencyInjectionManager : Node
{
    public IServiceProvider ServiceProvider { get; set; }
    public IServiceProvider Init()
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IInputManager, InputManager>()
            .BuildServiceProvider();

        ServiceProvider = serviceProvider;
        return serviceProvider;
    }
}
