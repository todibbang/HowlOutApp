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

		Button exploreButton = new Button { Text = "Explore", BackgroundColor = Color.Transparent, HorizontalOptions = LayoutOptions.Fill, TextColor = App.HowlOut, FontSize = 16 };
		Button friendsButton = new Button { Text = "Friends", BackgroundColor = Color.Transparent, HorizontalOptions = LayoutOptions.Fill, TextColor = App.HowlOut, FontSize = 16 };
		Button joinedButton = new Button { Text = "Joined", BackgroundColor = Color.Transparent, HorizontalOptions = LayoutOptions.Fill, TextColor = App.HowlOut, FontSize = 16 };
		Button trackedButton = new Button { Text = "Tracked", BackgroundColor = Color.Transparent, HorizontalOptions = LayoutOptions.Fill, TextColor = App.HowlOut, FontSize = 16 };


		int EventViewType;

		public EventView (int viewType)
		{
			InitializeComponent ();
			_dataManager = new DataManager();
			EventViewType = viewType;

			if (EventViewType == 1)
			{
				App.setOptionsGrid(optionGrid, new List<Button> { exploreButton, friendsButton }, new List<VisualElement> { searchEventList, searchEventList });
				setViewDesign(0);
			}
			else if (EventViewType != 10)
			{
				App.setOptionsGrid(optionGrid, new List<Button> { joinedButton, trackedButton }, new List<VisualElement> { manageEventList, manageEventList });
				setViewDesign(1);
			}
			else {
				setViewDesign(1);
			}

			manageEventList.ItemSelected += OnItemSelected;
			searchEventList.ItemSelected += OnItemSelected;
			/*
			SearchButtonLayout.Children.Add(standardButton.StandardButtonGrid (StandardButton.StandardButtonType.Plain, "Search",0));
			ManageButtonLayout.Children.Add(standardButton.StandardButtonGrid (StandardButton.StandardButtonType.Plain, "Attending",0));
			FollowButtonLayout.Children.Add(standardButton.StandardButtonGrid (StandardButton.StandardButtonType.Plain, "Following",0));
			InviteButtonLayout.Children.Add(standardButton.StandardButtonGrid (StandardButton.StandardButtonType.Plain, "Invited",0));
			*/


			exploreButton.Clicked += (sender, e) => { setViewDesign(0); };
			friendsButton.Clicked += (sender, e) => { setViewDesign(0); };
			joinedButton.Clicked += (sender, e) => { setViewDesign(1); };
			trackedButton.Clicked += (sender, e) => { setViewDesign(3); };
		}

		public async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			ListView list = null;
			if (manageEventList.SelectedItem != null)
			{
				list = manageEventList;
			}
			else if (searchEventList.SelectedItem != null)
			{
				list = searchEventList;
			}
			else return;

			var selectedEvent = list.SelectedItem as EventForLists;

			InspectController inspect = new InspectController(null, null, selectedEvent.eve);
			App.coreView.setContentViewWithQueue(inspect, "UserProfile", inspect.getScrollView());
			list.SelectedItem = null;

			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
		}

		void OnPanUpdated (object sender, PanUpdatedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine (e.TotalX + " " + e.TotalX);
		}
			
		private async void GoToSelectedEvent(string eveID) {
			Event eve = await _dataManager.EventApiManager.GetEventById(eveID);

			InspectController inspect = new InspectController(null, null, eve);
			App.coreView.setContentViewWithQueue(inspect, "UserProfile", inspect.getScrollView());

		}

		private void setViewDesign(int number){
			loading.IsVisible = true;
			UpdateManageList(number);
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

		public async void UpdateManageList(int listToUpdate){
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
					/*
					list.Children.Add (
						new Label () {
						Text = ("  " + orderedList [i].StartDate.ToString ("MMMM")),
						//BackgroundColor = Color.FromHex ("cccccc"),
						TextColor = App.HowlOut,
						FontSize = 25,
						HeightRequest = 40,
						VerticalTextAlignment = TextAlignment.Center
					});
					month = orderedList [i].StartDate.Month;*/
				}
				/*
				if (listToUpdate == 0) { list.Children.Add (new SearchEventTemplate (orderedList [i]));
				} else if (listToUpdate == 1) { list.Children.Add (new ManageEventTemplate (orderedList [i]));
				} else if (listToUpdate == 3) { list.Children.Add (new ManageEventTemplate (orderedList [i]));
				}
				*/
			}
			//list.Children.Add (new BoxView(){HeightRequest=120});


			//list.IsVisible = false;
			//DataTemplate st = new DataTemplate(typeof(SearchEventTemplate));
			//DataTemplate mt = new DataTemplate(typeof(ManageEventTemplate));

			var mt = new DataTemplate(() =>
			{
				return new ViewCell { View = new ManageEventTemplate() };
			});
			var st = new DataTemplate(() =>
			{
				return new ViewCell { View = new SearchEventTemplate() };
			});



			searchEventList = new ListView()
			{
				HasUnevenRows = true,
				SeparatorVisibility = SeparatorVisibility.None,
				BackgroundColor = Color.White,
				IsVisible = true,
				ItemTemplate = st,
			};

			manageEventList = new ListView()
			{
				HasUnevenRows = true,
				SeparatorVisibility = SeparatorVisibility.None,
				BackgroundColor = Color.White,
				IsVisible = true,
				ItemTemplate = mt,
			};

			contentTest.Children.Add(searchEventList, 0, 1);
			contentTest.Children.Add(manageEventList, 0, 1);

			List<EventForLists> eveFL = new List<EventForLists>();
			foreach (Event eve in orderedList)
			{
				eveFL.Add(new EventForLists(eve));
			}
			searchEventList.ItemsSource = null;
			manageEventList.ItemsSource = null;
			if (listToUpdate == 0)
			{
				searchEventList.ItemsSource = eveFL;
				searchEventList.IsVisible = true;
				manageEventList.IsVisible = false;
			}
			else {
				manageEventList.ItemsSource = eveFL;
				manageEventList.IsVisible = true;
				searchEventList.IsVisible = false;
			}


			loading.IsVisible = false;
		}

			/*
		public ScrollView getScrollView()
		{
			return scrollView;
		}

*/
	}
}

