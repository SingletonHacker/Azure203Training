using System;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Functions
{
    public static class ToTableStorage
    {
        private const string connectionString =
            "DefaultEndpointsProtocol=https;AccountName=bjornosstorageaccount;AccountKey=o2i2VBJ3z2zsezMW61SMNj3cbW61gnEuSFP9yrkqIgJnY97uwbpa6iB8vGVdAUdPGaZ/55+mZswmqb4qzE1imA==;EndpointSuffix=core.windows.net";

        [FunctionName("ToTable")]
        public static void Run([QueueTrigger("bjornosqueue", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            Console.WriteLine($"Triggered function with message: {myQueueItem}");

            // Table storage
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var client = storageAccount.CreateCloudTableClient();
            var table = client.GetTableReference("persons");

            table.CreateIfNotExists();

            TableOperation insertOperation = TableOperation.Insert(Person.FromString(myQueueItem));
            table.Execute(insertOperation);
        }
    }
}
