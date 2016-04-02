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
		bool beingRepositioned = false;
		int currentView = 0;

		public EventView ()
		{
			InitializeComponent ();
			_dataManager = new DataManager();


			//var gesture = new PanGestureRecognizer();
			//gesture.PanUpdated += OnPanUpdated;
			//contentTest.GestureRecognizers.Add(gesture);


			/*
			HorizontalStackLayout.WidthRequest = App.coreView.Width * 3;
			searchScrollView.WidthRequest = App.coreView.Width;
			manageScrollView.WidthRequest = App.coreView.Width;
			otherScrollView.WidthRequest = App.coreView.Width;

			//searchScrollView.IsEnabled = false;

			

			//SearchEventList.RowHeight = (int) ((0.524 * App.coreView.Width) + 32);

			HorizontalScrollView.Scrolled += (sender, e) => {
				if (HorizontalScrollView.ScrollX > App.coreView.Width * 1.5) {
					HorizontalScrollView.ScrollToAsync (App.coreView.Width * 2, 0, true);
				} else if (HorizontalScrollView.ScrollX > App.coreView.Width * 0.5) {
					HorizontalScrollView.ScrollToAsync (App.coreView.Width * 1, 0, true);
				} else {
					HorizontalScrollView.ScrollToAsync (0, 0, true);
				}
			};
			*/
			setViewDesign(0, searchList);
			//setViewDesign(1, manageList);
			//setViewDesign(2, otherList);


			SearchButton.Clicked += (sender, e) => {
				//HorizontalScrollView.ScrollToAsync(0, 0, true);
				setViewDesign(0, searchList);
			};
			ManageButton.Clicked += (sender, e) => {
				//HorizontalScrollView.ScrollToAsync(App.coreView.Width, 0, true);
				//setViewDesign(1, manageList);
				setViewDesign(1, searchList);
			};
			YoursButton.Clicked += (sender, e) => {
				//HorizontalScrollView.ScrollToAsync(App.coreView.Width * 2, 0, true);
				//setViewDesign(2, otherList);
				setViewDesign(2, searchList);
			};
			FollowedButton.Clicked += (sender, e) => {
				//HorizontalScrollView.ScrollToAsync(App.coreView.Width * 2, 0, true);
				//setViewDesign(3, otherList);
				setViewDesign(3, searchList);
			};
			InviteButton.Clicked += (sender, e) => {
				//HorizontalScrollView.ScrollToAsync(App.coreView.Width * 2, 0, true);
				//setViewDesign(4, otherList);
				setViewDesign(4, searchList);
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

		void OnPanUpdated (object sender, PanUpdatedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine (e.TotalX + " " + e.TotalX);
		}

			
		private async void GoToSelectedEvent(string eveID) {
			Event eve = await _dataManager.EventApiManager.GetEventById(eveID);
			App.coreView.setContentView (new InspectController (null, null, eve), "UserProfile");
		}



		private void setViewDesign(int number, StackLayout list){
			UpdateManageList(number, list);
			/*
			ManageEventList.IsVisible = false;
			YourEventList.IsVisible = false;
			InviteEventList.IsVisible = false;
			SearchEventList.IsVisible = false;
			FollowedEventList.IsVisible = false;
			*/
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
				SearchButton.FontAttributes = FontAttributes.Bold;
				searchLine.IsVisible = false;
			} else if (number == 1) {
				ManageButton.FontAttributes = FontAttributes.Bold;
				manageLine.IsVisible = false;
			} else if (number == 2) {
				YoursButton.FontAttributes = FontAttributes.Bold;
				yoursLine.IsVisible = false;
			} else if (number == 3) {
				FollowedButton.FontAttributes = FontAttributes.Bold;
				followLine.IsVisible = false;
			} else if (number == 4) {
				InviteButton.FontAttributes = FontAttributes.Bold;
				inviteLine.IsVisible = false;
			}
		}

		public async void UpdateManageList(int listToUpdate, StackLayout list){
			ObservableCollection<Event> evelist = new ObservableCollection<Event>();
			currentView = listToUpdate;
			var first = DateTime.Now;
			if (listToUpdate == 0) {
				evelist = await _dataManager.EventApiManager.SearchEvents (App.userProfile.ProfileId, App.lastKnownPosition.Latitude, App.lastKnownPosition.Longitude);
				//evelist = await _dataManager.EventApiManager.GetEventsWithOwnerId ();
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
				evelist = await _dataManager.ProfileApiManager.GetEventsFollowed ();
			} else if (listToUpdate == 4) {
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

			while(list.Children.Count != 0) {
				list.Children.RemoveAt(0);
			}
			var dateTimeMonth = DateTime.Now + new TimeSpan (-32, 0, 0, 0);
			int month = dateTimeMonth.Month;
			//for(int e = 0; e < 4; e++) {
			for (int i = 0; i < orderedList.Count; i++) {
				if (month != orderedList [i].StartDate.Month) {
					list.Children.Add (new Label () {
						Text = ("  " + orderedList [i].StartDate.ToString ("MMMM")),
						BackgroundColor = Color.FromHex ("cccccc"),
						TextColor = Color.White,
						FontSize = 25,
						HeightRequest = 40,
						VerticalTextAlignment = TextAlignment.Center
					});
					month = orderedList [i].StartDate.Month;

				}
				if (listToUpdate == 0) { list.Children.Add (new SearchEventTemplate (orderedList [i]));
				} else if (listToUpdate == 1) { list.Children.Add (new ManageEventTemplate (orderedList [i]));
				} else if (listToUpdate == 2) { list.Children.Add (new ManageEventTemplate (orderedList [i]));
				} else if (listToUpdate == 3) { list.Children.Add (new ManageEventTemplate (orderedList [i]));
				} else if (listToUpdate == 4) { list.Children.Add (new NewsMessageView (orderedList [i],NewsMessageView.MessageType.Invite));
				}
			}
			list.Children.Add (new BoxView(){HeightRequest=120});
			//}
		}
	}
}

