using Android.App;
using Android.OS;
using Android.Widget;

namespace NotificationHubSample
{
    [Activity(Label = "NotificationViewActivity")]
    public class NotificationViewActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Notification);
            // Create your application here

            string title = Intent.GetStringExtra("title");
            string desc = Intent.GetStringExtra("desc");

            var txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
            var txtDesc = FindViewById<TextView>(Resource.Id.txtDesc);

            txtTitle.Text = title;
            txtDesc.Text = desc;
        }
    }
}