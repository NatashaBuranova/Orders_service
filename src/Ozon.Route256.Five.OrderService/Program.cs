//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

using Ozon.Route256.Five.OrderService;

await Host
    .CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(builder => { builder.UseStartup<Startup>(); })
    .Build()
    .RunAsync();

