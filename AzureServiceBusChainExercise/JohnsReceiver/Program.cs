using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;

namespace JohnsReceiver
{
    internal class Program
    {
        private const string TopicName = "bjorn";

        private const string SubscriptionName = "sub";

        private static string cs = "Endpoint=sb://johngortersb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SZVMbLVfpNT9jM4AVhw4sRTb5rbxeXEvduT3GSciCK4=;TransportType=AmqpWebSockets";

        private static ISubscriptionClient subscriptionClient;

        private static void Main(string[] args)

        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()

        {
            ManagementClient management = new ManagementClient(cs);

            if (!await management.TopicExistsAsync("bjorn"))

                await management.CreateTopicAsync("bjorn");

            if (!await management.TopicExistsAsync("mirhan"))

                await management.CreateTopicAsync("mirhan");

            if (!await management.SubscriptionExistsAsync("bjorn", "sub"))

                await management.CreateSubscriptionAsync("bjorn", "sub");

            subscriptionClient = new SubscriptionClient(cs, TopicName, SubscriptionName);

            Console.WriteLine("======================================================");

            Console.WriteLine("Press ENTER key to exit after receiving all the messages.");

            Console.WriteLine("======================================================");

            // Register subscription message handler and receive messages in a loop.

            RegisterOnMessageHandlerAndReceiveMessages();

            Console.ReadKey();

            await subscriptionClient.CloseAsync();
        }

        private static void RegisterOnMessageHandlerAndReceiveMessages()

        {
            // Configure the message handler options in terms of exception handling, number of
            // concurrent messages to deliver, etc.

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)

            {
                MaxConcurrentCalls = 1,

                AutoComplete = false
            };

            // Register the function that processes messages.

            Console.WriteLine("registering the handler");

            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            Console.WriteLine("done");
        }

        private static async Task ProcessMessagesAsync(Message message, CancellationToken token)

        {
            // Process the message.

            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)

        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");

            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            Console.WriteLine("Exception context for troubleshooting:");

            Console.WriteLine($"- Endpoint: {context.Endpoint}");

            Console.WriteLine($"- Entity Path: {context.EntityPath}");

            Console.WriteLine($"- Executing Action: {context.Action}");

            return Task.CompletedTask;
        }
    }
}
