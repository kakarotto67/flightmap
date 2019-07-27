using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FlightDataFunctionApp
{
   /// <summary>
   /// This Azure Functions gets data from OpenSky API every 5 seconds and adds them to CosmosDB
   /// </summary>
   public static class FlightDataPoll
   {
      private static readonly HttpClient client = new HttpClient();

      [FunctionName("FlightDataPoll")]
      public static async Task RunAsync([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer,
          [CosmosDB(
            databaseName: "flightmap-db",
            collectionName: "flightmap-container",
            ConnectionStringSetting = "AzureCosmosDBConnection")]IAsyncCollector<Flight> documents,
         ILogger log)
      {
         log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");

         var openSkyUrl = "https://opensky-network.org/api/states/all?lamin=44.00&lomin=22.00&lamax=52.00&lomax=40.00";

         using (var getApiResult = await client.GetAsync(openSkyUrl))
         {
            using (var content = getApiResult.Content)
            {
               var result = JsonConvert.DeserializeObject<Rootobject>(await content.ReadAsStringAsync());
               foreach (var item in result.states)
               {
                  await documents.AddAsync(Flight.CreateFromData(item));
               }

               log.LogInformation($"Total flights processed{result.states.Length}");
            }
         }
      }
   }
}