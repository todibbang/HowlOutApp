using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace HowlOut
{
	public partial class InspectEvent : ContentView
	{
		ObservableCollection<Comment> comments = new ObservableCollection<Comment>();
		bool mapInitialized = false;
		ExtMap map = new ExtMap () { IsShowingUser = true, VerticalOptions = LayoutOptions.FillAndExpand };

		UtilityManager util = new UtilityManager();
		DataManager dataManager = new DataManager();

		public InspectEvent (Event eve, bool inspectType)
		{
			InitializeComponent ();
			setInfo (eve);





			if (inspectType) 					{ searchSpecific.IsVisible = true; manageSpecific.IsVisible = false; } 
			else  								{ searchSpecific.IsVisible = false; manageSpecific.IsVisible = true; }

			detailsButton.Clicked += (sender, e) => 
			{
				if(detailedInfo.IsVisible == false) { detailedInfo.IsVisible = true; quickInfo.IsVisible = false; } 
				else 								{ detailedInfo.IsVisible = false; quickInfo.IsVisible = true; }

				if(mapInitialized != true) { 
					mapInitialized = true;
					util.setMapForEvent (eve.AddressPosition, map, mapLayout);
					util.setPin(eve.AddressPosition, map, eve.Title, eve.AddressName);
				}
			};

			/*
			eventHolderButton.Clicked += (sender, e) => {
				App.coreView.setContentView (new UserProfile (eve.Attendees[0], null, null, false, false), "UserProfile");
				//App.coreView.setContentView(new InspectProfile(eve.Attendees[0]), "InspectProfile");
			};

			eventGroupButton.Clicked += (sender, e) => {
				//App.coreView.setContentView (new UserProfile (null, null, null, false, false), "UserProfile");

				//App.coreView.setContentView(new InspectGroup(eve.Attendees), "InspectGroup");
			};
			*/
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

			inviteButton.Clicked += (sender, e) => {
				App.coreView.setContentView (new UserProfile (null, null, eve, true, false), "UserProfile");
			};
		}

		public async void setInfo (Event eve)
		{
			var profilePicUri = dataManager.GetFacebookProfileImageUri(eve.OwnerId);
			eventHolderPhoto.Source = ImageSource.FromUri(profilePicUri);


			Position position = util.getCurrentUserPosition();

			// Time
			DateTime today = DateTime.Now.ToLocalTime();
			System.Diagnostics.Debug.WriteLine ("- " + (eve.StartDate - today) + " : " + (today - eve.StartDate));
			quickTime.Text = "" + eve.StartDate.DayOfWeek + " at " + util.getTime(eve.StartDate);
			StartTime.Text = "" + eve.StartDate.DayOfWeek + " the " + eve.StartDate.Day + " " + eve.StartDate.ToString("MMMM").ToLower();
			EndTime.Text = "From " + util.getTime(eve.StartDate) + " till " + util.getTime(eve.EndDate);

			//Place
			//ObservableCollection<string> addressList = new ObservableCollection<string>();
			string [] addressList = new string [3];
			addressList = Regex.Split(eve.AddressName, ",");
			for (int i = 0; i < addressList.Length; i++) { 
				Label label = new Label (); 
				label.Text = addressList [i];
				addressLayout.Children.Add(label);
			}
			quickDistance.Text = util.distance(eve.AddressPosition, position);
			Label labelD = new Label ();
			labelD.Text = quickDistance.Text;
			addressLayout.Children.Add(labelD);

			//Other
			eventTitle.Text = eve.Title;
			eventDescription.Text = eve.Description;
			eventAttending.Text = (eve.Attendees.Count) + "/" + eve.MaxSize;

			quickInfo.IsVisible = true;
			detailedInfo.IsVisible = false;
		}

		private async void leaveEvent(Event eve)
		{
			bool leaveConfirmed = await App.coreView.displayConfirmMessage("Warning", "You are about to leave this event, would you like to continue?", "Yes", "No");
			if (leaveConfirmed) {
				bool hasLeft = await dataManager.UnattendEvent (eve.EventId, App.StoredUserFacebookId);
				if (hasLeft) {
					await App.coreView.displayAlertMessage ("Event Left", "You have successfully left the event.", "Ok");
					App.coreView.setContentView (new ManageEvent (), "ManageEvent");
				} else {
					await App.coreView.displayAlertMessage ("Event Not Left", "An error happened and you have not yet left the event, try again.", "Ok");
				}
			}
		}

		private async void joinEvent(Event eve)
		{
			bool joinConfirmed = await App.coreView.displayConfirmMessage("Joining", "You are about to join this event, would you like to continue?", "Yes", "No");
			if (joinConfirmed) {
				bool hasJoined = await dataManager.AttendEvent (eve.EventId, App.StoredUserFacebookId);
				if (hasJoined) {
					Event eventWhenJoined = await dataManager.GetEventById (eve.EventId);
					await App.coreView.displayAlertMessage ("Event Joined", "You have successfully joined the event.", "Ok");

					App.coreView.setContentView (new UserProfile (null, null, eventWhenJoined, false, false), "UserProfile");
					//App.coreView.setContentView (new InspectEvent (eventWhenJoined, 2), "InspectEvent");

				} else {
					await App.coreView.displayAlertMessage ("Event Not Joined", "An error happened and you have not yet joined the event, try again.", "Ok");
				}
			}
		}
	}
}