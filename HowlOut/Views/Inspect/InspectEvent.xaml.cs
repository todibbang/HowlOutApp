using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Http;
using ModernHttpClient;

namespace HowlOut
{
	public partial class InspectEvent : ContentView
	{
		ObservableCollection<Comment> comments = new ObservableCollection<Comment>();
		bool mapInitialized = false;
		ExtMap map = new ExtMap () { IsShowingUser = true, VerticalOptions = LayoutOptions.FillAndExpand };
		DataManager _dataManager = new DataManager();
		EventForLists efl;

		public InspectEvent (Event eve, bool inspectType, ScrollView scrollView)
		{
			InitializeComponent ();
			efl = new EventForLists(eve);
			BindingContext = efl;
			//attendingInfo.Text = (eve.Attendees.Count + 1) + "/" + eve.MaxSize;


			_dataManager = new DataManager();
			setInfo (eve);

			//System.Diagnostics.Debug.WriteLine("Eve id:D " + eve.EventId);

			if (!_dataManager.IsEventJoined(eve)) { searchSpecific.IsVisible = true; manageSpecific.IsVisible = false; } 
			else  { searchSpecific.IsVisible = false; manageSpecific.IsVisible = true; }

			detailsButton.Clicked += (sender, e) => 
			{
				
				if(detailedInfo.IsVisible == false) { 
					detailedInfo.IsVisible = true; 
					quickInfo.IsVisible = false;
					detailsButton.Text = "Hide Map";
					App.scrollTo(100);
				} 
				else { 
					detailedInfo.IsVisible = false; 
					quickInfo.IsVisible = true;
					detailsButton.Text = "Show Map";
					App.scrollTo(200);
				}

				if(mapInitialized != true) { 
					mapInitialized = true;
					_dataManager.UtilityManager.setMapForEvent (new Position(eve.Latitude, eve.Longitude), map, mapLayout);
					_dataManager.UtilityManager.setPin(new Position(eve.Latitude, eve.Longitude), map, eve.Title, eve.AddressName);
				}
			};

			scrollView.Scrolled += (sender, e) =>
			{
				if (scrollView.ScrollY < 100 && scrollView.ScrollY > 0)
				{
					titleInfo.TranslationY = scrollView.ScrollY;
				}
			};
				
			mapButton.Clicked += (sender, e) => {
				App.coreView.setContentViewWithQueue(new MapsView(eve), "MapsView", null);
			};

			if (_dataManager.IsEventYours(eve)) {
				editLeaveButton.Text = "Edit";
			}

			editLeaveButton.Clicked += (sender, e) => {
				if (_dataManager.IsEventYours(eve)) {
					App.coreView.setContentViewWithQueue (new CreateEvent (eve, false), "CreateEvent", null);
				} else {
					_dataManager.leaveEvent (eve);
				}
			};

			joinButton.Clicked += (sender, e) => {
				_dataManager.joinEvent(eve);
			};

			followButton.Clicked += (sender, e) =>  {
				_dataManager.followEvent(eve);
			};

			inviteButton.Clicked += (sender, e) => {
				App.coreView.setContentViewWithQueue (new InviteView (null, eve, InviteView.WhatToShow.PeopleToInviteToEvent), "InviteView", null);
			};
		}

		public async void setInfo (Event eve)
		{
			quickInfo.IsVisible = true;
			detailedInfo.IsVisible = false;

			topTime.Text = eve.StartDate.ToString("ddd dd MMM");
			bottomTime.Text = eve.StartDate.ToString("HH:mm") + "-" + eve.EndDate.ToString("HH:mm");

			topDist.Text = efl.Distance + " km";


			//ProfileContent.Content = new ProfileDesignView (eve.Owner, null, null, 130, ProfileDesignView.Design.Plain, true);
			//GroupContent.Content = new EventDesignView (eve, 130, EventDesignView.Design.Plain);
			//BannerHeight.Height = (0.524 * App.coreView.Width) - 60;

			//Title.Text = eve.Title;
			eventDescription.Text = eve.Description;
			//Distance.Text = _dataManager.UtilityManager.distance(new Position(eve.Latitude, eve.Longitude), App.lastKnownPosition);
			//var Times = _dataManager.UtilityManager.setTime(eve.StartDate);
			//BigTime.Text = Times [0];
			//SmallTime.Text = Times [1];

			StartTime.Text = "" + eve.StartDate.DayOfWeek + " the " + eve.StartDate.Day + " " + eve.StartDate.ToString("MMMM").ToLower();
			EndTime.Text =  _dataManager.UtilityManager.getTime(eve.StartDate) + " - " + _dataManager.UtilityManager.getTime(eve.EndDate);

			string [] addressList = new string [3];
			addressList = Regex.Split(eve.AddressName, ",");
			for (int i = 0; i < addressList.Length; i++) { 
				Label label = new Label () {TextColor = Color.FromHex("646464")}; 
				label.Text = addressList [i];
				label.FontSize = 14;
				addressLayout.Children.Add(label);
			}
			if (addressList.Length == 2)
			{
				bottomDist.Text = addressList[0].Substring(5).Trim();
			}
			else {
				bottomDist.Text = addressList[1].Substring(5).Trim();
			}
		}
	}
}