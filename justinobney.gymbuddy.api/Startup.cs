using System.Collections.Generic;
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
            
            var options = new DashboardOptions
            {
                AuthorizationFilters = new[] { new MyNonRestrictiveAuthorizationFilter() }
            };

            app.UseHangfireDashboard("/jobs", options);
            app.UseHangfireServer();
        }
    }

    public class MyNonRestrictiveAuthorizationFilter : IAuthorizationFilter
    {
        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            return true;
        }
    }
}