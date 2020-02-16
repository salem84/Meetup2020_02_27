using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Food.API.Filters
{
    public class DelaySimulatorFilter : IActionFilter
    {
        public DelaySimulatorFilter(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var configRandomRate = Configuration.GetValue<double>("DelaySimulator:DelayRate");
            var randomValue = new Random().NextDouble();

            if (randomValue < configRandomRate)
            {
                var averageDelay = Configuration.GetValue<int>("DelaySimulator:DelayAverageMs");
                var jitter = averageDelay / 3;
                var delay = averageDelay + new Random().Next(-jitter, jitter);
                Thread.Sleep(delay);
            }
        }
    }
}
