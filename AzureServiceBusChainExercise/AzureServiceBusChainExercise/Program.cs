using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Management;

namespace AzureServiceBusChainExercise
{
    internal class Program
    {
        private const string ServiceBusConnectionString = "Endpoint=sb://johngortersb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SZVMbLVfpNT9jM4AVhw4sRTb5rbxeXEvduT3GSciCK4=;TransportType=AmqpWebSockets";
        private const string ReceiveTopicName = "bjorn";
        private const string SubName = "sub";

        private const string SendTopicName = "paulberndsen";

        private static async Task Main(string[] args)
        {
            var bjornoSender = new Sender(ServiceBusConnectionString, ReceiveTopicName);

            var sender = new Sender(ServiceBusConnectionString, SendTopicName);
            var receiver = new Receiver(sender, ReceiveTopicName, ServiceBusConnectionString, SubName);

            Console.WriteLine("======================================================");
            Console.WriteLine("Press ENTER key to exit after receiving all the messages.");
            Console.WriteLine("======================================================");

            await SetupTopicsAndSubs();

            Console.WriteLine("Press S to send message or any other key to continue");

            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.S)
            {
                await bjornoSender.Send();
                Console.WriteLine("Send completed");
            }

            Console.WriteLine("waiting for message");
            await receiver.Receive();
        }

        private static async Task SetupTopicsAndSubs()
        {
            var mc = new ManagementClient(ServiceBusConnectionString);

            if (!await mc.TopicExistsAsync(ReceiveTopicName))
            {
                Console.WriteLine("Receive topic bestaat niet, aanmaken");
                await mc.CreateTopicAsync(ReceiveTopicName);
                Console.WriteLine("Receive topic aangemaakt");
            }
            else
            {
                Console.WriteLine("Receive topic bestaat al");
            }

            if (!await mc.TopicExistsAsync(SendTopicName))
            {
                Console.WriteLine("Send topic bestaat niet, aanmaken");
                await mc.CreateTopicAsync(SendTopicName);
                Console.WriteLine("Send topic aangemaakt");
            }
            else
            {
                Console.WriteLine("Send topic bestaat al");
            }

            if (!await mc.SubscriptionExistsAsync(ReceiveTopicName, SubName))
            {
                Console.WriteLine("Subsription bestaat niet, aanmaken");

                await mc.CreateSubscriptionAsync(ReceiveTopicName, SubName);
            }
            else
            {
                Console.WriteLine("Subsription bestaat al");
            }
        }
    }
}
