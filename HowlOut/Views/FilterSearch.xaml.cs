using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class FilterSearch : ContentView
	{	
		ObservableCollection<EventType> EventTypes = new ObservableCollection<EventType>();

		public FilterSearch (SearchSettings userSearchSettings)
		{
			InitializeComponent ();

			distance.Minimum = 0;
			distance.Maximum = 30;
			distance.Value = userSearchSettings.Distance;
			distanceLabel.Text = "Distance, " + distance.Value + " km";

			distance.ValueChanged += (sender, e) => {
				distanceLabel.Text = "Distance, " + ((int) distance.Value + " km");
			};

			if(userSearchSettings.EventTypes.Contains(EventType.Party)) { fest = typeButtonPressed(fest, EventType.Party); };
			if(userSearchSettings.EventTypes.Contains(EventType.Sport)) { sport = typeButtonPressed(sport, EventType.Sport); };
			if(userSearchSettings.EventTypes.Contains(EventType.Culture)) { kultur = typeButtonPressed(kultur, EventType.Culture); };
			if(userSearchSettings.EventTypes.Contains(EventType.Movie)) { film = typeButtonPressed(film, EventType.Movie); };
			if(userSearchSettings.EventTypes.Contains(EventType.Music)) { musik = typeButtonPressed(musik, EventType.Music); };
			if(userSearchSettings.EventTypes.Contains(EventType.Cafe)) { cafe = typeButtonPressed(cafe, EventType.Cafe); };
			if(userSearchSettings.EventTypes.Contains(EventType.Food)) { mad = typeButtonPressed(mad, EventType.Food); };
			if(userSearchSettings.EventTypes.Contains(EventType.Hobby)) { hobby = typeButtonPressed(hobby, EventType.Hobby); };

			fest.Clicked += (sender, e) => { fest = typeButtonPressed(fest, EventType.Party); };
			sport.Clicked += (sender, e) => { sport = typeButtonPressed(sport, EventType.Sport); };
			kultur.Clicked += (sender, e) => { kultur = typeButtonPressed(kultur, EventType.Culture); };
			film.Clicked += (sender, e) => { film = typeButtonPressed(film, EventType.Movie); };
			musik.Clicked += (sender, e) => { musik = typeButtonPressed(musik, EventType.Music); };
			cafe.Clicked += (sender, e) => { cafe = typeButtonPressed(cafe, EventType.Cafe); };
			mad.Clicked += (sender, e) => { mad = typeButtonPressed(mad, EventType.Food); };
			hobby.Clicked += (sender, e) => { hobby = typeButtonPressed(hobby, EventType.Hobby); };

		}

		//TODO cleaned up this part, changed to enum
		private Button typeButtonPressed(Button typeButton, EventType eventType)
		{

			System.Diagnostics.Debug.WriteLine ("Color shit");
			if (typeButton.BackgroundColor == Color.White) {
				typeButton.BackgroundColor = Color.FromHex ("00E0A0");
				typeButton.TextColor = Color.White;
				EventTypes.Add (eventType);
			} else {
				typeButton.BackgroundColor = Color.White;
				typeButton.TextColor = Color.FromHex ("00E0A0");
				EventTypes.Remove (eventType);
			}
			return typeButton;
		}
	}
}

