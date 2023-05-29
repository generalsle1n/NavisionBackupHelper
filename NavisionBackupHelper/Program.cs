using NavisionBackupHelper;
using Quartz;
using Serilog;
using System.Reflection;

string CurrentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
IConfiguration _tempConfig = new ConfigurationBuilder()
    .AddJsonFile(Path.Combine(CurrentFolder, "appsettings.json"))
    .Build();

JobKey MainJobKey = new JobKey("NavisionBackup");

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddQuartzHostedService(settings =>
        {
            settings.WaitForJobsToComplete = true;
        });
        services.AddQuartz(queue =>
        {
            queue.UseMicrosoftDependencyInjectionJobFactory();
            
            queue.AddJob<BackupJob>(job =>
            {
                job.WithIdentity(MainJobKey);
            });

            queue.AddTrigger(trigger =>
            {
                trigger.WithDailyTimeIntervalSchedule(settings =>
                {
                    settings.WithIntervalInHours(_tempConfig.GetValue<int>("NavisionBackup:RecuringHour"));
                    settings.OnEveryDay();
                    settings.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(
                        _tempConfig.GetValue<int>("NavisionBackup:StartHour"),
                        _tempConfig.GetValue<int>("NavisionBackup:StartMinute"))
                        );
                });
                trigger.ForJob(MainJobKey);
            });
        });
    })
    .ConfigureLogging(Logger =>
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(path: CurrentFolder)
            .CreateLogger();
    })
    .UseWindowsService()
    .UseSerilog()
    .Build();


var schedulerFactory = host.Services.GetRequiredService<ISchedulerFactory>();
var scheduler = await schedulerFactory.GetScheduler();

// define the job and tie it to our HelloJob class
//var job = JobBuilder.Create<BackupJob>()
//    .Build();

// Trigger the job to run now, and then every 40 seconds

//var trigger = TriggerBuilder.Create()
//    .WithIdentity("myTrigger", "group1")
//    .WithDailyTimeIntervalSchedule(settings =>
//    {
//        settings.WithIntervalInHours(24);
//        settings.OnEveryDay();
//        settings.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(16, 32));
//    })
//    .Build();
////await scheduler.ScheduleJob(job, trigger);

host.Run();
