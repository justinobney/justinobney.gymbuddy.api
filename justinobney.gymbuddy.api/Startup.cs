using Hangfire;
using Hangfire.Dashboard;
using justinobney.gymbuddy.api;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace justinobney.gymbuddy.api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration
                .UseSqlServerStorage("AppContext");
            
            var filter = new BasicAuthAuthorizationFilter(
                new BasicAuthAuthorizationFilterOptions
                {
                    // Require secure connection for dashboard
                    RequireSsl = false,
                    SslRedirect = false,
                    // Case sensitive login checking
                    LoginCaseSensitive = true,
                    // Users
                    Users = new[]
                    {
                        new BasicAuthAuthorizationUser
                        {
                            Login = "jobney",
                            // Password as plain text
                            PasswordClear = "hangfire_jobs"
                        }
                    }
                });

            var options = new DashboardOptions
            {
                AuthorizationFilters = new[] { filter }
            };

            app.UseHangfireDashboard("/jobs", options);
            app.UseHangfireServer();
        }
    }
}