using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using CRM.Demo.Application.Common.Behaviors;

namespace CRM.Demo.Application;

/// <summary>
/// Extension methods dla Dependency Injection w Application layer.
/// Rejestruje MediatR, AutoMapper, FluentValidation i Pipeline Behaviors.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // MediatR - rejestruje wszystkie Handlers z assembly
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // AutoMapper - rejestruje wszystkie Profile z assembly
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // FluentValidation - rejestruje wszystkie Validators z assembly
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Pipeline Behaviors (kolejność ma znaczenie!)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        return services;
    }
}
