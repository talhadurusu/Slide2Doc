using Microsoft.Extensions.DependencyInjection;
using Slide2Doc.Application.Interfaces;
using Slide2Doc.Application.Services;

namespace Slide2Doc.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITopicMatcher, TopicMatcher>();
        services.AddScoped<ICompileDocumentService, CompileDocumentService>();
        return services;
    }
}
