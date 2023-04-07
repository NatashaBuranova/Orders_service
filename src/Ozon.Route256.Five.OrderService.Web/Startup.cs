using Grpc.Net.ClientFactory;
using Ozon.Route256.Five.OrderService.Core.Services;
using Ozon.Route256.Five.OrderService.CustomersGrpsServise;
using Ozon.Route256.Five.OrderService.DateTimeProvider;
using Ozon.Route256.Five.OrderService.GrpsServices;
using Ozon.Route256.Five.OrderService.Helpers;
using Ozon.Route256.Five.OrderService.Infrastructure;
using Ozon.Route256.Five.OrderService.Infrastructure.ClientBalancing;
using Ozon.Route256.Five.OrderService.Infrastructure.DataBaseConnection;
using Ozon.Route256.Five.OrderService.Infrastructure.Repositories;
using Ozon.Route256.Five.OrderService.Kafka;
using Ozon.Route256.Five.OrderService.LogisticsSimulatorGrpsService;
using Ozon.Route256.Five.OrderService.Midlewares;
using Ozon.Route256.Five.OrderService.Services;

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

        services.AddSingleton<IDateTimeProvider, LocalDateTimeProvider>();

        services.AddServiceDiscovery(
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

        services.AddGrpcClient<LogisticsSimulatorService.LogisticsSimulatorServiceClient>(
            options =>
            {
                options.Address = new Uri("http://localhost:5080");
            });

        services.AddGrpcClient<Customers.CustomersClient>(
            options =>
            {
                options.Address = new Uri("http://localhost:5004");
            });

        services.AddDataBaseConnection();
        services.AddRepositories();

        services.AddTransient<ICancelOrderServices, CancelOrderServices>();
        services.AddTransient<IGetClientServices, GetClientServices>();
        services.AddTransient<ISendNewOrder, SendNewOrder>();

        services.AddTransient<IValidateOrderServices, ValidateOrderServices>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = _configuration.GetValue<string>("Redis:ConnectionString");
        });

        services.AddKafka(_configuration);
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
