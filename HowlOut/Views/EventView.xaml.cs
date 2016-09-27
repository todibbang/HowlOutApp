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
		private StandardButton standardButton = new StandardButton();
		bool beingRepositioned = false;
		int currentView = 0;

		int EventViewType;

		public EventView (int viewType)
		{
			InitializeComponent ();
			_dataManager = new DataManager();
			EventViewType = viewType;


			/*
			SearchButtonLayout.Children.Add(standardButton.StandardButtonGrid (StandardButton.StandardButtonType.Plain, "Search",0));
			ManageButtonLayout.Children.Add(standardButton.StandardButtonGrid (StandardButton.StandardButtonType.Plain, "Attending",0));
			FollowButtonLayout.Children.Add(standardButton.StandardButtonGrid (StandardButton.StandardButtonType.Plain, "Following",0));
			InviteButtonLayout.Children.Add(standardButton.StandardButtonGrid (StandardButton.StandardButtonType.Plain, "Invited",0));
			*/
			setViewDesign(viewType, searchList);

			App.coreView.topBar.getOptionButtons()[0].Clicked += (sender, e) =>
			{
				if (EventViewType == 1)
				{
					setViewDesign(1, searchList);
				} else if (EventViewType == 2)
				{
					setViewDesign(2, searchList);
				}
			};
			App.coreView.topBar.getOptionButtons()[1].Clicked += (sender, e) =>
			{
				if (EventViewType == 1)
				{
					setViewDesign(3, searchList);
				}else if (EventViewType == 2)
				{
					setViewDesign(2, searchList);
				}
			};

		}

		void OnPanUpdated (object sender, PanUpdatedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine (e.TotalX + " " + e.TotalX);
		}
			
		private async void GoToSelectedEvent(string eveID) {
			Event eve = await _dataManager.EventApiManager.GetEventById(eveID);
			App.coreView.setContentViewWithQueue (new InspectController (null, null, eve), "UserProfile");
		}

		private void setViewDesign(int number, StackLayout list){
			loading.IsVisible = true;
			UpdateManageList(number, list);
			/*
			SearchButton.FontAttributes = FontAttributes.None;
			ManageButton.FontAttributes = FontAttributes.None;
			InviteButton.FontAttributes = FontAttributes.None;
			FollowedButton.FontAttributes = FontAttributes.None;
			searchLine.IsVisible = true;
			manageLine.IsVisible = true;
			inviteLine.IsVisible = true;
			followLine.IsVisible = true;

			if (number == 0) {
				SearchButton.FontAttributes = FontAttributes.Bold;
				searchLine.IsVisible = false;
			} else if (number == 1) {
				ManageButton.FontAttributes = FontAttributes.Bold;
				manageLine.IsVisible = false;
			} else if (number == 3) {
				FollowedButton.FontAttributes = FontAttributes.Bold;
				followLine.IsVisible = false;
			} else if (number == 4) {
				InviteButton.FontAttributes = FontAttributes.Bold;
				inviteLine.IsVisible = false;
			}
			*/
		}

		public async void UpdateManageList(int listToUpdate, StackLayout list){

			while(list.Children.Count != 0) {
				list.Children.RemoveAt(0);
			}
			ObservableCollection<Event> evelist = new ObservableCollection<Event>();
			currentView = listToUpdate;
			var first = DateTime.Now;
			if (listToUpdate == 0) {
				evelist = await _dataManager.EventApiManager.SearchEvents ();

			} else if (listToUpdate == 1) {
				evelist = await _dataManager.EventApiManager.GetEventsWithOwnerId ();

			} else if (listToUpdate == 3) {
				evelist = await _dataManager.ProfileApiManager.GetEventsFollowed ();

			} else if (listToUpdate == 2) {
				evelist = await _dataManager.ProfileApiManager.GetEventsInvitedTo ();
				var evesAttended = await _dataManager.EventApiManager.GetEventsWithOwnerId ();
				for (int i = evelist.Count -1; i > -1; i--) {
					for (int m = 0; m < evesAttended.Count; m++) {
						if (evelist [i].EventId == evesAttended[m].EventId) {
							evelist.RemoveAt (i);
							break;
						}
					}
				}
			}
			var second = DateTime.Now;
			var time = second - first;
			System.Diagnostics.Debug.WriteLine ("Time to load: " + (time.Milliseconds) + " ms");

			var orderedList = new ObservableCollection<Event>();
			Event itemToAdd = new Event();
			while(evelist.Count != 0){
				DateTime Time = evelist [0].StartDate;
				itemToAdd = evelist [0];

				for (int i = 0; i < evelist.Count; i++) {
					if (evelist [i].StartDate < Time) {
						itemToAdd = evelist [i];
						Time = itemToAdd.StartDate;
					}
				}
				orderedList.Add (itemToAdd);
				evelist.Remove (itemToAdd);
			}


			var dateTimeMonth = DateTime.Now + new TimeSpan (-32, 0, 0, 0);
			int month = dateTimeMonth.Month;
			for (int i = 0; i < orderedList.Count; i++) {
				if (month != orderedList [i].StartDate.Month) {
					list.Children.Add (
						new Label () {
						Text = ("  " + orderedList [i].StartDate.ToString ("MMMM")),
						//BackgroundColor = Color.FromHex ("cccccc"),
						TextColor = Color.FromHex("df7a7a"),
						FontSize = 25,
						HeightRequest = 40,
						VerticalTextAlignment = TextAlignment.Center
					});
					month = orderedList [i].StartDate.Month;

				}
				if (listToUpdate == 0) { list.Children.Add (new SearchEventTemplate (orderedList [i]));
				} else if (listToUpdate == 1) { list.Children.Add (new ManageEventTemplate (orderedList [i]));
				} else if (listToUpdate == 3) { list.Children.Add (new ManageEventTemplate (orderedList [i]));
				} else if (listToUpdate == 4) { list.Children.Add (new NewsMessageView (orderedList [i],NewsMessageView.MessageType.Invite));
				}
			}
			list.Children.Add (new BoxView(){HeightRequest=120});
			loading.IsVisible = false;
		}


	}
}

