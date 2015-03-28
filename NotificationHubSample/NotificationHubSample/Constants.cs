namespace NotificationHubSample
{
    public class Constants
    {
        // The sample values are not real. Follow the guide in [http://azure.microsoft.com/en-us/documentation/articles/partner-xamarin-notification-hubs-android-get-started/] to create them
        public const string SenderID = "<your Google API Project Number>"; // i.e. 1093199957993

        // Azure app specific connection string and hub path
        public const string ConnectionString = "<your listen connection string>";
        // i.e. Endpoint=sb://yourservice.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=nXXXXX9eMcCSdXXXqXdIsBm2XQXXXXrpvgi+7XXX="

        public const string NotificationHubPath = "<NotificationHub name>";
    }
}