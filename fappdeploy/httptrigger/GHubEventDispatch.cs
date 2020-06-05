using System;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

public static class GHubEventDispatch{
    [FunctionName("DispatchEvent")]
    public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req, ILogger log, ExecutionContext context)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");
        
        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Awesome-Octocat-App");
            httpClient.DefaultRequestHeaders.Accept.Clear();

            var PATTOKEN =  Environment.GetEnvironmentVariable("PATTOKEN", EnvironmentVariableTarget.Process);
            log.LogInformation($"PATTOKEN: {PATTOKEN}");

            httpClient.DefaultRequestHeaders.Add("Authorization", PATTOKEN);


            var client_payload = new Newtonsoft.Json.Linq.JObject { ["unit "] = false, ["integration"] = true, ["github_SHA"] = "7a6fe10d22b5c44be55698f6d123c6480451e18b"};

            var payload = Newtonsoft.Json.JsonConvert.SerializeObject(new Newtonsoft.Json.Linq.JObject { ["event_type"] = "deploy-command", ["client_payload"] = client_payload });
            
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync("https://api.github.com/repos/cs1170353/test_mlops/dispatches", content);
            var resultString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(resultString);
            return (ActionResult)new OkObjectResult(resultString);
        }
        
        return (ActionResult)new OkObjectResult($"Hello");
    }
}
