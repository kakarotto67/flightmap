using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace FlightDataFunctionApp
{
   /// <summary>
   /// This Azure Function is triggered whenever CosmosDB is modified
   /// and pushes the input value via SignalR to client's method
   /// </summary>
   public static class FlightDataChangeFeed
   {
      [FunctionName("FlightDataChangeFeed")]
      public static async Task RunAsync([CosmosDBTrigger(
            databaseName: "flightmap-db",
            collectionName: "flightmap-container",
            ConnectionStringSetting = "AzureCosmosDBConnection",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input,
         [SignalR(HubName = "flightmap")] IAsyncCollector<SignalRMessage> signalRMessages,
         ILogger log)
      {
         if (input != null && input.Count > 0)
         {
            log.LogInformation("Documents modified " + input.Count);
            //log.LogInformation("First document Id " + input[0].Id);

            // Push SignalR messages to client's method newFlightData() with flight information
            foreach (var flight in input)
            {
               await signalRMessages.AddAsync(new SignalRMessage
               {
                  Target = "newFlightData",
                  Arguments = new[] { flight }
               });

               log.LogInformation($"flight added: {flight}");
            }
         }
      }
   }
}