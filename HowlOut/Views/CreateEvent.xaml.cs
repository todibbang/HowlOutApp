using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Plugin.Geolocator;
using Xamarin.Forms.Maps;

namespace HowlOut
{
	public partial class CreateEvent : ContentView
	{
		public Event newEvent = new Event();
		public Button locationButton = new Button();
		UtilityManager utilityManager = new UtilityManager ();

		public CreateEvent ()
		{
			InitializeComponent ();

			newEvent.OwnerId = App.StoredUserFacebookId;
			locationButton.WidthRequest = 200;
			locationButton.HeightRequest = 40;
			locationButton.Text = "Location";
			locationButtonPlace.Children.Add(locationButton);

			Dictionary<string, int> agePicker = new Dictionary<string, int> { };
			for (int i = 18; i < 100; i++) agePicker.Add ("" + i, i);
			foreach (string age in agePicker.Keys) { minAge.Items.Add(age); maxAge.Items.Add(age);}
			minAge.SelectedIndex = 0;
			maxAge.SelectedIndex = agePicker.Count;
			newEvent.MinAge = agePicker [minAge.Items[minAge.SelectedIndex]];
			newEvent.MaxAge = agePicker [maxAge.Items[maxAge.SelectedIndex]];

			Dictionary<string, int> sizePicker = new Dictionary<string, int> { };
			int sizeNumber = 5;
			for (int i = 0; i < 20; i++) {
				sizePicker.Add ("" + sizeNumber, sizeNumber);
				sizeNumber += 5;
			}
			foreach (string size in sizePicker.Keys) { minSize.Items.Add(size); maxSize.Items.Add(size);}
			minSize.SelectedIndex = 0;
			maxSize.SelectedIndex = sizePicker.Count;
			newEvent.MinSize = sizePicker [minSize.Items[minSize.SelectedIndex]];
			newEvent.MaxSize = sizePicker [maxSize.Items[maxSize.SelectedIndex]];

			Position pos = utilityManager.getCurrentUserPosition();
			newEvent.Latitude = pos.Latitude;
			newEvent.Longitude = pos.Longitude;


			title.TextChanged += (sender, e) => { newEvent.Title = title.Text; };
			description.TextChanged += (sender, e) => { newEvent.Description = description.Text; };


			//startTime.Time = TimeSpan.FromTicks(DateTime.Now.ToLocalTime().Ticks);
			//endTime.Time = TimeSpan.FromTicks(DateTime.Now.ToLocalTime().Ticks);
			//startTime.Time = DateTime.Now.ToLocalTime();
			//endTime.Time = DateTime.Now.ToLocalTime();
			newEvent.StartDate = startDate.Date.Add(startTime.Time);
			newEvent.EndDate = endDate.Date.Add(endTime.Time);

			startDate.PropertyChanged += (sender, e) => { newEvent.StartDate = startDate.Date.Add(startTime.Time); };
			startTime.PropertyChanged += (sender, e) => { newEvent.StartDate = startDate.Date.Add(startTime.Time); };
			endDate.PropertyChanged += (sender, e) => { newEvent.EndDate = endDate.Date.Add(endTime.Time); };
			endTime.PropertyChanged += (sender, e) => { newEvent.EndDate = endDate.Date.Add(endTime.Time); };



			minAge.SelectedIndexChanged += (sender, args) => {
				if (minAge.SelectedIndex != -1) { string age = minAge.Items[minAge.SelectedIndex]; newEvent.MinAge = agePicker[age]; } };
			maxAge.SelectedIndexChanged += (sender, args) => {
				if (maxAge.SelectedIndex != -1) { string age = maxAge.Items[maxAge.SelectedIndex]; newEvent.MaxAge = agePicker[age]; } };
			minSize.SelectedIndexChanged += (sender, args) => {
				if (minSize.SelectedIndex != -1) { string size = minSize.Items[minSize.SelectedIndex]; newEvent.MinSize = sizePicker[size]; } };
			maxSize.SelectedIndexChanged += (sender, args) => {
				if (maxSize.SelectedIndex != -1) { string size = maxSize.Items[maxSize.SelectedIndex]; newEvent.MaxSize = sizePicker[size]; } };


			launchButton.Clicked += (sender, e) =>
			{
				if(newEvent.Title != null && newEvent.Description != null) {
					//App.coreView.setContentView(new InspectEvent(newEvent, 2), 0);
					App.coreView.setContentView(new ManageEvent(), 2);
					LaunchEvent(newEvent); 
				}
			};


			locationButton.Clicked += (sender, e) =>
			{
				MapsView mapView = new MapsView (utilityManager.getCurrentUserPosition());
				mapView.createEventView = this;
				App.coreView.setContentView (mapView, 0);
			};

			fest.Clicked += (sender, e) => { fest = typeButtonPressed(fest); };
			sport.Clicked += (sender, e) => { sport = typeButtonPressed(sport); };
			kultur.Clicked += (sender, e) => { kultur = typeButtonPressed(kultur); };
			film.Clicked += (sender, e) => { film = typeButtonPressed(film); };
			musik.Clicked += (sender, e) => { musik = typeButtonPressed(musik); };
			cafe.Clicked += (sender, e) => { cafe = typeButtonPressed(cafe); };
			mad.Clicked += (sender, e) => { mad = typeButtonPressed(mad); };
			hobby.Clicked += (sender, e) => { hobby = typeButtonPressed(hobby); };
		}
			
		private async void LaunchEvent(Event eventToCreate)
		{
			EventType eventType1 = new EventType{ EventTypeId=2, Type="Outdoor"};
			List<EventType> EventTypes = new List<EventType>();
			EventTypes.Add (eventType1);

			DataManager dataManager = new DataManager();
			EventDBO newEventAsDBO = new EventDBO{OwnerId = eventToCreate.OwnerId, Title = eventToCreate.Title, 
				Description = eventToCreate.Description,
				StartDate = eventToCreate.StartDate, EndDate = eventToCreate.EndDate, MinAge = eventToCreate.MinAge,
				MaxAge = eventToCreate.MaxAge, MinSize = eventToCreate.MinSize, MaxSize = eventToCreate.MaxSize, 
				Public = true, Latitude = eventToCreate.Latitude, Longitude = eventToCreate.Longitude, 
				EventTypes = EventTypes};
			//await dataManager.CreateEvent (eventToCreate);
			await dataManager.CreateEvent (newEventAsDBO);
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

		public async void goToMapView()
		{

		}
	}
}

