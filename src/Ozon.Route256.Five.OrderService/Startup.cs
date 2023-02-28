﻿using Ozon.Route256.Five.OrderService.ClientBalancing;
using Ozon.Route256.Five.OrderService.GrpsServices;
using Ozon.Route256.Five.OrderService.Midlewares;

namespace Ozon.Route256.Five.OrderService;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddGrpc(options => { options.Interceptors.Add<LoggerInterceptor>(); });
        services.AddGrpcReflection();

        services.AddSingleton<IDbStore, DbStore>();
        //services.AddHostedService<SdConsumerHostedService>();
        //services.AddGrpcClient<SdService.SdServiceClient>(
        //    options =>
        //    {
        //        options.Address = new Uri("http://localhost:5081");
        //        options.InterceptorRegistrations.Add(
        //            new InterceptorRegistration(
        //                InterceptorScope.Client,
        //                sp =>
        //                {
        //                    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

        //                    return new LoggerInterceptor(loggerFactory.CreateLogger<LoggerInterceptor>());
        //                }));
        //    });

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
