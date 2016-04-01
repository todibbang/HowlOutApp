using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Plugin.Geolocator;
using Xamarin.Forms.Maps;
using System.Net.Http;
using ModernHttpClient;

namespace HowlOut
{
	public partial class CreateEvent : ContentView
	{
		MapsView mapView;
		DataManager _dataManager;

		public Event newEvent;
		Dictionary<string, int> agePicker = new Dictionary<string, int> { };
		Dictionary<string, int> sizePicker = new Dictionary<string, int> { };

		private bool Launching = false;

		public CreateEvent (Event givenEvent, bool isCreate)
		{
			_dataManager = new DataManager ();
			newEvent = givenEvent;
			InitializeComponent ();

			mapView = new MapsView (App.lastKnownPosition);

			for (int i = 18; i < 100; i++) agePicker.Add ("" + i, i);
			foreach (string age in agePicker.Keys) { minAge.Items.Add(age); maxAge.Items.Add(age);}

			int sizeNumber = 5;
			for (int i = 0; i < 20; i++) { sizePicker.Add ("" + sizeNumber, sizeNumber); sizeNumber += 5;}
			foreach (string size in sizePicker.Keys) { minSize.Items.Add(size); maxSize.Items.Add(size);}

			/// set title and description
			title.TextChanged += (sender, e) => { newEvent.Title = title.Text; };
			description.TextChanged += (sender, e) => { newEvent.Description = description.Text; };

			/// set event type
			fest.Clicked += (sender, e) => { fest = typeButtonPressed(fest, EventType.Party); };
			sport.Clicked += (sender, e) => { sport = typeButtonPressed(sport, EventType.Sport); };
			kultur.Clicked += (sender, e) => { kultur = typeButtonPressed(kultur, EventType.Culture); };
			film.Clicked += (sender, e) => { film = typeButtonPressed(film, EventType.Movie); };
			musik.Clicked += (sender, e) => { musik = typeButtonPressed(musik, EventType.Music); };
			cafe.Clicked += (sender, e) => { cafe = typeButtonPressed(cafe, EventType.Cafe); };
			mad.Clicked += (sender, e) => { mad = typeButtonPressed(mad, EventType.Food); };
			hobby.Clicked += (sender, e) => { hobby = typeButtonPressed(hobby, EventType.Hobby); };

			/// set time and date
			startDate.PropertyChanged += (sender, e) => { 
				newEvent.StartDate = startDate.Date.Add(startTime.Time); 
				var newTimeSpan = newEvent.StartDate + new TimeSpan(1,0,0);
				if(newEvent.EndDate.Ticks < newTimeSpan.Ticks) {
					newEvent.EndDate = newEvent.StartDate + new TimeSpan(2,0,0);
					endTime.Time = newEvent.EndDate.TimeOfDay;
					endDate.Date = newEvent.EndDate;
				}
			};
			startTime.PropertyChanged += (sender, e) => { 
				newEvent.StartDate = startDate.Date.Add(startTime.Time);
				var newTimeSpan = newEvent.StartDate + new TimeSpan(1,0,0);
				if(newEvent.EndDate.Ticks < newTimeSpan.Ticks) {
					newEvent.EndDate = newEvent.StartDate + new TimeSpan(2,0,0);
					endTime.Time = newEvent.EndDate.TimeOfDay;
					endDate.Date = newEvent.EndDate;
				}
			};
			endDate.PropertyChanged += (sender, e) => { newEvent.EndDate = endDate.Date.Add(endTime.Time); };
			endTime.PropertyChanged += (sender, e) => { newEvent.EndDate = endDate.Date.Add(endTime.Time); };

			/// set location
			locationButton.Clicked += (sender, e) => {
				if (newEvent.Latitude == 0 && newEvent.Longitude == 0){
					mapView = new MapsView (App.lastKnownPosition);
				}
				else {
					mapView = new MapsView (new Position(newEvent.Latitude, newEvent.Longitude));
				}
				mapView.createEventView = this;
				App.coreView.setContentView (mapView, "MapsView");
			};

			selectBannerButton.Clicked += (sender, e) => {
				SelectBannerView selectBannerView = new SelectBannerView();
				selectBannerView.createEventView = this;
				App.coreView.setContentView(selectBannerView, "");
			};

			/// set age and size limits
			minAge.SelectedIndexChanged += (sender, args) => {
				if (minAge.SelectedIndex != -1) { string age = minAge.Items[minAge.SelectedIndex]; newEvent.MinAge = agePicker[age]; } };
			maxAge.SelectedIndexChanged += (sender, args) => {
				if (maxAge.SelectedIndex != -1) { string age = maxAge.Items[maxAge.SelectedIndex]; newEvent.MaxAge = agePicker[age]; } };
			minSize.SelectedIndexChanged += (sender, args) => {
				if (minSize.SelectedIndex != -1) { string size = minSize.Items[minSize.SelectedIndex]; newEvent.MinSize = sizePicker[size]; } };
			maxSize.SelectedIndexChanged += (sender, args) => {
				if (maxSize.SelectedIndex != -1) { string size = maxSize.Items[maxSize.SelectedIndex]; newEvent.MaxSize = sizePicker[size]; } };

			if (isCreate) {
				setNewEvent ();
			} else {
				setEditEvent ();
			}

			launchButton.Clicked += (sender, e) => {
				if(isCreate && !Launching) {
					LaunchEvent(newEvent);
					Launching = true;
				} else {
					UpdateEvent(newEvent);
				}
			};

			cancelButton.Clicked += (sender, e) => { CancelTheEvent(); };
		}

