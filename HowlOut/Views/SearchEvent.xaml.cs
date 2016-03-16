using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class SearchEvent : ContentView
	{
		ObservableCollection<EventForLists> listEvents = new ObservableCollection<EventForLists>();
		private DataManager _dataManager;

		public SearchEvent ()
		{
			InitializeComponent ();
			_dataManager = new DataManager();
			UpdateList ();
			SearchEventList.ItemSelected += OnItemSelected;
		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if(SearchEventList.SelectedItem == null)
				return;
			var eveForLis = SearchEventList.SelectedItem as EventForLists;
			GoToSelectedEvent (eveForLis.eve.EventId);
			SearchEventList.SelectedItem = null;
		}

		private async void GoToSelectedEvent(string eveID) {
			Event eve = await _dataManager.EventApiManager.GetEventById(eveID);
			App.coreView.setContentView (new UserProfile (null, null, eve), "UserProfile");
		}

		public async void UpdateList(){
			ObservableCollection<Event> evelist = await _dataManager.EventApiManager.SearchEvents (App.userProfile.ProfileId, App.lastKnownPosition.Latitude, App.lastKnownPosition.Longitude);
			listEvents.Clear ();
			for (int i = 0; i < evelist.Count; i++) {
				EventForLists EveForLis = new EventForLists (evelist [i]);
				listEvents.Add (EveForLis);
			}
			SearchEventList.ItemsSource = listEvents;
		}
	}
}

