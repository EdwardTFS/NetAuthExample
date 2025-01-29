using Hangfire;
using Hangfire.InMemory;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;

namespace WebApi;

public class Program
{
    const string HangfirePolicy = nameof(HangfirePolicy);

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddHangfire(configuration => {
            configuration.UseInMemoryStorage();
        });
        builder.Services.AddHangfireServer();

        var azureADSection = builder.Configuration.GetSection("AzureAd");
        builder.Services.AddAuthentication().AddMicrosoftIdentityWebApp(azureADSection);
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(HangfirePolicy, policy => policy.AddAuthenticationSchemes(OpenIdConnectDefaults.AuthenticationScheme).RequireAuthenticatedUser());
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseAuthentication();
        app.UseAuthorization();
        // app.MapHangfireDashboardWithAuthorizationPolicy(HangfirePolicy,"/hangfire", new DashboardOptions
		//  	{
		//  		Authorization = new[] { new HangfireAuthorizationFilter() },
		//  		AppPath = null,
		//  	});

        app.MapHangfireDashboard("/hangfire", new DashboardOptions
		 	{
		 		Authorization = [new HangfireAuthorizationFilter()],
		 		AppPath = null,
		 	}).RequireAuthorization(HangfirePolicy);

        app.MapControllers();
        app.MapGet("/", InfoPage);

        // Use IRecurringJobManager for recurring jobs
        app.Services.GetService<IRecurringJobManager>().AddOrUpdate("test", () => Console.WriteLine("Test!"), Cron.Minutely);

        // Use IBackgroundJobClient interface for regular jobs
        app.Services.GetService<IBackgroundJobClient>().Enqueue(() => Console.WriteLine("start"));

        app.Run();
    }

    private static  Task InfoPage(HttpContext context)
        =>context.Response.WriteAsync(@$"<html><body><h1>WebApi</h1><a href=""hangfire"">hangfire</a><br/><a href=""weatherforecast"">weatherforecast</a></body></html>");
}
