using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Queue;

namespace MyWebFrontEnd.Pages
{
    public class IndexModel : PageModel
    {
        private const string connectionString =
            "DefaultEndpointsProtocol=https;AccountName=bjornosstorageaccount;AccountKey=o2i2VBJ3z2zsezMW61SMNj3cbW61gnEuSFP9yrkqIgJnY97uwbpa6iB8vGVdAUdPGaZ/55+mZswmqb4qzE1imA==;EndpointSuffix=core.windows.net";

        [HttpPost]
        public async Task OnPostAsync(string firstname, string lastname, IFormFile file)
        {
            // do something with emailAddress
            var firstname1 = firstname;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            var sbc = storageAccount.CreateCloudBlobClient();
            var cbc = sbc.GetContainerReference("samples-images");
            var cbc2 = sbc.GetContainerReference("sample-images-sm");
            cbc2.CreateIfNotExists();

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("bjornosqueue");

            await PublishToQueueAsync(queue, firstname + "" + lastname);
            await PublishToBlobStorage(cbc, $"{firstname}_{lastname}.png", file);
        }

        private static async Task PublishToQueueAsync(CloudQueue theQueue, string newMessage)
        {
            bool createdQueue = await theQueue.CreateIfNotExistsAsync();

            if (createdQueue)
            {
                Console.WriteLine("The queue was created.");
            }

            CloudQueueMessage message = new CloudQueueMessage(newMessage);
            await theQueue.AddMessageAsync(message);
        }

        private static async Task PublishToBlobStorage(CloudBlobContainer container, string fileName, IFormFile file)
        {
            await container.CreateIfNotExistsAsync();

            Console.WriteLine("Publishing img to blob storage");

            var photo = container.GetBlockBlobReference(fileName);
            photo.UploadFromStream(file.OpenReadStream());
        }
    }
}
