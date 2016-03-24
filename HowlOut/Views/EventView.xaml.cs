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
		int currentView = 0;

		public EventView ()
		{
			InitializeComponent ();
			_dataManager = new DataManager();

			//SearchEventList.RowHeight = (int) ((0.524 * App.coreView.Width) + 32);

			setViewDesign(0);

			SearchButton.Clicked += (sender, e) => {
				setViewDesign(0);
			};
			ManageButton.Clicked += (sender, e) => {
				setViewDesign(1);
			};
			YoursButton.Clicked += (sender, e) => {
				setViewDesign(2);
			};
			FollowedButton.Clicked += (sender, e) => {
				setViewDesign(3);
			};
			InviteButton.Clicked += (sender, e) => {
				setViewDesign(4);
			};

			var createImage = new TapGestureRecognizer();
			createImage.Tapped += async (sender, e) => 
			{
				await CreateImage.ScaleTo(0.7, 50, Easing.Linear);
				await Task.Delay(60);
				await CreateImage.ScaleTo(1, 50, Easing.Linear);
				if(currentView == 0) {
					App.coreView.setContentView(new FilterSearch(App.userProfile.SearchReference), "FilterSearch");
				} else {
					App.coreView.setContentView(new CreateEvent(new Event(), true), "CreateEvent"); 
				}
			};
			CreateImage.GestureRecognizers.Add(createImage); 



		}
			
		private async void GoToSelectedEvent(string eveID) {
			Event eve = await _dataManager.EventApiManager.GetEventById(eveID);
			App.coreView.setContentView (new InspectController (null, null, eve), "UserProfile");
		}

		public async void UpdateManageList(int listToUpdate){
			ObservableCollection<Event> evelist = new ObservableCollection<Event>();
			currentView = listToUpdate;
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

			while(EventListTest.Children.Count != 0) {
				EventListTest.Children.RemoveAt(0);
			}
			var dateTimeMonth = DateTime.Now + new TimeSpan (-32, 0, 0, 0);
			int month = dateTimeMonth.Month;
			for (int i = 0; i < orderedList.Count; i++) {
				if (month != orderedList [i].StartDate.Month) {
					EventListTest.Children.Add (new Label () {
						Text = ("  " + orderedList [i].StartDate.ToString ("MMMM")),
						BackgroundColor = Color.FromHex ("cccccc"),
						TextColor = Color.White,
						FontSize = 25,
						HeightRequest = 40,
						VerticalTextAlignment = TextAlignment.Center
					});
					month = orderedList [i].StartDate.Month;

				}
				if (listToUpdate == 0) { EventListTest.Children.Add (new SearchEventTemplate (orderedList [i]));
				} else if (listToUpdate == 1) { EventListTest.Children.Add (new ManageEventTemplate (new EventForLists (orderedList [i])));
				} else if (listToUpdate == 2) { EventListTest.Children.Add (new ManageEventTemplate (new EventForLists (orderedList [i])));
				} else if (listToUpdate == 3) { EventListTest.Children.Add (new ManageEventTemplate (new EventForLists (orderedList [i])));
				} else if (listToUpdate == 4) { EventListTest.Children.Add (new MessageView (orderedList [i],MessageView.MessageType.Invite));
				}
			}
			EventListTest.Children.Add (new BoxView(){HeightRequest=120});
		}

		private void setViewDesign(int number){
			UpdateManageList(number);
			/*
			ManageEventList.IsVisible = false;
			YourEventList.IsVisible = false;
			InviteEventList.IsVisible = false;
			SearchEventList.IsVisible = false;
			FollowedEventList.IsVisible = false;
			*/
			//CreateButton.IsVisible = false;
			//CreateImage.IsVisible = false;
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
				CreateImage.Source = "ic_search.png";
				SearchButton.FontAttributes = FontAttributes.Bold;
				searchLine.IsVisible = false;
			} else if (number == 1) {
				CreateImage.Source = "ic_add.png";
				ManageButton.FontAttributes = FontAttributes.Bold;
				manageLine.IsVisible = false;
			} else if (number == 2) {
				CreateImage.Source = "ic_add.png";
				YoursButton.FontAttributes = FontAttributes.Bold;
				yoursLine.IsVisible = false;
			} else if (number == 3) {
				CreateImage.Source = "ic_add.png";
				FollowedButton.FontAttributes = FontAttributes.Bold;
				followLine.IsVisible = false;
			} else if (number == 4) {
				CreateImage.Source = "ic_add.png";
				InviteButton.FontAttributes = FontAttributes.Bold;
				inviteLine.IsVisible = false;
			}
		}
	}
}

