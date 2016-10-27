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

		public FilterSearch(SearchSettings userSearchSettings)
		{
			InitializeComponent();
			newSearchSettings = userSearchSettings;
			_dataManager = new DataManager();

			if(newSearchSettings != null) {


				distance.Minimum = 0;
				distance.Maximum = 1000;
				distance.Value = (int)newSearchSettings.Distance;
				distanceLabel.Text = "Distance " + ((int)distance.Value) + " km";

				distance.ValueChanged += (sender, e) =>
				{
					distanceLabel.Text = "Distance " + ((int)distance.Value + " km");
					newSearchSettings.Distance = distance.Value;
				};



				EventCategory.ManageCategories(eventTypeGrid, newSearchSettings.EventTypes, false);
				/*
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

					bool selected = false;
					if (newSearchSettings.EventTypes.Contains(en)) { selected = true; }
					EventCategoryButton b = new EventCategoryButton(en, selected, 60, null);
					//typeButtons.Add(b);
					eventTypeGrid.Children.Add(b, column, row);
					b.button.Clicked += (sender, e) =>
					{
						if (newSearchSettings.EventTypes.Contains(en)) { 
							newSearchSettings.EventTypes.Remove(en);
						} else { 
							newSearchSettings.EventTypes.Add(en);
						}
					};
					column += 2;
				}
				*/

				updateButton.Clicked += (sender, e) =>
				{
					UpdateSearch();
				};
			}
		}

		private async void UpdateSearch()
		{
			await _dataManager.ProfileApiManager.updateSearchSettings (newSearchSettings);
			App.userProfile = await _dataManager.ProfileApiManager.GetLoggedInProfile ();
			App.coreView.returnToPreviousView();
		}
	}
}

