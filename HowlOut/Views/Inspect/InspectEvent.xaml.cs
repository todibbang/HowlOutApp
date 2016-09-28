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

		public InspectEvent (Event eve, bool inspectType)
		{
			InitializeComponent ();
			BindingContext = new EventForLists (eve);
			attendingInfo.Text = (eve.Attendees.Count + 1) + "/" + eve.MaxSize;
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
					detailsButton.Text = "    Show Less Details";
				} 
				else { 
					detailedInfo.IsVisible = false; 
					quickInfo.IsVisible = true;
					detailsButton.Text = "    Show More Details";
				}

				if(mapInitialized != true) { 
					mapInitialized = true;
					_dataManager.UtilityManager.setMapForEvent (new Position(eve.Latitude, eve.Longitude), map, mapLayout);
					_dataManager.UtilityManager.setPin(new Position(eve.Latitude, eve.Longitude), map, eve.Title, eve.AddressName);
				}
			};
				
			mapButton.Clicked += (sender, e) => {
				App.coreView.setContentViewWithQueue(new MapsView(eve), "MapsView");
			};

			if (eve.Owner.ProfileId == App.StoredUserFacebookId) {
				editLeaveButton.Text = "Edit";
			}
			editLeaveButton.Clicked += (sender, e) => {
				if (eve.Owner.ProfileId == App.StoredUserFacebookId) {
					App.coreView.setContentViewWithQueue (new CreateEvent (eve, false), "CreateEvent");
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
				App.coreView.setContentViewWithQueue (new InviteView (null, eve, InviteView.WhatToShow.PeopleToInviteToEvent), "InviteView");
			};
		}

		public async void setInfo (Event eve)
		{
			quickInfo.IsVisible = true;
			detailedInfo.IsVisible = false;

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
				label.FontSize = 12;
				addressLayout.Children.Add(label);
			}
		}
	}
}