using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Queue;
using CloudStorageAccount = Microsoft.Azure.Storage.CloudStorageAccount;
using CosmosCloudStorageAccount = Microsoft.Azure.Cosmos.Table.CloudStorageAccount;

namespace MyWebFrontEnd.Pages
{
    public class IndexModel : PageModel
    {
        private const string connectionString =
            "DefaultEndpointsProtocol=https;AccountName=bjornosstorageaccount;AccountKey=o2i2VBJ3z2zsezMW61SMNj3cbW61gnEuSFP9yrkqIgJnY97uwbpa6iB8vGVdAUdPGaZ/55+mZswmqb4qzE1imA==;EndpointSuffix=core.windows.net";

        public List<Person> Persons { get; set; } = new List<Person>();

        [HttpPost]
        public async Task OnPostAsync(string firstname, string lastname, IFormFile file)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            var sbc = storageAccount.CreateCloudBlobClient();

            //Blob Containers
            var writeContainer = sbc.GetContainerReference("samples-images");
            var readContainer = sbc.GetContainerReference("sample-images-sm");
            await writeContainer.CreateIfNotExistsAsync();
            await readContainer.CreateIfNotExistsAsync();

            // Queue
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("bjornosqueue");

            await PublishToQueueAsync(queue, new Person(firstname, lastname));
            PublishToBlobStorage(writeContainer, $"{firstname}_{lastname}.png", file);

            // De person die net is gepost zit nog in de queue deze moet nog door de azure function
            // naar table gestorage gezet worden Oftewel wij lopen hier in principe 1 post achter En
            // dat is prima voor test doeleinden
            var selectAllQuery = new TableQuery<Person>();

            var account = CosmosCloudStorageAccount.Parse(connectionString);
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("persons");

            Persons = table.ExecuteQuery(selectAllQuery).ToList();
        }

        private static async Task PublishToQueueAsync(CloudQueue theQueue, Person person)
        {
            bool createdQueue = await theQueue.CreateIfNotExistsAsync();

            if (createdQueue)
            {
                Console.WriteLine("The queue was created.");
            }

            CloudQueueMessage message = new CloudQueueMessage(person.ToString());
            await theQueue.AddMessageAsync(message);
        }

        private static void PublishToBlobStorage(CloudBlobContainer container, string fileName, IFormFile file)
        {
            Console.WriteLine("Publishing img to blob storage");

            var photo = container.GetBlockBlobReference(fileName);
            photo.UploadFromStream(file.OpenReadStream());
        }
    }
}
