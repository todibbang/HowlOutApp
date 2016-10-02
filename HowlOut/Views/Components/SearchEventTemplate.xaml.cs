using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Text.RegularExpressions;

namespace HowlOut
{
	public partial class SearchEventTemplate : ContentView
	{
		DataManager _dataManager = new DataManager();
		public SearchEventTemplate ()
		{
			InitializeComponent ();


		}
		public SearchEventTemplate (Event eve)
		{
			InitializeComponent ();
			EventForLists efl = new EventForLists(eve);
			BindingContext = efl;

			attendingInfo.Text = (eve.Attendees.Count + 1) + "/" + eve.MaxSize;

			topTime.Text = eve.StartDate.ToString("ddd dd MMM");
			bottomTime.Text = eve.StartDate.ToString("HH:mm") + "-" + eve.EndDate.ToString("HH:mm");

			topDist.Text = efl.Distance + " km";

			string[] addressList = new string[3];
			addressList = Regex.Split(eve.AddressName, ",");
			for (int i = 0; i < addressList.Length; i++)
			{
				Label label = new Label() { TextColor = Color.FromHex("646464") };
				label.Text = addressList[i];
				label.FontSize = 14;
			}
			if (addressList.Length == 2)
			{
				bottomDist.Text = addressList[0].Substring(5).Trim();
			}
			else {
				bottomDist.Text = addressList[1].Substring(5).Trim();
			}

			/*
			SubjectButton.Clicked += (sender, e) => {
				InspectController inspect = new InspectController(null, null, eve);
				App.coreView.setContentViewWithQueue(inspect, "", inspect.getScrollView());
			};
			*/
		}
	}
}

