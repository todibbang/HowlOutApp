using System;
using System.Collections.Generic;
using System.Linq;
using WindowsAzure.Messaging;
using Facebook.CoreKit;
using Facebook.LoginKit;
using Foundation;
using UIKit;
using ImageCircle.Forms.Plugin.iOS;
using HowlOut;

namespace HowlOut.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		private SBNotificationHub Hub { get; set; }

		public static int width;
		public static int height;

		public const string ConnectionString = "Endpoint=sb://howlout.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=S56W4coW1bGmVTdirY59X7poLwlxZjWkEcYOMmyAezI=";
		public const string NotificationHubPath = "HowloutNotificationHub";

		string appId = "651141215029165";
		string appName = "HowlOut";

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init ();

			if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
					   UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
					   new NSSet());

				UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
				UIApplication.SharedApplication.RegisterForRemoteNotifications();
			}
			else {
				UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
			}

			height = (int) UIScreen.MainScreen.Bounds.Height;
			width = (int) UIScreen.MainScreen.Bounds.Width;

			Facebook.CoreKit.Profile.EnableUpdatesOnAccessTokenChange(true);
			Settings.AppID = appId;
			Settings.DisplayName = appName;


			ImageCircleRenderer.Init ();
			LoadApplication(new App());

			return base.FinishedLaunching (app, options);
			//return ApplicationDelegate.SharedInstance.FinishedLaunching(app, options);
		}


		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			Hub = new SBNotificationHub(ConnectionString, NotificationHubPath);
			App.StoredNotificationToken.DeviceToken = deviceToken.ToString();
		}


		public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
		{
			ProcessNotification(userInfo, false);

			NSObject Type;
			NSObject Id;

			var success = userInfo.TryGetValue(new NSString("type"), out Type);
			App.UpdateLiveConversations();
			if (success)
			{
				success = userInfo.TryGetValue(new NSString("id"), out Id);
				if (success)
				{
					System.Diagnostics.Debug.WriteLine(Type.ToString() + ", " + Id.ToString());
					if (Type.ToString() == "conversation" || Type.ToString() == "comment")
					{
						
					}
				}
			}
		}

		void ProcessNotification(NSDictionary options, bool fromFinishedLaunching)
		{
			// Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
			if (null != options && options.ContainsKey(new NSString("aps")))
			{
				//Get the aps dictionary
				NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;

				string alert = string.Empty;

				//Extract the alert text
				// NOTE: If you're using the simple alert by just specifying
				// "  aps:{alert:"alert msg here"}  ", this will work fine.
				// But if you're using a complex alert with Localization keys, etc.,
				// your "alert" object from the aps dictionary will be another NSDictionary.
				// Basically the JSON gets dumped right into a NSDictionary,
				// so keep that in mind.
				if (aps.ContainsKey(new NSString("alert")))
					alert = (aps[new NSString("alert")] as NSString).ToString();

				//If this came from the ReceivedRemoteNotification while the app was running,
				// we of course need to manually process things like the sound, badge, and alert.
				if (!fromFinishedLaunching)
				{
					//Manually show an alert
					if (!string.IsNullOrEmpty(alert))
					{
						UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
						avAlert.Show();
					}
				}
			}
		}

		public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			return ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, annotation);
		}
	}
}

