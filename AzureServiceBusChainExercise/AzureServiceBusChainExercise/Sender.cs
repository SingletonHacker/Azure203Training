using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace AzureServiceBusChainExercise
{
    internal class Sender
    {
        private readonly string TopicName;
        private readonly string ServiceBusConnectionString;
        private ITopicClient topicClient;

        public Sender(string connectionString, string topicName)
        {
            ServiceBusConnectionString = connectionString;
            TopicName = topicName;
        }

        public async Task Send()
        {
            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            const int numberOfMessages = 10;
            for (var i = 0; i < numberOfMessages; i++)
            {
                // Send messages.
                await SendMessagesAsync($"Message: Bjorn was here{i}");
            }

            await topicClient.CloseAsync();
        }

        public async Task Reply(string messageBody)
        {
            topicClient = new TopicClient(ServiceBusConnectionString, TopicName);

            await SendMessagesAsync(messageBody);

            await topicClient.CloseAsync();
        }

        private async Task SendMessagesAsync(string messageBody)
        {
            try
            {
                // Create a new message to send to the topic.
                var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                // Write the body of the message to the console.
                Console.WriteLine($"Sending message: {messageBody}");

                // Send the message to the topic.
                await topicClient.SendAsync(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}
