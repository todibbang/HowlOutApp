using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class ManageEventTemplate : ContentView
	{
		DataManager _dataManager = new DataManager();
		public ManageEventTemplate ()
		{
			InitializeComponent ();

			TapGestureRecognizer tgr = new TapGestureRecognizer();
			tgr = new TapGestureRecognizer();
			tgr.Tapped += async (sender, e) =>
			{
				EventForLists eveFL = (EventForLists)this.BindingContext;
				Event eve = eveFL.eve;
				bool success = false;
				if (App.userProfile.EventsFollowed.Exists(ef => ef == eve.EventId))
				{
					success = await _dataManager.AttendTrackEvent(eve, false, false);
					if (success)
					{
						trackImg.Foreground = App.HowlOutFade;
					}
				}
				else {
					success = await _dataManager.AttendTrackEvent(eve, true, false);
					if (success)
					{
						trackImg.Foreground = App.HowlOut;
					}
				}
			};
			trackImg.GestureRecognizers.Add(tgr);
			setFollowButton();
		}

		public async void setFollowButton()
		{
			await Task.Delay(2);
			EventForLists eveFL = (EventForLists)this.BindingContext;
			Event eve = eveFL.eve;
			if (App.userProfile.EventsFollowed.Exists(ef => ef == eve.EventId))
			{
				trackImg.Foreground = App.HowlOut;
			}
		}

		public ManageEventTemplate (Event eve)
		{
			InitializeComponent ();
			BindingContext = new EventForLists(eve);

			//distance.Text += " km";
			var times = _dataManager.UtilityManager.setTime (eve.StartDate);
			//time.Text = times[0] + " " + times[1];
			//attending.Text = (eve.NumberOfAttendees) + "/" + eve.MaxSize;

			System.Diagnostics.Debug.WriteLine("Number of attenInt: " + eve.NumberOfAttendees + ", and number of AttendList: " + eve.Attendees.Count);
			if (_dataManager.IsEventYours(eve))
			{
				System.Diagnostics.Debug.WriteLine("Number of attenInt: " + eve.NumberOfAttendees + ", and number of AttendList: " + eve.Attendees.Count);
			}

			//address.Text = "At " + eve.eve.AddressName;

			//ProfilView.Content = new ProfileDesignView (eve.Owner, null, null, 80, ProfileDesignView.Design.Plain, false);
			//EventView.Content = new EventDesignView (eve, 80, EventDesignView.Design.Plain);

			/*
			SubjectButton.Clicked += (sender, e) => {
				InspectController inspect = new InspectController(null, null, eve);
				App.coreView.setContentViewWithQueue(inspect,"", inspect.getScrollView());
			};
			*/
		}
	}
}

