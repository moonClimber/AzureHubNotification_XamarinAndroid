Azure NotificationHUB Sample
===================
This code extends the sample you can build by following the official Microsoft guide in [1] with the following additional features:

 - Receive remote push notifications even if app is not running
 - Open the app by click the notification and controls which activity open

I've tested this code on Asus Fonepad 6 (not on emulator).

Getting started
-------------
Before to execute the projects you have to obtain the following four values:

 - SenderID
 - ConnectionString Listen
 - ConnectionString Full
 - Path

You can obtain the first one by creating a project in Google Cloud Console (complete the "*Enable Google Cloud Messaging*" step in [1])
You can obtain the others by complete the "*Configure your Notification Hub*" step in [1])

Insert those values in Constants.cs file (in NotificationHubSample project) and in Program.cs file (in NotificationHubSampleConsole project)

Other details
-------------
Note that if you run the project in DEBUG mode from Visual Studio, notifications in status bar will disappear when you close the app and, moreover, your notifications won't be managed by the app. 
You have to run the app in RELEASE mode (or without debug) to publish the app on the device and to be able to manage push notification even if the app is not running.

The solution contains two projects:

 - **NotificationHubSample**: a Xamarin.Android project which install the app on the device and contains three simple activity, with very simple code
	 - **MainActivity**: is the MainLauncher (first activity executed) and is the activity which registers the notification service
	 - **NotificationViewActivity**: show the payload retrieved from push notification
	 - **ThirdActivity**: just to display another view
 - **NotificationHubSampleConsole**: this project simulates the server and throw push notifications to the app through Google Cloud Messaging Service and Azure HubNotification. It sends real remote messages, no local or simulations.

Environment
-------------
My development environment is:
 - Windows 8.1
 - Visual Studio 2013 Update 4
 - Xamarin 3.9.483.0
 - Xamarin.Android 4.20.0.37

I use following remote service:
 - Azure Notification Hub ([link](https://msdn.microsoft.com/en-us/library/azure/jj927170.aspx?f=255&MSPPError=-2147217396))
 - Google Cloud Messaging for Android ([link](https://developer.android.com/google/gcm/index.html))

> **Xamarin components:**
To run this solution install following components in **Components** folder in Xamarin.Android project (NuGet should not be able to restore them)
 > - Azure Messaging
 > - Azure Mobile Service
 > - Google Cloud Messaging Client


License
-------
MIT License, see the file LICENSE for more information.

----------
[1]: [Get started with Notification Hubs](http://azure.microsoft.com/en-us/documentation/articles/partner-xamarin-notification-hubs-android-get-started/)
[2]: [An Overview of Remote Notifications in Xamarin.Android](http://developer.xamarin.com/guides/cross-platform/application_fundamentals/notifications/android/remote_notifications_in_android/)