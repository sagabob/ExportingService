using ReportExporting.PlaceOrderApi.Handlers;

namespace ReportExporting.PlaceOrderApi;

public static class Extensions
{
    public static IServiceCollection AddMediaRExtension(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<ExportRequestHandler>());
        return services;
    }
}