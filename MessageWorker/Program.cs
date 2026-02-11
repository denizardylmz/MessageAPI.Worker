using MessageWorker;
using MessageWorker.Application;
using MessageWorker.Infrastructure;
using MessageWorker.Workers;



var builder = Host.CreateApplicationBuilder(args);



builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);


builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<RabbitMqConsumerWorker>();


var host = builder.Build();
host.Run();
