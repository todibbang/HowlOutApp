using System;
using System.Collections.Generic;
using System.Linq;
using WindowsAzure.Messaging;
using Facebook.CoreKit;
using Facebook.LoginKit;
using Foundation;
using UIKit;
using ImageCircle.Forms.Plugin.iOS;
//using CarouselView.FormsPlugin.iOS;
using HowlOut;
using UserNotifications;

namespace HowlOut.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		private SBNotificationHub Hub { get; set; }

		public static int width;
		public static int height;

		public bool notiCheck = true;

		public const string ConnectionString = "Endpoint=sb://howlout.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=S56W4coW1bGmVTdirY59X7poLwlxZjWkEcYOMmyAezI=";
		public const string NotificationHubPath = "HowloutNotificationHub";

		string appId = "651141215029165";
		string appName = "HowlOut";

		SocialControllerRenderer socia = new SocialControllerRenderer();

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();


			if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
			{
				UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) =>
				{

				});
				var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
					   UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
					   new NSSet());

				UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
				UIApplication.SharedApplication.RegisterForRemoteNotifications();
			}
			else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
					   UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
					   new NSSet());

				UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
				UIApplication.SharedApplication.RegisterForRemoteNotifications();
			}
			else
			{
				UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
			}

			height = (int)UIScreen.MainScreen.Bounds.Height;
			width = (int)UIScreen.MainScreen.Bounds.Width;

			Facebook.CoreKit.Profile.EnableUpdatesOnAccessTokenChange(true);
			Settings.AppID = appId;
			Settings.DisplayName = appName;



			//CarouselViewRenderer.Init();
			ImageCircleRenderer.Init();
			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
			//return ApplicationDelegate.SharedInstance.FinishedLaunching(app, options);
		}

		public override void OnActivated(UIApplication application)
		{
			Console.WriteLine("OnActivated called, App is active.");
			setNotiCheck(false);
		}
		public override void DidEnterBackground(UIApplication application)
		{
			Console.WriteLine("App entering background state.");
			setNotiCheck(true);
		}

		private async void setNotiCheck(bool check)
		{
			await System.Threading.Tasks.Task.Delay(4000);
			notiCheck = check;
		}



		public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
		{
			Hub = new SBNotificationHub(ConnectionString, NotificationHubPath);
			App.StoredNotificationToken.DeviceToken = deviceToken.ToString();
		}


		public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
		{
			ProcessNotification(userInfo, notiCheck);


			socia.setNotificationBadge(1);

			NSObject Type;
			NSObject Id;

			var success = userInfo.TryGetValue(new NSString("type"), out Type);
			if (success)
			{
				success = userInfo.TryGetValue(new NSString("id"), out Id);
				NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;
				string alert = string.Empty;
				if (aps.ContainsKey(new NSString("alert")))
					alert = (aps[new NSString("alert")] as NSString).ToString();
				if (success)
				{
					App.notificationController.HandlePushNotification(Type.ToString(), Id.ToString(), notiCheck, alert);
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
						//UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
						//avAlert.Show();
					}
				}
			}
		}

		public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
		{
			System.Diagnostics.Debug.WriteLine("ContinueUserActivity method has been called...");
			System.Diagnostics.Debug.WriteLine(userActivity.WebPageUrl);

			string Content = userActivity.WebPageUrl.ToString();

			if (Content.Contains("event"))
			{
				int startIndex = Content.IndexOf("event") + "event".Length;
				System.Diagnostics.Debug.WriteLine(Content.Substring(startIndex));
				App.coreView.GoToSelectedEvent(Content.Substring(startIndex));
			}

			return true;
		}

		public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			return ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, annotation);
		}

		public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
		{
			UIApplication.SharedApplication.ApplicationIconBadgeNumber++;

			// show an alert
			/*
			UIAlertController okayAlertController = UIAlertController.Create(notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.Alert);
			okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
			viewController.PresentViewController(okayAlertController, true, null);

			// reset our badge
			UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0; */
		}
	}
}