		public void setBanner(string banner) {
			selectBannerButton.Text = "";
			selectBannerButton.Image = banner;
			selectBannerButton.WidthRequest = App.coreView.Width;
			selectBannerButton.HeightRequest = App.coreView.Width / 2;
		}

		private void setNewEvent()
		{
			cancelButton.IsVisible = false;
			newEvent.Owner = new Profile(){ProfileId = App.StoredUserFacebookId};

			System.Diagnostics.Debug.WriteLine (startTime.Time);
			System.Diagnostics.Debug.WriteLine (DateTime.Now.TimeOfDay);
			System.Diagnostics.Debug.WriteLine (new TimeSpan (1,10,0));

			startTime.Time = (new TimeSpan (DateTime.Now.TimeOfDay.Hours + 3, 0, 0));
			endTime.Time = (new TimeSpan (DateTime.Now.TimeOfDay.Hours + 5, 0, 0));

			newEvent.StartDate = startDate.Date.Add(startTime.Time);
			newEvent.EndDate = endDate.Date.Add(endTime.Time);

			minAge.SelectedIndex = 0;
			maxAge.SelectedIndex = agePicker.Count;

			newEvent.MinAge = agePicker [minAge.Items[minAge.SelectedIndex]];
			newEvent.MaxAge = agePicker [maxAge.Items[maxAge.SelectedIndex]];

			minSize.SelectedIndex = 0;
			maxSize.SelectedIndex = sizePicker.Count;
			newEvent.MinSize = sizePicker [minSize.Items[minSize.SelectedIndex]];
			newEvent.MaxSize = sizePicker [maxSize.Items[maxSize.SelectedIndex]];
		}

		private void setEditEvent()
		{
			cancelButton.IsVisible = true;

			launchButton.Text = "Update";

			title.Text = newEvent.Title;
			description.Text = newEvent.Description;

			startTime.Time = newEvent.StartDate.TimeOfDay;
			endTime.Time = newEvent.EndDate.TimeOfDay;
			startDate.Date = newEvent.StartDate.Date;
			endDate.Date = newEvent.EndDate.Date;

			locationButton.Text = newEvent.AddressName;

			int minvalue = 0;
			int maxvalue = 0;

			for (int i = 0; i < minAge.Items.Count; i++) {
				if (minAge.Items [i] == newEvent.MinAge.ToString ())
					minvalue = i;
				if (maxAge.Items [i] == newEvent.MaxAge.ToString ())
					maxvalue = i;
			}

			minAge.SelectedIndex = minvalue;
			maxAge.SelectedIndex = maxvalue;

			for (int i = 0; i < minSize.Items.Count; i++) {
				if (minSize.Items [i] == newEvent.MinSize.ToString ())
					minvalue = i;
				if (maxSize.Items [i] == newEvent.MaxSize.ToString ())
					maxvalue = i;
			}

			minSize.SelectedIndex = minvalue;
			maxSize.SelectedIndex = maxvalue;

			if(newEvent.EventTypes.Contains(EventType.Party)) { fest = PreSetButton(fest, EventType.Party); };
			if(newEvent.EventTypes.Contains(EventType.Sport)) { sport = PreSetButton(sport, EventType.Sport); };
			if(newEvent.EventTypes.Contains(EventType.Culture)) { kultur = PreSetButton(kultur, EventType.Culture); };
			if(newEvent.EventTypes.Contains(EventType.Movie)) { film = PreSetButton(film, EventType.Movie); };
			if(newEvent.EventTypes.Contains(EventType.Music)) { musik = PreSetButton(musik, EventType.Music); };
			if(newEvent.EventTypes.Contains(EventType.Cafe)) { cafe = PreSetButton(cafe, EventType.Cafe); };
			if(newEvent.EventTypes.Contains(EventType.Food)) { mad = PreSetButton(mad, EventType.Food); };
			if(newEvent.EventTypes.Contains(EventType.Hobby)) { hobby = PreSetButton(hobby, EventType.Hobby); };
		}
			
