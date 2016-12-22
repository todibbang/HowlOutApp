using System;
using EventKit;
using System.Threading.Tasks;
using Xamarin.Forms;
using Facebook.CoreKit;
using System.Collections.Generic;
using UIKit;
using HowlOut.iOS;
using Foundation;

using Facebook.LoginKit;
using Facebook.ShareKit;

[assembly: Dependency(typeof(SocialControllerRenderer))]

namespace HowlOut.iOS
{
	public class SocialControllerRenderer : SocialController
	{
		EKEventStore eventStore;
		List<string> readPermissions = new List<string> { "public_profile", "user_events", "read_custom_friendlists", "user_friends" };

		public async Task<List<string>> getFacebookFriends()
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			List<string> friendsIDs = new List<string>();

			var request = new GraphRequest("/"+App.StoredUserFacebookId+"/friends", null, App.StoredToken, null, "GET");
			var requestConnection = new GraphRequestConnection();

			var fbEvents = new List<FaceBookEvent>();

			requestConnection.AddRequest(request, (connection, result, error) =>
			{
				if (error != null)
				{
					System.Diagnostics.Debug.WriteLine("Hnnn2");
					new UIAlertView("Error...", error.Description, null, "Ok", null).Show();
					return;
				}

				NSDictionary userInfo = (result as NSDictionary);

				NSObject IDs;
				var success = userInfo.TryGetValue(new NSString("data"), out IDs);

				String s = IDs.ToString();
				try
				{
					while (s.Contains(" = "))
					{
						int start = s.IndexOf(" = ");
						int end = s.IndexOf(";");
						//System.Diagnostics.Debug.WriteLine(start + ", " + end + ":" + s.Substring((start + 3), (end - start-3)) + ":");

						friendsIDs.Add(s.Substring((start + 3), (end - start - 3)));
						s = s.Substring(s.IndexOf("}")+1);
						//System.Diagnostics.Debug.WriteLine(s);
					}
				}
				catch (Exception e) {}

				//System.Diagnostics.Debug.WriteLine("HEY");

				if (error != null)
				{
					
				}
				tcs.TrySetResult(true);
			});
			requestConnection.Start();

			await tcs.Task;


			return friendsIDs;
		}



		public async Task<bool> addEventToCalendar(Event eve)
		{
			eventStore = new EKEventStore();
			return await AddAppointment(eve);
		}

		public async Task<bool> AddAppointment(Event eve)
		{
			var granted = await eventStore.RequestAccessAsync(EKEntityType.Event);//, (bool granted, NSError e) =>
			try
			{
				if (granted.Item1)
				{
					EKEvent newEvent = EKEvent.FromStore(eventStore);
					// set the alarm for 10 minutes from now
					//newEvent.AddAlarm(EKAlarm.FromDate((NSDate)appointment.));
					// make the event start 20 minutes from now and last 30 minutes
					newEvent.StartDate = DateTimeToNSDate(eve.StartDate);
					newEvent.EndDate = DateTimeToNSDate(eve.EndDate);
					newEvent.Title = eve.Title;
					newEvent.Notes = eve.AddressName;
					newEvent.Calendar = eventStore.DefaultCalendarForNewEvents;
					NSError e;
					eventStore.SaveEvent(newEvent, EKSpan.ThisEvent, out e);
					return true;
				}
				else
					new UIAlertView("Access Denied", "User Denied Access to Calendar Data", null, "ok", null).Show();
				// });
			}
			catch (Exception e) { }
			return false;
		}
		public DateTime NSDateToDateTime(NSDate date)
		{
			double secs = date.SecondsSinceReferenceDate;
			if (secs < -63113904000)
				return DateTime.MinValue;
			if (secs > 252423993599)
				return DateTime.MaxValue;
			return (DateTime)date;
		}

		public NSDate DateTimeToNSDate(DateTime date)
		{
			if (date.Kind == DateTimeKind.Unspecified)
				date = DateTime.SpecifyKind(date, DateTimeKind.Local);// or DateTimeKind.Utc, this depends on each app */)
			return (NSDate)date;
		}
	}
}
