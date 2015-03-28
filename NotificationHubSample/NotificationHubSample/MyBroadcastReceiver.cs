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
    ///     Very important: it doesn't work if Visual Studio is in DEBUG mode (I waste a lot of time for this reason, so, pay
    ///     attention to this)
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
        public const string TAG = "MyBroadcastReceiver-GCM";
        public static string[] SENDER_IDS = {Constants.SenderID};
    }

    [Service] // Must use the service tag
    public class PushHandlerService : GcmServiceBase
    {
        public static string RegistrationID { get; private set; }
        private NotificationHub Hub { get; set; }

        protected override void OnMessage(Context context, Intent intent)
        {
            Log.Info(MyBroadcastReceiver.TAG, "GCM Message Received!");
        }

        protected override void OnError(Context context, string errorId)
        {
            throw new NotImplementedException();
        }

        protected override void OnRegistered(Context context, string registrationId)
        {
            Log.Verbose(MyBroadcastReceiver.TAG, "GCM Registered: " + registrationId);
            RegistrationID = registrationId;

            //CreateNotification("PushHandlerService-GCM Registered...", "The device has been Registered, Tap to View!");

            Hub = new NotificationHub(Constants.NotificationHubPath, Constants.ConnectionString, context);
            try
            {
                Hub.UnregisterAll(registrationId);
            }
            catch (Exception ex)
            {
                Log.Error(MyBroadcastReceiver.TAG, "GCM Exception: " + ex.Message);
                Debugger.Break();
            }

            var tags = new List<string> {""}; // create tags if you want

            try
            {
                Registration hubRegistration = Hub.Register(registrationId, tags.ToArray());
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }
        }

        protected override void OnUnRegistered(Context context, string registrationId)
        {
            throw new NotImplementedException();
        }

        protected override void OnHandleIntent(Intent intent)
        {
            if (intent != null)
            {
                string myFld = null;
                string action = intent.Action;
                if (!action.Equals("com.google.android.c2dm.intent.REGISTRATION"))
                {
                    var msg = new StringBuilder();

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
                    BuildNotificationIntent("OnHandleIntent:RECEIVE", msg.ToString(), myFld == "third");
                }
            }
            else
            {
                BuildNotificationIntent("OnHandleIntent", "No intent");
            }
        }


        /// <summary>
        ///     Creates the notification
        /// </summary>
        /// <param name="title"></param>
        /// <param name="desc"></param>
        /// <param name="openThird">Just to test the Activity selection criteria</param>
        private void BuildNotificationIntent(string title, string desc, bool openThird = false)
        {
            var notificationManager = GetSystemService(NotificationService) as NotificationManager;
            var notification = new Notification(Android.Resource.Drawable.SymActionEmail, title);
            notification.Flags = NotificationFlags.AutoCancel;
            Intent resultIntent = null;

            // The following statements control which activity open when user presses the notification
            if (openThird)
            {
                resultIntent = new Intent(this, typeof (ThirdActivity));
            }
            else
            {
                resultIntent = new Intent(this, typeof (NotificationViewActivity));
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