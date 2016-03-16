using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class FilterSearch : ContentView
	{	
		SearchSettings newSearchSettings;
		DataManager _dataManager;

		public FilterSearch (SearchSettings userSearchSettings)
		{
			InitializeComponent ();
			newSearchSettings = userSearchSettings;
			_dataManager = new DataManager ();

			distance.Minimum = 0;
			distance.Maximum = 30;
			distance.Value = newSearchSettings.Distance;
			distanceLabel.Text = "Distance, " + distance.Value + " km";

			distance.ValueChanged += (sender, e) => {
				distanceLabel.Text = "Distance, " + ((int) distance.Value + " km");
			};

			if(newSearchSettings.EventTypes.Contains(EventType.Party)) { fest = PreSetButton(fest, EventType.Party); };
			if(newSearchSettings.EventTypes.Contains(EventType.Sport)) { sport = PreSetButton(sport, EventType.Sport); };
			if(newSearchSettings.EventTypes.Contains(EventType.Culture)) { kultur = PreSetButton(kultur, EventType.Culture); };
			if(newSearchSettings.EventTypes.Contains(EventType.Movie)) { film = PreSetButton(film, EventType.Movie); };
			if(newSearchSettings.EventTypes.Contains(EventType.Music)) { musik = PreSetButton(musik, EventType.Music); };
			if(newSearchSettings.EventTypes.Contains(EventType.Cafe)) { cafe = PreSetButton(cafe, EventType.Cafe); };
			if(newSearchSettings.EventTypes.Contains(EventType.Food)) { mad = PreSetButton(mad, EventType.Food); };
			if(newSearchSettings.EventTypes.Contains(EventType.Hobby)) { hobby = PreSetButton(hobby, EventType.Hobby); };

			fest.Clicked += (sender, e) => { fest = typeButtonPressed(fest, EventType.Party); };
			sport.Clicked += (sender, e) => { sport = typeButtonPressed(sport, EventType.Sport); };
			kultur.Clicked += (sender, e) => { kultur = typeButtonPressed(kultur, EventType.Culture); };
			film.Clicked += (sender, e) => { film = typeButtonPressed(film, EventType.Movie); };
			musik.Clicked += (sender, e) => { musik = typeButtonPressed(musik, EventType.Music); };
			cafe.Clicked += (sender, e) => { cafe = typeButtonPressed(cafe, EventType.Cafe); };
			mad.Clicked += (sender, e) => { mad = typeButtonPressed(mad, EventType.Food); };
			hobby.Clicked += (sender, e) => { hobby = typeButtonPressed(hobby, EventType.Hobby); };

			updateButton.Clicked += (sender, e) => {
				UpdateSearch();
			};
		}

		private async void UpdateSearch()
		{
			await _dataManager.ProfileApiManager.SetSearchSettings (App.userProfile.ProfileId, newSearchSettings);
			App.coreView.setContentView (new SearchEvent (), "SearchEvent");
		}

		//TODO cleaned up this part, changed to enum
		private Button typeButtonPressed(Button typeButton, EventType eventType)
		{
			if (typeButton.BackgroundColor == Color.White) {
				typeButton.BackgroundColor = Color.FromHex ("00E0A0");
				typeButton.TextColor = Color.White;
				newSearchSettings.EventTypes.Add (eventType);
			} else {
				typeButton.BackgroundColor = Color.White;
				typeButton.TextColor = Color.FromHex ("00E0A0");
				newSearchSettings.EventTypes.Remove (eventType);
			}
			return typeButton;
		}

		private Button PreSetButton(Button typeButton, EventType eventType)
		{
			System.Diagnostics.Debug.WriteLine ("1234567890");

			typeButton.BackgroundColor = Color.FromHex ("00E0A0");
			typeButton.TextColor = Color.White;

			return typeButton;
		}
	}
}

