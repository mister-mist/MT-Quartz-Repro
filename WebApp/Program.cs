using System.Configuration;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;
using WebApp;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(Settings.connectionString));


builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    q.UsePersistentStore(s =>
    {
        s.UseProperties = true;

        s.UseSqlServer(Settings.connectionString);

        s.UseJsonSerializer();
    });
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<SchedulePipelineConsumer>();
    x.AddPublishMessageScheduler();
    var q = new Uri("queue:"+Settings.SchedulerQueueName);
    x.AddMessageScheduler(q);
    x.AddQuartzConsumers(options => options.QueueName = Settings.SchedulerQueueName);

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.UseMessageScheduler(q);

        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddQuartzHostedService(configurator =>
{
    //configurator.StartDelay = TimeSpan.FromSeconds(5);
    configurator.WaitForJobsToComplete = true;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

