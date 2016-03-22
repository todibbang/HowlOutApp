using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class EventView : ContentView
	{
		private DataManager _dataManager;

		public EventView ()
		{
			InitializeComponent ();
			_dataManager = new DataManager();
			setViewDesign(0, SearchEventList);

			SearchButton.Clicked += (sender, e) => {
				setViewDesign(0, SearchEventList);
			};
			ManageButton.Clicked += (sender, e) => {
				setViewDesign(1, ManageEventList);
			};
			YoursButton.Clicked += (sender, e) => {
				setViewDesign(2, YourEventList);
			};
			FollowedButton.Clicked += (sender, e) => {
				setViewDesign(3, FollowedEventList);
			};
			InviteButton.Clicked += (sender, e) => {
				setViewDesign(4, InviteEventList);
			};

			ManageEventList.ItemSelected += OnManageItemSelected;
			SearchEventList.ItemSelected += OnSearchItemSelected;
			YourEventList.ItemSelected += OnYourItemSelected;
			FollowedEventList.ItemSelected += OnFollowedItemSelected;
			InviteEventList.ItemSelected += OnInviteItemSelected;

			var createImage = new TapGestureRecognizer();
			createImage.Tapped += async (sender, e) => 
			{
				await CreateImage.ScaleTo(0.7, 50, Easing.Linear);
				await Task.Delay(60);
				await CreateImage.ScaleTo(1, 50, Easing.Linear);
				if(SearchEventList.IsVisible) {
					App.coreView.setContentView(new FilterSearch(App.userProfile.SearchReference), "FilterSearch");
				}
				if(ManageEventList.IsVisible) {
					App.coreView.setContentView(new CreateEvent(new Event(), true), "CreateEvent"); 
				}
			};
			CreateImage.GestureRecognizers.Add(createImage); 
		}

		void OnManageItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if(ManageEventList.SelectedItem == null)
				return;
			var eveForLis = ManageEventList.SelectedItem as EventForLists;
			GoToSelectedEvent (eveForLis.eve.EventId);
			ManageEventList.SelectedItem = null;
		}

		void OnSearchItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if(SearchEventList.SelectedItem == null)
				return;
			var eveForLis = SearchEventList.SelectedItem as EventForLists;
			GoToSelectedEvent (eveForLis.eve.EventId);
			SearchEventList.SelectedItem = null;
		}

		void OnYourItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if(YourEventList.SelectedItem == null)
				return;
			var eveForLis = YourEventList.SelectedItem as EventForLists;
			GoToSelectedEvent (eveForLis.eve.EventId);
			YourEventList.SelectedItem = null;
		}

		void OnFollowedItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if(FollowedEventList.SelectedItem == null)
				return;
			var eveForLis = FollowedEventList.SelectedItem as EventForLists;
			GoToSelectedEvent (eveForLis.eve.EventId);
			FollowedEventList.SelectedItem = null;
		}

		void OnInviteItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if(InviteEventList.SelectedItem == null)
				return;
			var eveForLis = InviteEventList.SelectedItem as EventForLists;
			GoToSelectedEvent (eveForLis.eve.EventId);
			InviteEventList.SelectedItem = null;
		}

		private async void GoToSelectedEvent(string eveID) {
			Event eve = await _dataManager.EventApiManager.GetEventById(eveID);
			App.coreView.setContentView (new InspectController (null, null, eve), "UserProfile");
		}

		public async void UpdateManageList(int listToUpdate, ListView theListToUpdate){
			ObservableCollection<Event> evelist = new ObservableCollection<Event>();
			ObservableCollection<EventForLists> listEvents = new ObservableCollection<EventForLists>();
			var first = DateTime.Now;
			if (listToUpdate == 0) {
				evelist = await _dataManager.EventApiManager.SearchEvents (App.userProfile.ProfileId, App.lastKnownPosition.Latitude, App.lastKnownPosition.Longitude);
			} else if (listToUpdate == 1) {
				evelist = await _dataManager.EventApiManager.GetEventsWithOwnerId ();
			} else if (listToUpdate == 2) {
				evelist = await _dataManager.EventApiManager.GetEventsWithOwnerId ();
				for (int i = evelist.Count -1; i > -1; i--) {
					if (evelist [i].Owner.ProfileId != App.userProfile.ProfileId) {
						evelist.RemoveAt (i);
					}
				}
			} else if (listToUpdate == 3) {
				evelist = await _dataManager.ProfileApiManager.GetEventsInvitedTo (App.userProfile.ProfileId);
			} else if (listToUpdate == 4) {
				evelist = await _dataManager.ProfileApiManager.GetEventsInvitedTo (App.userProfile.ProfileId);
			}
			var second = DateTime.Now;
			var time = second - first;
			System.Diagnostics.Debug.WriteLine ("Time to load: " + (time.Milliseconds) + " ms");


			listEvents.Clear ();
			for (int i = 0; i < evelist.Count; i++) {
				EventForLists EveForLis = new EventForLists (evelist [i]);
				listEvents.Add (EveForLis);
			}
			theListToUpdate.ItemsSource = listEvents;
		}

		private void setViewDesign(int number, ListView listToShow){
			UpdateManageList(number, listToShow);
			ManageEventList.IsVisible = false;
			YourEventList.IsVisible = false;
			InviteEventList.IsVisible = false;
			SearchEventList.IsVisible = false;
			FollowedEventList.IsVisible = false;
			CreateButton.IsVisible = false;
			CreateImage.IsVisible = false;
			SearchButton.FontAttributes = FontAttributes.None;
			ManageButton.FontAttributes = FontAttributes.None;
			YoursButton.FontAttributes = FontAttributes.None;
			InviteButton.FontAttributes = FontAttributes.None;
			FollowedButton.FontAttributes = FontAttributes.None;
			searchLine.IsVisible = true;
			manageLine.IsVisible = true;
			yoursLine.IsVisible = true;
			inviteLine.IsVisible = true;
			followLine.IsVisible = true;

			if (number == 0) {
				CreateButton.IsVisible = true;
				CreateImage.IsVisible = true;
				CreateImage.Source = "ic_search.png";
				SearchEventList.IsVisible = true;
				SearchButton.FontAttributes = FontAttributes.Bold;
				searchLine.IsVisible = false;
			} else if (number == 1) {
				CreateButton.IsVisible = true;
				CreateImage.IsVisible = true;
				CreateImage.Source = "ic_add.png";
				ManageEventList.IsVisible = true;
				ManageButton.FontAttributes = FontAttributes.Bold;
				manageLine.IsVisible = false;
			} else if (number == 2) {
				YourEventList.IsVisible = true;
				YoursButton.FontAttributes = FontAttributes.Bold;
				yoursLine.IsVisible = false;
			} else if (number == 3) {
				FollowedEventList.IsVisible = true;
				FollowedButton.FontAttributes = FontAttributes.Bold;
				followLine.IsVisible = false;
			} else if (number == 4) {
				InviteEventList.IsVisible = true;
				InviteButton.FontAttributes = FontAttributes.Bold;
				inviteLine.IsVisible = false;
			}
		}
	}
}

