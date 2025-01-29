using Hangfire;
using Hangfire.InMemory;

namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddHangfire(configuration => {
            configuration.UseInMemoryStorage();
        });
        builder.Services.AddHangfireServer();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        
        app.UseAuthorization();
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
			{
				
				Authorization = new[] { new HangfireAuthorizationFilter() },
				AppPath = null
			});

        app.MapControllers();
        app.MapGet("/", InfoPage);
        BackgroundJob.Enqueue(() => Console.WriteLine("start"));
        RecurringJob.AddOrUpdate("test", () => Console.WriteLine("Test!"), Cron.Minutely);

        app.Run();
    }

    private static  Task InfoPage(HttpContext context)
        =>context.Response.WriteAsync(@$"<html><body><h1>WebApi</h1><a href=""hangfire"">hangfire</a></body></html>");
}
