using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class ManageEventTemplate : ContentView
	{
		DataManager _dataManager = new DataManager();
		public ManageEventTemplate ()
		{
			InitializeComponent ();




		}
		public ManageEventTemplate (Event eve)
		{
			InitializeComponent ();
			BindingContext = new EventForLists(eve);

			distance.Text += " km";
			var times = _dataManager.UtilityManager.setTime (eve.StartDate);
			time.Text = times[0] + " " + times[1];
			attending.Text = (eve.NumberOfAttendees) + "/" + eve.MaxSize;

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

