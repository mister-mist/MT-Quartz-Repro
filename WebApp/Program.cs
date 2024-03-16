using System.Configuration;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Quartz;
using WebApp;

const string SchedulerQueueName = "repro-quartz-scheduler"; 
var SchedulerQueue = new Uri("queue:" + SchedulerQueueName);
var connectionString = "Server=(localdb)\\mssqllocaldb;Database=Repro;Trusted_Connection=True;";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    q.UsePersistentStore(s =>
    {
        s.UseProperties = true;

        s.UseSqlServer(connectionString);

        s.UseJsonSerializer();
    });
});

builder.Services.AddMassTransit(x =>
{
    x.AddPublishMessageScheduler();

    x.AddMessageScheduler(SchedulerQueue);
    x.AddQuartzConsumers(options => options.QueueName = SchedulerQueueName);

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.UseMessageScheduler(SchedulerQueue);

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