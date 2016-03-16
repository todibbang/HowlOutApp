using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class ManageEvent : ContentView
	{
		ObservableCollection<EventForLists> listEvents = new ObservableCollection<EventForLists>();
		private DataManager _dataManager;

		public ManageEvent ()
		{
			InitializeComponent ();
			_dataManager = new DataManager();
			UpdateList ();
			ManageEventList.ItemSelected += OnItemSelected;
		}
			
		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if(ManageEventList.SelectedItem == null)
				return;
			var eveForLis = ManageEventList.SelectedItem as EventForLists;
			GoToSelectedEvent (eveForLis.eve.EventId);
			ManageEventList.SelectedItem = null;
		}

		private async void GoToSelectedEvent(string eveID) {
			Event eve = await _dataManager.EventApiManager.GetEventById(eveID);
			App.coreView.setContentView (new UserProfile (null, null, eve), "UserProfile");
		}

		public async void UpdateList(){
			ObservableCollection<Event> evelist = await _dataManager.EventApiManager.GetEventsWithOwnerId ();
			listEvents.Clear ();
			for (int i = 0; i < evelist.Count; i++) {
				EventForLists EveForLis = new EventForLists (evelist [i]);
				listEvents.Add (EveForLis);
			}
			ManageEventList.ItemsSource = listEvents;
		}
	}
}

