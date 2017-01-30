using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class SearchEventTemplate : ContentView
	{
		DataManager _dataManager = new DataManager();
		public SearchEventTemplate()
		{
			InitializeComponent();

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
			try
			{
				await Task.Delay(2);
				EventForLists eveFL = (EventForLists)this.BindingContext;
				Event eve = eveFL.eve;
				if (App.userProfile.EventsFollowed.Exists(ef => ef == eve.EventId))
				{
					trackImg.Foreground = App.HowlOut;
				}
			}
			catch (Exception exc) { }
		}
		/*
		public SearchEventTemplate(Event eve)
		{
			InitializeComponent();
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
		}
		*/
	}
}

