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
					distance.Maximum = 30;
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
						UpdateSearch(true);
					};

					setLocation.Clicked += (sender, e) =>
					{
						App.coreView.setContentViewWithQueue(new MapsView( App.lastKnownPosition,true));
					};

					useCurrentLocation.Clicked += async (sender, e) =>
					{
						App.coreView.IsLoading(true);
						App.alreadyAskedToSetPositionManually = false;
						App.storeManualPosition("","");
						await _dataManager.UtilityManager.getGeoLocation();
						mapLayout.IsVisible = false;
						mapLayout2.IsVisible = false;
						mapLayout.Children.Clear();
						App.coreView.IsLoading(false);
					};

					if (App.setPositionManually)
					{
						mapLayout.IsVisible = true;
						mapLayout2.IsVisible = true;
						mapLayout.Children.Add(new MapsView(App.lastKnownPosition, false));
					}
				}
			}
			catch (Exception e) { }
		}

		public void viewInFocus(UpperBar bar)
		{
			bar.setNavigationlabel("Search Preferences");
		}

		public void reloadView()
		{
			this.Content = new FilterSearch(App.userProfile.SearchPreference);
		}

		public void viewExitFocus() {
			UpdateSearch(false);
		}

		public ContentView getContentView() { return this; }

		private async void UpdateSearch(bool returnView)
		{
			App.coreView.IsLoading(true);
			await _dataManager.ProfileApiManager.updateSearchSettings(newSearchSettings);
			App.userProfile = await _dataManager.ProfileApiManager.GetLoggedInProfile();
			await _dataManager.UtilityManager.getGeoLocation();
			//App.coreView.exploreEventCategories.searchEventList.UpdateList(true, "");
			App.coreView.exploreEvents.UpdateList(true, "");
			if(returnView) App.coreView.returnToPreviousView();
			App.coreView.IsLoading(false);
		}
	}
}

