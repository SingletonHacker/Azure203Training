using System;
using System.IO;
using Microsoft.Azure.WebJobs;

namespace MyAzureWebJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written on an Azure Queue
        // called queue.
        public static void ProcessQueueMessage([QueueTrigger("bjornosqueue")] string message, TextWriter log)
        {
            Console.WriteLine($"Incoming message Bjorno:{message}");
            log.WriteLine(message);

            //TableOperations to = TableOperations.Insert()//        }
        }
    }
}
