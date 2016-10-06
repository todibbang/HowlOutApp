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
		List<Button> typeButtons = new List<Button>();

		public FilterSearch (SearchSettings userSearchSettings)
		{
			InitializeComponent ();
			newSearchSettings = userSearchSettings;
			_dataManager = new DataManager ();

			distance.Minimum = 0;
			distance.Maximum = 1000;
			distance.Value = (int) newSearchSettings.Distance;
			distanceLabel.Text = "Distance " + ((int) distance.Value) + " km";

			distance.ValueChanged += (sender, e) => {
				distanceLabel.Text = "Distance " + ((int) distance.Value + " km");
				newSearchSettings.Distance = distance.Value;
			};

			int row = 0;
			int column = 1;
			eventTypeGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			foreach (EventType en in Enum.GetValues(typeof(EventType)))
			{
				if (column == 9)
				{
					eventTypeGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
					column = 1;
					row++;
				}

				Button b = new Button()
				{
					TextColor = App.HowlOut,
					BorderRadius = 25,
					BorderWidth = 1,
					HeightRequest = 50,
					WidthRequest = 50,
					BorderColor = App.HowlOutFade,
					BackgroundColor = Color.White,
					FontSize = 12,
					Text = en.ToString(),
				};
				typeButtons.Add(b);
				eventTypeGrid.Children.Add(b, column, row);
				b.Clicked += (sender, e) =>
				{
					typeButtonPressed(b, en);
				};

				if (newSearchSettings.EventTypes.Contains(en))
				{
					b.BackgroundColor = App.HowlOut;
					b.TextColor = Color.White;
				}
				column += 2;
			}

			/*


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
			*/
			updateButton.Clicked += (sender, e) => {
				UpdateSearch();
			};
		}

		private async void UpdateSearch()
		{
			await _dataManager.ProfileApiManager.SetSearchSettings (App.userProfile.ProfileId, newSearchSettings);
			App.userProfile = await _dataManager.ProfileApiManager.GetLoggedInProfile (App.StoredUserFacebookId);
			App.coreView.returnToPreviousView();
		}

		//TODO cleaned up this part, changed to enum
		private Button typeButtonPressed(Button typeButton, EventType eventType)
		{
			if (typeButton.BackgroundColor == Color.White) {
				typeButton.BackgroundColor = App.HowlOut;
				typeButton.TextColor = Color.White;
				newSearchSettings.EventTypes.Add (eventType);
			} else {
				typeButton.BackgroundColor = Color.White;
				typeButton.TextColor = App.HowlOut;
				newSearchSettings.EventTypes.Remove (eventType);
			}
			return typeButton;
		}

	}
}

