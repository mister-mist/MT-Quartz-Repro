using MassTransit.Scheduling;

namespace WebApp;

public class PipelineRecurringSchedule : PipelineDefaultRecurringSchedule
{
    public PipelineRecurringSchedule(string scheduleId, string scheduleGroup) : base(scheduleId, scheduleGroup)
    {
        CronExpression = "0/5 * * ? * * *"; 
    }
}
public abstract class PipelineDefaultRecurringSchedule : RecurringSchedule
{
    protected PipelineDefaultRecurringSchedule(string scheduleId, string scheduleGroup)
    {
        ScheduleId = scheduleId;
        ScheduleGroup = scheduleGroup;

        TimeZoneId = TimeZoneInfo.Local.Id;
        StartTime = DateTime.Now;
    }

    public MissedEventPolicy MisfirePolicy { get; protected set; }
    public string TimeZoneId { get; protected set; }
    public DateTimeOffset StartTime { get; protected set; }
    public DateTimeOffset? EndTime { get; protected set; }
    public string ScheduleId { get; private set; }
    public string ScheduleGroup { get; private set; }
    public string CronExpression { get; protected set; }
    public string Description { get; protected set; }
}