using MassTransit;

namespace WebApp;
public record SchedulePipelineStart
{
    public Guid PipelineId { get; init; }

    //public StartPipelineItem StartRequest { get; init; }
}

public class SchedulePipelineConsumer : IConsumer<SchedulePipelineStart>
{

    public SchedulePipelineConsumer()
    {
    }
    public async Task Consume(ConsumeContext<SchedulePipelineStart> context)
    {
        
        var schedulerEndpoint = await context.GetSendEndpoint(new Uri("queue:" +Settings.SchedulerQueueName));
        
        var pipelineRecurringSchedule = new PipelineRecurringSchedule(
            context.Message.PipelineId.ToString(), "PipelineStartGroup");
        var scheduledRecurringMessage = 
            await schedulerEndpoint.ScheduleRecurringSend(
            new Uri("queue:PollExternalSystem"), 
            pipelineRecurringSchedule, 
            
            new PollExternalSystem(){Message = "msg"});
                
                
            
        

        
    }
}

public class PollExternalSystem
{
    public string? Message { get; set; }
}

public class PollExternalSystemConsumer : IConsumer<PollExternalSystem>
{
    private readonly ILogger<PollExternalSystemConsumer> _logger;

    public PollExternalSystemConsumer(ILogger<PollExternalSystemConsumer> logger)
    {
        _logger = logger;
    }
    public Task Consume(ConsumeContext<PollExternalSystem> context)
    {
        _logger.LogInformation("Polling");
        return Task.CompletedTask;
    }
}