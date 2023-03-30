using Dapper;
using Grpc.Net.ClientFactory;
using Ozon.Route256.Five.OrderService.ClientBalancing;
using Ozon.Route256.Five.OrderService.DateTimeProvider;
using Ozon.Route256.Five.OrderService.GrpsServices;
using Ozon.Route256.Five.OrderService.Helpers;
using Ozon.Route256.Five.OrderService.Infrastructure;
using Ozon.Route256.Five.OrderService.Kafka;
using Ozon.Route256.Five.OrderService.Midlewares;
using Ozon.Route256.Five.OrderService.Repositories;
using Ozon.Route256.Five.OrderService.Repositories.DataBaseImp;
using Ozon.Route256.Five.OrderService.Repositories.ShardImp;
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
        SqlMapper.AddTypeHandler(AddressObjectHandler.Instance);

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddGrpc(options => { options.Interceptors.Add<LoggerInterceptor>(); });
        services.AddGrpcReflection();

        services.AddSingleton<IDateTimeProvider, LocalDateTimeProvider>();
        services.AddSingleton<IDbStore, DbStore>();
        services.AddHostedService<SdConsumerHostedService>();
        services.AddGrpcClient<SdService.SdServiceClient>(
            options =>
            {
                options.Address = new Uri("http://localhost:5010");
                //options.Address = new Uri("http://localhost:5004");
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

        services.AddScoped<IShardingRule<long>, RoundRobinLongShardingRule>();
        services.AddScoped<IShardingRule<string>, RoundRobinStringShardingRule>();
        services.AddScoped<IShardConnectionFactory, ShardConnectionFactory>();

        services.AddScoped<IOrderRepository, OrderShardRepository>();
        services.AddScoped<IRegionRepository, RegionShardRepository>();
        services.AddScoped<IClientRepository, ClientShardRepository>();

        services.AddScoped<IPostgresConnectionFactory, PostgresConnectionFactory>();

        services.AddTransient<ICancelOrderServices, CancelOrderServices>();
        services.AddTransient<IGetClientServices, GetClientServices>();
        services.AddTransient<ISendNewOrder, SendNewOrder>();

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
