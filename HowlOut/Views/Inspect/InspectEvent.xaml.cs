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
			_dataManager = new DataManager();
			setInfo (eve);

			if (inspectType) { searchSpecific.IsVisible = true; manageSpecific.IsVisible = false; } 
			else  { searchSpecific.IsVisible = false; manageSpecific.IsVisible = true; }

			detailsButton.Clicked += (sender, e) => 
			{
				if(detailedInfo.IsVisible == false) { detailedInfo.IsVisible = true; quickInfo.IsVisible = false;} 
				else { detailedInfo.IsVisible = false; quickInfo.IsVisible = true;}

				if(mapInitialized != true) { 
					mapInitialized = true;
					_dataManager.UtilityManager.setMapForEvent (new Position(eve.Latitude, eve.Longitude), map, mapLayout);
					_dataManager.UtilityManager.setPin(new Position(eve.Latitude, eve.Longitude), map, eve.Title, eve.AddressName);
				}
			};
				
			mapButton.Clicked += (sender, e) => {
				App.coreView.setContentView(new MapsView(eve), "MapsView");
			};

			if (eve.OwnerId == App.StoredUserFacebookId) {
				editLeaveButton.Text = "Edit";
			}
			editLeaveButton.Clicked += (sender, e) => {
				if (eve.OwnerId == App.StoredUserFacebookId) {
					App.coreView.setContentView (new CreateEvent (eve, false), "CreateEvent");
				} else {
					leaveEvent (eve);
				}
			};

			joinButton.Clicked += (sender, e) => {
				joinEvent(eve);
			};

			followButton.Clicked += (sender, e) =>  {
				followEvent(eve);
			};

			inviteButton.Clicked += (sender, e) => {
				App.coreView.setContentView (new InviteView (null, null, eve, App.userProfile.Friends), "InviteView");
			};
		}

		public async void setInfo (Event eve)
		{
			ProfileContent.Content = new ProfileDesignView (eve.Attendees[0], null, null, 130, ProfileDesignView.ProfileDesign.Plain);
			GroupContent.Content = new ProfileDesignView (null, null, eve, 130, ProfileDesignView.ProfileDesign.Plain);

			Position position = App.lastKnownPosition;

			// Time
			DateTime today = DateTime.Now.ToLocalTime();
			System.Diagnostics.Debug.WriteLine ("- " + (eve.StartDate - today) + " : " + (today - eve.StartDate));

			//quickTime.Text = "" + eve.StartDate.DayOfWeek + " at " + _dataManager.UtilityManager.getTime(eve.StartDate);


			StartTime.Text = "" + eve.StartDate.DayOfWeek + " the " + eve.StartDate.Day + " " + eve.StartDate.ToString("MMMM").ToLower();
			EndTime.Text = "From " + _dataManager.UtilityManager.getTime(eve.StartDate) + " till " + _dataManager.UtilityManager.getTime(eve.EndDate);

			//Place
			//ObservableCollection<string> addressList = new ObservableCollection<string>();
			string [] addressList = new string [3];
			addressList = Regex.Split(eve.AddressName, ",");
			for (int i = 0; i < addressList.Length; i++) { 
				Label label = new Label (); 
				label.Text = addressList [i];
				addressLayout.Children.Add(label);
			}
			Distance.Text = _dataManager.UtilityManager.distance(new Position(eve.Latitude, eve.Longitude), position);
			Title.Text = eve.Title;

			Label labelD = new Label ();
			addressLayout.Children.Add(labelD);

			//Other
			setTime(eve.StartDate);

			eventDescription.Text = eve.Description;

			quickInfo.IsVisible = true;
			detailedInfo.IsVisible = false;
		}

		private async void leaveEvent(Event eve)
		{
			bool leaveConfirmed = await App.coreView.displayConfirmMessage("Warning", "You are about to leave this event, would you like to continue?", "Yes", "No");
			if (leaveConfirmed) {
				bool hasLeft = await _dataManager.EventApiManager.UnattendEvent (eve.EventId, App.StoredUserFacebookId);
				if (hasLeft) {
					await App.coreView.displayAlertMessage ("Event Left", "You have successfully left the event.", "Ok");
					App.coreView.setContentView (new EventView (), "Event");
				} else {
					await App.coreView.displayAlertMessage ("Event Not Left", "An error happened and you have not yet left the event, try again.", "Ok");
				}
			}
		}

		private async void joinEvent(Event eve)
		{
			bool joinConfirmed = await App.coreView.displayConfirmMessage("Joining", "You are about to join this event, would you like to continue?", "Yes", "No");
			if (joinConfirmed) {
				bool hasJoined = await _dataManager.EventApiManager.AttendEvent (eve.EventId, App.StoredUserFacebookId);
				if (hasJoined) {
					Event eventWhenJoined = await _dataManager.EventApiManager.GetEventById (eve.EventId);
					await App.coreView.displayAlertMessage ("Event Joined", "You have successfully joined the event.", "Ok");

					App.coreView.setContentView (new InspectController (null, null, eventWhenJoined), "UserProfile");
					//App.coreView.setContentView (new InspectEvent (eventWhenJoined, 2), "InspectEvent");

				} else {
					await App.coreView.displayAlertMessage ("Event Not Joined", "An error happened and you have not yet joined the event, try again.", "Ok");
				}
			}
		}

		private async void followEvent(Event eve)
		{
			bool followConfirmed = await App.coreView.displayConfirmMessage("Following", "You are about to follow this event, would you like to continue?", "Yes", "No");
			if (followConfirmed) {
				bool hasFollowed = await _dataManager.EventApiManager.FollowEvent (eve.EventId, App.userProfile.ProfileId);
				if (hasFollowed) {
					//Event eventWhenJoined = await eventApiManager.GetEventById (eve.EventId);
					await App.coreView.displayAlertMessage ("Event Followed", "You have successfully followed the event.", "Ok");

					//App.coreView.setContentView (new UserProfile (null, null, eventWhenJoined, false, false), "UserProfile");
					//App.coreView.setContentView (new InspectEvent (eventWhenJoined, 2), "InspectEvent");

				} else {
					await App.coreView.displayAlertMessage ("Event Not Followed", "An error happened and you have not yet followed the event, try again.", "Ok");
				}
			}
		}

		private void setTime(DateTime eveTime) {
			var theTimeNow = DateTime.Now;
			var timeBetween = eveTime - theTimeNow;

			if (timeBetween.TotalDays < 1) {
				BigTime.Text = timeBetween.Hours + "";
				SmallTime.Text = "Hour";
			} else if (timeBetween.TotalDays < 7) {
				BigTime.Text = (eveTime.TimeOfDay + "").Substring(0, 5);
				SmallTime.Text = (eveTime.DayOfWeek + "").Substring(0, 3);
			} else {
				BigTime.Text = eveTime.Day + "";
				SmallTime.Text = eveTime.ToString("MMMM") + "";
			}

		}
	}
}