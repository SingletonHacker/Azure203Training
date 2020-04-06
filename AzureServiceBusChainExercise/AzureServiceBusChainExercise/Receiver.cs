using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace AzureServiceBusChainExercise
{
    internal class Receiver
    {
        private readonly string TopicName;
        private readonly string SubscriptionName;
        private readonly Sender _sender;
        private readonly string ServiceBusConnectionString;
        private ISubscriptionClient subscriptionClient;

        public Receiver(Sender sender, string topicName, string connectionString, string subscriptionName)
        {
            _sender = sender;
            TopicName = topicName;
            ServiceBusConnectionString = connectionString;
            SubscriptionName = subscriptionName;
        }

        public async Task Receive()
        {
            subscriptionClient = new SubscriptionClient(ServiceBusConnectionString, TopicName, SubscriptionName);

            // Register subscription message handler and receive messages in a loop
            RegisterOnMessageHandlerAndReceiveMessages();

            Console.WriteLine("setup receive, press key to stop receiving");
            Console.ReadKey();

            await subscriptionClient.CloseAsync();
        }

        // Use this handler to examine the exceptions received on the message pump.
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

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message.
            var body = Encoding.UTF8.GetString(message.Body);

            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{body}");

            // Complete the message so that it is not received again. This can be done only if the
            // subscriptionClient is created in ReceiveMode.PeekLock mode (which is the default).
            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);

            var newBody = $"Bjorn touched this: {body}";

            await _sender.Reply(newBody);

            // Note: Use the cancellationToken passed as necessary to determine if the
            // subscriptionClient has already been closed. If subscriptionClient has already been
            // closed, you can choose to not call CompleteAsync() or AbandonAsync() etc. to avoid
            // unnecessary exceptions.
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the message handler options in terms of exception handling, number of
            // concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to
                // 1 for simplicity. Set it according to how many messages the application wants to
                // process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether the message pump should automatically complete the messages
                // after returning from user callback. False below indicates the complete operation
                // is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false
            };

            // Register the function that processes messages.
            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }
    }
}
