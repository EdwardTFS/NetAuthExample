using Hangfire.Dashboard;

namespace WebApi
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public HangfireAuthorizationFilter()
        {
        }

        public bool Authorize(DashboardContext dashboardContext)
        {
            var httpContext = dashboardContext.GetHttpContext();
            Console.WriteLine($"HangfireAuthorizationFilter IsAuth={httpContext.User?.Identity?.IsAuthenticated} User={httpContext.User?.Identity?.Name} "); 
            return true;
        }
    }
}
