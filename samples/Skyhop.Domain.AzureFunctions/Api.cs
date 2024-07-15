using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Skyhop.Domain.AzureFunctions;

public class Api
{
    [Function("Test")]
    public HttpResponseData Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post")] 
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

        response.WriteString("Welcome to Azure Functions!");

        return response;        
    }
}