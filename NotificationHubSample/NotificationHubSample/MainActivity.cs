﻿using Android.App;
using Android.OS;
using Android.Util;
using Android.Widget;
using Gcm.Client;
using Debug = System.Diagnostics.Debug;

namespace NotificationHubSample
{
    [Activity(Label = "NotificationHubSample", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            var button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };

            PushHandlerService.Initialize(this);
            PushHandlerService.Register(this);
        }

    }
}