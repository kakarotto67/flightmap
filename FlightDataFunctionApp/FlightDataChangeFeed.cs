using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FlightDataFunctionApp
{
   /// <summary>
   /// This Azure Function is triggered whenever CosmosDB is modified
   /// </summary>
   public static class FlightDataChangeFeed
   {
      [FunctionName("FlightDataChangeFeed")]
      public static void Run([CosmosDBTrigger(
            databaseName: "flightmap-db",
            collectionName: "flightmap-container",
            ConnectionStringSetting = "AzureCosmosDBConnection",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, ILogger log)
      {
         if (input != null && input.Count > 0)
         {
            log.LogInformation("Documents modified " + input.Count);
            log.LogInformation("First document Id " + input[0].Id);
         }
      }
   }
}