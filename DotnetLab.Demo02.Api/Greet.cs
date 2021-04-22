using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DotnetLab.Demo02.Api
{
    public static class Greet
    {
        [FunctionName("Greet")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var principal = StaticWebAppsAuth.Parse(req);
            log.LogInformation("C# HTTP trigger function processed a request.");

            var name = principal.Identity.IsAuthenticated ?
                principal.Identity.Name :
                "Anonymous";
            var responseMessage = $"Hello, {name}. This HTTP triggered function executed successfully.";
            return new OkObjectResult(responseMessage);
        }
    }
}

