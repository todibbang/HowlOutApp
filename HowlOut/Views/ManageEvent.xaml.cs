using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class ManageEvent : ContentView
	{
		ObservableCollection<EventForLists> listEvents = new ObservableCollection<EventForLists>();

		public ManageEvent ()
		{
			InitializeComponent ();
			//GetEventsMatchingOwner (ManageEventList);
			ManageEventList.ItemSelected += OnItemSelected;
		}
			
		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if(ManageEventList.SelectedItem == null)
				return;
			var eveForLis = ManageEventList.SelectedItem as EventForLists;
			App.coreView.setContentView(new InspectEvent(eveForLis.eve, 2), "InspectEvent");
			ManageEventList.SelectedItem = null;
		}

		public void updateList(){
			var eve = App.coreView.manageEventList;
			listEvents.Clear ();
			for (int i = 0; i < eve.Count; i++) {
				EventForLists EveForLis = new EventForLists (eve [i]);
				listEvents.Add (EveForLis);
			}
			ManageEventList.ItemsSource = listEvents;
		}
	}
}

