using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Food.API.Filters
{

    public class ErrorSimulatorFilter : IActionFilter
    {
        public ErrorSimulatorFilter(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var configRandomRate = Configuration.GetValue<double>("ErrorSimulator:ErrorRate");
            var randomValue = new Random().NextDouble();

            if (randomValue < configRandomRate)
            {
                var statusCode = Configuration.GetValue<int>("ErrorSimulator:ErrorStatusCode"); ;
                context.Result = new StatusCodeResult(statusCode);
            }
        }
    }
}
