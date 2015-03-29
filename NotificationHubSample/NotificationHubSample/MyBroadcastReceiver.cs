using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using WindowsAzure.Messaging;
using Android.App;
using Android.Content;
using Android.Util;
using Gcm.Client;

[assembly: Permission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "@PACKAGE_NAME@.permission.C2D_MESSAGE")]
[assembly: UsesPermission(Name = "com.google.android.c2dm.permission.RECEIVE")]

//GET_ACCOUNTS is only needed for android versions 4.0.3 and below

[assembly: UsesPermission(Name = "android.permission.GET_ACCOUNTS")]
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
[assembly: UsesPermission(Name = "android.permission.WAKE_LOCK")]

namespace NotificationHubSample
{
    /// <summary>
    ///     This class is responsible to manage the notifications.
    /// </summary>
    /// <remarks>
    ///     Very important: you have to launch in "Start Without Debugging"
    ///     This code starts from the Official Microsoft's sample (first reference) and evolves with notes in Xamarin Android
    ///     Remote notifications sample (second reference)
    ///     Moreover, I've added also some trivial code to demonstrate how to open a specific activity and how to pass
    ///     parameters from push notification to activity
    /// </remarks>
    /// <see
    ///     cref="http://azure.microsoft.com/en-us/documentation/articles/partner-xamarin-notification-hubs-android-get-started/" />
    /// <see
    ///     cref="http://developer.xamarin.com/guides/cross-platform/application_fundamentals/notifications/android/remote_notifications_in_android/" />
    [BroadcastReceiver(Permission = Gcm.Client.Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new[] {Gcm.Client.Constants.INTENT_FROM_GCM_MESSAGE},
        Categories = new[] {"@PACKAGE_NAME@"})]
    [IntentFilter(new[] {Gcm.Client.Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK},
        Categories = new[] {"@PACKAGE_NAME@"})]
    [IntentFilter(new[] {Gcm.Client.Constants.INTENT_FROM_GCM_LIBRARY_RETRY},
        Categories = new[] {"@PACKAGE_NAME@"})]
    [IntentFilter(new[] {Intent.ActionBootCompleted})]
    // This attribute is to receive the notification even if the app is not running
    public class MyBroadcastReceiver : GcmBroadcastReceiverBase<PushHandlerService>
    {
        public const string LOG_CATEGORY = "NotificationHubSample-LOG";
        public static string[] SENDER_IDS = {Constants.SenderID};
    }

    [Service] // Must use the service tag
    public class PushHandlerService : GcmServiceBase
    {
        public static string RegistrationID { get; private set; }
        static NotificationHub Hub;

        public static void Initialize(Context context)
        {
            // Call this from our main activity
            Hub = new NotificationHub(Constants.NotificationHubPath, Constants.ConnectionString, context);
        }

        public static void Register(Context context)
        {
            // Makes this easier to call from our Activity
            GcmClient.Register(context, Constants.SenderID);
        }

        protected override void OnError(Context context, string errorId)
        {
            Log.Info(MyBroadcastReceiver.LOG_CATEGORY, "OnError");
        }

        protected override void OnRegistered(Context context, string registrationId)
        {
            Log.Info(MyBroadcastReceiver.LOG_CATEGORY, "OnRegistered");

            //Receive registration Id for sending GCM Push Notifications to
            var tags = new List<string> { "" }; // tags are just "filters"
            if (Hub != null)
                Hub.Register(registrationId, tags.ToArray());
        }

        protected override void OnUnRegistered(Context context, string registrationId)
        {
            Log.Info(MyBroadcastReceiver.LOG_CATEGORY, "OnUnRegistered");


            if (Hub != null)
                Hub.Unregister();
        }

        protected override void OnMessage(Context context, Intent intent)
        {
            Log.Info(MyBroadcastReceiver.LOG_CATEGORY, "OnMessage");
            if (intent != null)
            {
                HandleMessage(intent);
            }
            else
            {
                Log.Info(MyBroadcastReceiver.LOG_CATEGORY, "OnMessage::no intent");
            }
        }

        private void HandleMessage(Intent intent)
        {
            // Custom logic... apply whatever you want
            // This logic happens when a push message is received

            // In this case, following statemens "transform" a push message into a notification message
            // In particular, through BuildNotificationIntent this method creates an interactive notification message
            // in other words, when user tap the notification, it activates an Intent and open an Activity
            string action = intent.Action;
            var msg = new StringBuilder();
            string myFld = null;
            if (intent.Extras != null)
            {
                foreach (string key in intent.Extras.KeySet())
                    msg.AppendLine(key + "=" + intent.Extras.Get(key));

                msg.AppendLine(intent.Extras.GetString("msg"));
                myFld = intent.Extras.GetString("customField2");
            }

            msg.AppendLine(!string.IsNullOrEmpty(action)
                ? string.Format("Intent action:{0}", action)
                : "Intent action:undefined");
            BuildNotificationIntent("OnMessage#3", msg.ToString(), myFld == "third");
            Log.Info(MyBroadcastReceiver.LOG_CATEGORY, "OnMessage#3:{0}", msg.ToString());
        }

        private void HandleRegistration(Intent intent)
        {
            Log.Info(MyBroadcastReceiver.LOG_CATEGORY, "REGISTRATION");
        }


        /// <summary>
        /// OnHandleIntent is just an higher level alternative to OnError, OnMessage, OnRegistered and OnUnRegistered
        /// 
        /// </summary>
        /// <param name="intent"></param>
        protected override void OnHandleIntent(Intent intent)
        {
            Log.Info(MyBroadcastReceiver.LOG_CATEGORY, "OnHandleIntent");
            if (intent != null)
            {
                string action = intent.Action;
                // Here you can put your custom logic
                if (action.Equals("com.google.android.c2dm.intent.REGISTRATION"))
                {
                    HandleRegistration(intent);
                }
                else if (action.Equals("com.google.android.c2dm.intent.MESSAGE"))
                {
                    HandleMessage(intent);
                }
                else
                {
                    Log.Info(MyBroadcastReceiver.LOG_CATEGORY, "OnHandleIntent::{0}", action);
                }
            }
            else
            {
                Log.Info(MyBroadcastReceiver.LOG_CATEGORY, "OnHandleIntent::no intent");
            }
            base.OnHandleIntent(intent);
        }

        private void BuildNotificationIntent(string title, string desc, bool openThird = false)
        {
            var notificationManager = GetSystemService(NotificationService) as NotificationManager;
            var notification = new Notification(Android.Resource.Drawable.SymActionEmail, title);
            notification.Flags = NotificationFlags.AutoCancel;
            Intent resultIntent = null;

            // The following statements control which activity open when user presses the notification
            if (openThird)
            {
                resultIntent = new Intent(this, typeof(ThirdActivity));
            }
            else
            {
                resultIntent = new Intent(this, typeof(NotificationViewActivity));
            }

            resultIntent.PutExtra("title", title);
            resultIntent.PutExtra("desc", desc);

            PendingIntent resultPendingIntent = PendingIntent.GetActivity(this, 1, resultIntent,
                PendingIntentFlags.OneShot);

            notification.SetLatestEventInfo(this, title, desc, resultPendingIntent);

            notificationManager.Notify(1, notification);
        }

    }
}