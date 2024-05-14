using Godot;
using System;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionContainer
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services) {
        services.AddSingleton<IInputManager, InputManager>();

        return services;
    }
}
