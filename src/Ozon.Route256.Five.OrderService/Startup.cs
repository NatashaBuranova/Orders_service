using Grpc.Net.ClientFactory;
using Ozon.Route256.Five.OrderService.ClientBalancing;
using Ozon.Route256.Five.OrderService.GrpsServices;
using Ozon.Route256.Five.OrderService.Helpers;
using Ozon.Route256.Five.OrderService.Midlewares;
using Ozon.Route256.Five.OrderService.Repositories;
using Ozon.Route256.Five.OrderService.Repositories.ImMemoryImp;
using Ozon.Route256.Five.OrderService.Services;
using InterceptorRegistration = Grpc.Net.ClientFactory.InterceptorRegistration;

namespace Ozon.Route256.Five.OrderService;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddGrpc(options => { options.Interceptors.Add<LoggerInterceptor>(); });
        services.AddGrpcReflection();

        services.AddSingleton<IDbStore, DbStore>();
        services.AddHostedService<SdConsumerHostedService>();
        services.AddGrpcClient<SdService.SdServiceClient>(
            options =>
            {
                options.Address = new Uri("http://localhost:5081");
                options.InterceptorRegistrations.Add(
                    new InterceptorRegistration(
                        InterceptorScope.Client,
                        sp =>
                        {
                            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                            return new LoggerInterceptor(loggerFactory.CreateLogger<LoggerInterceptor>());
                        }));
            });

        services.AddGrpcClient<LogisticsSimulatorService.LogisticsSimulatorService.LogisticsSimulatorServiceClient>(
            options =>
            {
                options.Address = new Uri("http://localhost:5080");
            });

        services.AddGrpcClient<Customers.Customers.CustomersClient>(
            options =>
            {
                options.Address = new Uri("http://localhost:5004");
            });

        services.AddScoped<IOrderRepository, OrderInMemoryRepository>();
        services.AddScoped<IRegionRepository, RegionInMemoryRepository>();

        services.AddTransient<ICanceledOrderServices, CanceledOrderServices>();
        services.AddTransient<IGetClientServices, GetClientServices>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = _configuration.GetValue<string>("Redis:ConnectionString");
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGrpcService<OrdersService>();
            endpoints.MapGrpcReflectionService();

        });

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}
