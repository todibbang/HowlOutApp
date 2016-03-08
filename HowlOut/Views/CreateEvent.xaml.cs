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
		UtilityManager utilityManager = new UtilityManager ();
		EventApiManager eventApiManager;
		MapsView mapView;
		private HttpClient httpClient;

		public Event newEvent;
		Dictionary<string, int> agePicker = new Dictionary<string, int> { };
		Dictionary<string, int> sizePicker = new Dictionary<string, int> { };

		public CreateEvent (Event givenEvent, bool isCreate)
		{
			httpClient = new HttpClient(new NativeMessageHandler());
			eventApiManager = new EventApiManager (httpClient);
			newEvent = givenEvent;
			InitializeComponent ();
			mapView = new MapsView (utilityManager.getCurrentUserPosition());

			for (int i = 18; i < 100; i++) agePicker.Add ("" + i, i);
			foreach (string age in agePicker.Keys) { minAge.Items.Add(age); maxAge.Items.Add(age);}

			int sizeNumber = 5;
			for (int i = 0; i < 20; i++) { sizePicker.Add ("" + sizeNumber, sizeNumber); sizeNumber += 5;}
			foreach (string size in sizePicker.Keys) { minSize.Items.Add(size); maxSize.Items.Add(size);}

			/// set title and description
			title.TextChanged += (sender, e) => { newEvent.Title = title.Text; };
			description.TextChanged += (sender, e) => { newEvent.Description = description.Text; };

			/// set event type
			fest.Clicked += (sender, e) => { fest = typeButtonPressed(fest); };
			sport.Clicked += (sender, e) => { sport = typeButtonPressed(sport); };
			kultur.Clicked += (sender, e) => { kultur = typeButtonPressed(kultur); };
			film.Clicked += (sender, e) => { film = typeButtonPressed(film); };
			musik.Clicked += (sender, e) => { musik = typeButtonPressed(musik); };
			cafe.Clicked += (sender, e) => { cafe = typeButtonPressed(cafe); };
			mad.Clicked += (sender, e) => { mad = typeButtonPressed(mad); };
			hobby.Clicked += (sender, e) => { hobby = typeButtonPressed(hobby); };

			/// set time and date
			startDate.PropertyChanged += (sender, e) => { newEvent.StartDate = startDate.Date.Add(startTime.Time); };
			startTime.PropertyChanged += (sender, e) => { newEvent.StartDate = startDate.Date.Add(startTime.Time); };
			endDate.PropertyChanged += (sender, e) => { newEvent.EndDate = endDate.Date.Add(endTime.Time); };
			endTime.PropertyChanged += (sender, e) => { newEvent.EndDate = endDate.Date.Add(endTime.Time); };

			/// set location
			locationButton.Clicked += (sender, e) => {
				if (newEvent.AddressPosition.Latitude == 0 && newEvent.AddressPosition.Longitude == 0){
					mapView = new MapsView (utilityManager.getCurrentUserPosition());
				}
				else {
					mapView = new MapsView (newEvent.AddressPosition);
				}
				mapView.createEventView = this;
				App.coreView.setContentView (mapView, "MapsView");
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
				if(isCreate) {
					LaunchEvent(newEvent);
				} else {
					eventApiManager.UpdateEvent(newEvent);
				}
			};

			cancelButton.Clicked += (sender, e) => { CancelTheEvent(); };
		}

		private void setNewEvent()
		{
			cancelButton.IsVisible = false;
			newEvent.OwnerId = App.StoredUserFacebookId;
			newEvent.StartDate = startDate.Date.Add(startTime.Time);
			newEvent.EndDate = endDate.Date.Add(endTime.Time);

			//newEvent.address.data.position = utilityManager.getCurrentUserPosition();


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

			title.Text = newEvent.Title;
			description.Text = newEvent.Description;

			startDate.Date = newEvent.StartDate;
			endDate.Date = newEvent.EndDate;

			locationButton.Text = newEvent.AddressName;

			minAge.SelectedIndex = agePicker ["16"];
			maxAge.SelectedIndex = agePicker [newEvent.MaxAge.ToString ()];

			minSize.SelectedIndex = sizePicker [newEvent.MinSize.ToString ()];
			maxSize.SelectedIndex = sizePicker [newEvent.MaxSize.ToString ()];
		}
			
		private async void LaunchEvent(Event eventToCreate)
		{
			EventType eventType1 = new EventType{ EventTypeId=2, Type="Outdoor"};
			List<EventType> EventTypes = new List<EventType>();
			EventTypes.Add (eventType1);
			if (eventToCreate.AddressName == null) {
				eventToCreate.AddressName = "Unknown";
			}
			EventDBO newEventAsDBO = new EventDBO{OwnerId = eventToCreate.OwnerId, Title = eventToCreate.Title, 
				Description = eventToCreate.Description,
				StartDate = eventToCreate.StartDate, EndDate = eventToCreate.EndDate, MinAge = eventToCreate.MinAge,
				MaxAge = eventToCreate.MaxAge, MinSize = eventToCreate.MinSize, MaxSize = eventToCreate.MaxSize, 
				Public = true, AddressPosition = eventToCreate.AddressPosition, 
				AddressName = eventToCreate.AddressName, EventTypes = EventTypes};

			Event eventCreated = await eventApiManager.CreateEvent (newEventAsDBO);
			eventCreated.Attendees = new List<Profile> ();
			eventCreated.Followers = new List<Profile> ();
			if (eventCreated != null) {
				App.coreView.setContentView (new UserProfile (null, null, eventCreated), "UserProfile");
				//App.coreView.setContentView (new InspectEvent (eventCreated, 2), "InspectEvent");
			} else {
				App.coreView.displayAlertMessage ("Error", "Event not created, try again", "Ok");
			}
		}

		private Button typeButtonPressed(Button typeButton)
		{
			if (typeButton.BackgroundColor == Color.White) {
				if (newEvent.EventTypes.Count < 3) {
					typeButton.BackgroundColor = Color.FromHex ("00E0A0");
					typeButton.TextColor = Color.White;
					//newEvent.EventTypes.Add (typeButton.Text.ToString ());
				}
			} else {
				typeButton.BackgroundColor = Color.White;
				typeButton.TextColor = Color.FromHex ("00E0A0");
				//newEvent.EventTypes.Remove (typeButton.Text.ToString ());
			}
			return typeButton;
		}

		public async void CancelTheEvent()
		{
			bool confirmDelete = await App.coreView.displayConfirmMessage ("Warning", "You are about to delete this event permanently, would you like to continue", "Yes", "No");

			if (confirmDelete) {
				bool wasEventDeleted = await eventApiManager.DeleteEvent (newEvent.EventId);
				if (wasEventDeleted) {
					await App.coreView.displayAlertMessage ("Event Deleted", "The event was successfully cancelled", "Ok");
					App.coreView.setContentView (new ManageEvent (), "ManageEvent");
				} else {
					App.coreView.displayAlertMessage ("Event Not Deleted", "The event was not cancelled, try again", "Ok");
				}
			}
		}

		public void setLocationButton(string name) {
			locationButton.Text = name;
		}
	}
}

