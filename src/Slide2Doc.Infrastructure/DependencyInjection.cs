using Microsoft.Extensions.DependencyInjection;
using Slide2Doc.Application.Interfaces;
using Slide2Doc.Infrastructure.Composers;
using Slide2Doc.Infrastructure.Extractors;
using Slide2Doc.Infrastructure.Indexers;

namespace Slide2Doc.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IPptxTopicExtractor, OpenXmlPptxTopicExtractor>();
        services.AddScoped<IDocxSectionIndexer, OpenXmlDocxSectionIndexer>();
        services.AddScoped<IDocxComposer, OpenXmlDocxComposer>();
        return services;
    }
}
