using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public partial class FilterSearch : ContentView, ViewModelInterface
	{	
		SearchSettings newSearchSettings;
		DataManager _dataManager;
		List<Button> typeButtons = new List<Button>();

		public FilterSearch(SearchSettings userSearchSettings)
		{
			InitializeComponent();

			try
			{

				newSearchSettings = userSearchSettings;
				_dataManager = new DataManager();

				if (newSearchSettings != null)
				{


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

					updateButton.Clicked += (sender, e) =>
					{
						UpdateSearch();
					};
				}
			}
			catch (Exception e) { }
		}

		public void viewInFocus(UpperBar bar)
		{
			
		}

		public void reloadView()
		{

		}

		public void viewExitFocus() {  }

		public ContentView getContentView() { return this; }

		private async void UpdateSearch()
		{
			await _dataManager.ProfileApiManager.updateSearchSettings (newSearchSettings);
			App.userProfile = await _dataManager.ProfileApiManager.GetLoggedInProfile ();
			App.coreView.returnToPreviousView();
		}
	}
}

