using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;

namespace MyWebFrontEnd
{
    [ApiController]
    [Route("api/[controller]")]
    public class Controller : ControllerBase
    {
        private const string connectionString =
           "DefaultEndpointsProtocol=https;AccountName=bjornosstorageaccount;AccountKey=o2i2VBJ3z2zsezMW61SMNj3cbW61gnEuSFP9yrkqIgJnY97uwbpa6iB8vGVdAUdPGaZ/55+mZswmqb4qzE1imA==;EndpointSuffix=core.windows.net";

        /// <summary>
        /// Send firstname lastname
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SetNameToQueue(string firstname, string lastname)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("bjornosqueue");

            await SendMessageAsync(queue, firstname + "" + lastname);

            return Accepted();
        }

        private static async Task SendMessageAsync(CloudQueue theQueue, string newMessage)
        {
            bool createdQueue = await theQueue.CreateIfNotExistsAsync();

            if (createdQueue)
            {
                Console.WriteLine("The queue was created.");
            }

            CloudQueueMessage message = new CloudQueueMessage(newMessage);
            await theQueue.AddMessageAsync(message);
        }
    }
}
