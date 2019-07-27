using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace FlightDataFunctionApp
{
   /// <summary>
   /// This Azure Function provides SignalR connection info to the client
   /// </summary>
   public static class SignalRInfo
   {
      [FunctionName("Negotiate")]
      public static IActionResult Run(
         [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
         [SignalRConnectionInfo(HubName = "flightdata")] SignalRConnectionInfo connectionInfo,
         ILogger log)
      {
         return new OkObjectResult(connectionInfo);
      }
   }
}