using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Notifications;

namespace NotificationHubSampleConsole
{
    public class Program
    {

        private static string sharedConnectionString = "<your full connection string>"; // i.e. Endpoint=sb://yourservice.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=N/v+LRzdXXXXXXXXlhF0xXXxX78PpzXEvRzXXXxXX00=
        private static string notificationHubPath = "<your hub path>";

        static void Main(string[] args)
        {
            SendNotificationAsync().Wait();
        }

       

        private static async Task SendNotificationAsync()
        {
            NotificationHubClient hub = NotificationHubClient.CreateClientFromConnectionString(sharedConnectionString, notificationHubPath);

            string key = "a";
            while (key!="x")
            {

                // This is just a sample of payload you can send
                // In this sample "customField2" is used to determine the activity to open (see MyBroadcastReceiver.cs)
                string payload = "";
                if (key=="d")
                {
                    payload = "{ \"data\" : {\"msg\":\"Hello from Azure!\", \"customField1\" : \"Hello World!\", \"customField2\":\"third\"}}";
                }
                else
                {
                    payload = "{ \"data\" : {\"msg\":\"Hello from Azure!\", \"customField1\" : \"Hello World!\", \"customField2\":\"any message\"}}";

                }
                await hub.SendGcmNativeNotificationAsync(payload);
                Console.WriteLine("Message sent");
                await Task.Delay(1000);
                Console.Clear();
                Console.WriteLine("Press [x] key to exit");
                Console.WriteLine("Press [d] key to open demo activity on app");
                Console.WriteLine("Press any other key just to send a push notification");
                key = Console.ReadLine();

            }

        }
    }


}
