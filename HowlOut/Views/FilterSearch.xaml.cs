using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class FilterSearch : ContentView
	{	
		ObservableCollection<EventType> EventTypes = new ObservableCollection<EventType>();

		public FilterSearch ()
		{
			InitializeComponent ();

			distance.Minimum = 0;
			distance.Maximum = 30;
			distance.Value = 5;
			distanceLabel.Text = "Distance, " + distance.Value + " km";

			distance.ValueChanged += (sender, e) => {
				distanceLabel.Text = "Distance, " + ((int) distance.Value + " km");
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

		private Button typeButtonPressed(Button typeButton)
		{

			System.Diagnostics.Debug.WriteLine ("Color shit");
			if (typeButton.BackgroundColor == Color.White) {
				typeButton.BackgroundColor = Color.FromHex ("00E0A0");
				typeButton.TextColor = Color.White;
				EventTypes.Add (new EventType{Type = typeButton.Text.ToString ()});
			} else {
				typeButton.BackgroundColor = Color.White;
				typeButton.TextColor = Color.FromHex ("00E0A0");
				for (int i = 0; i < EventTypes.Count; i++) {
					if(EventTypes[i].Type == typeButton.Text.ToString ())EventTypes.Remove (EventTypes[i]);
				}
			}
			return typeButton;
		}
	}
}