		private async void LaunchEvent(Event eventToCreate)
		{
			if (String.IsNullOrWhiteSpace (eventToCreate.Title)) {
				App.coreView.displayAlertMessage ("Title Missing", "Title is missing", "Ok");
			} else if (String.IsNullOrWhiteSpace (eventToCreate.Description)) {
				App.coreView.displayAlertMessage ("Description Missing", "Description is missing", "Ok");
			}	else if (eventToCreate.EventTypes.Count == 0) {
				App.coreView.displayAlertMessage ("EventTypes Missing", "No Event Type has been selected", "Ok");
			} else if (String.IsNullOrWhiteSpace (eventToCreate.AddressName) || eventToCreate.Latitude == 0) {
				App.coreView.displayAlertMessage ("Address Missing", "No valid address has been selected", "Ok");
			} else if (String.IsNullOrWhiteSpace (eventToCreate.BannerName)) {
				App.coreView.displayAlertMessage ("Banner Missing", "No banner has been selected", "Ok");
			}else {
				EventDBO newEventAsDBO = new EventDBO{
					Owner = eventToCreate.Owner, 
					Title = eventToCreate.Title, 
					Description = eventToCreate.Description,
					StartDate = eventToCreate.StartDate, 
					EndDate = eventToCreate.EndDate, 
					MinAge = eventToCreate.MinAge,
					MaxAge = eventToCreate.MaxAge, 
					MinSize = eventToCreate.MinSize, 
					MaxSize = eventToCreate.MaxSize, 
					Public = true, 
					Latitude = eventToCreate.Latitude,
					Longitude = eventToCreate.Longitude, 
					AddressName = eventToCreate.AddressName, 
					EventTypes = eventToCreate.EventTypes,
					Banner = eventToCreate.BannerName};

				Event eventCreated = await _dataManager.EventApiManager.CreateEvent (newEventAsDBO);
				Launching = false;

				if (eventCreated != null) {
					eventCreated.Attendees = new List<Profile> ();
					eventCreated.Followers = new List<Profile> ();
					App.coreView.setContentView (new InspectController (null, null, eventCreated), "UserProfile");
				} else {
					await App.coreView.displayAlertMessage ("Error", "Event not created, try again", "Ok");
				}
			}
		}

		private async void UpdateEvent(Event eventToUpdate)
		{
			bool eventUpdated = await _dataManager.EventApiManager.UpdateEvent (eventToUpdate);

			if (eventUpdated) {
				App.coreView.setContentView (new InspectController (null, null, eventToUpdate), "UserProfile");
			} else {
				await App.coreView.displayAlertMessage ("Error", "Event not updated, try again", "Ok");
			}
		}

		private Button typeButtonPressed(Button typeButton, EventType eventType)
		{
			if (typeButton.BackgroundColor == Color.White) {
				if (newEvent.EventTypes.Count < 3) {
					typeButton.BackgroundColor = Color.FromHex ("00e1c4");
					typeButton.TextColor = Color.White;
					newEvent.EventTypes.Add (eventType);
				}
			} else {
				typeButton.BackgroundColor = Color.White;
				typeButton.TextColor = Color.FromHex ("00e1c4");
				newEvent.EventTypes.Remove (eventType);
			}
			return typeButton;
		}

		public async void CancelTheEvent()
		{
			bool confirmDelete = await App.coreView.displayConfirmMessage ("Warning", "You are about to delete this event permanently, would you like to continue", "Yes", "No");

			if (confirmDelete) {
				bool wasEventDeleted = await _dataManager.EventApiManager.DeleteEvent (newEvent.EventId);
				if (wasEventDeleted) {
					await App.coreView.displayAlertMessage ("Event Deleted", "The event was successfully cancelled", "Ok");
					App.coreView.setContentView (new EventView (), "Event");
				} else {
					App.coreView.displayAlertMessage ("Event Not Deleted", "The event was not cancelled, try again", "Ok");
				}
			}
		}

		public void setLocationButton(string name) {
			locationButton.Text = name;
		}

		private Button PreSetButton(Button typeButton, EventType eventType)
		{
			System.Diagnostics.Debug.WriteLine ("1234567890");

			typeButton.BackgroundColor = Color.FromHex ("00e1c4");
			typeButton.TextColor = Color.White;

			return typeButton;
		}
	}
}

