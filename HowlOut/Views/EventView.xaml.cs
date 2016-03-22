﻿using System;
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
				}
				if(currentView == 1) {
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
			ObservableCollection<EventForLists> listEvents = new ObservableCollection<EventForLists>();
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

			/*
			listEvents.Clear ();
			for (int i = 0; i < orderedList.Count; i++) {
				EventForLists EveForLis = new EventForLists (orderedList [i]);
				listEvents.Add (EveForLis);
			}
			theListToUpdate.ItemsSource = listEvents;
			*/

			while(EventListTest.Children.Count != 0) {
				EventListTest.Children.RemoveAt(0);
			}
			var dateTimeMonth = DateTime.Now + new TimeSpan (-32, 0, 0, 0);
			int month = dateTimeMonth.Month;
			for (int i = 0; i < orderedList.Count; i++) {
				if (month != orderedList [i].StartDate.Month) {
					EventListTest.Children.Add (new Label (){ Text = ("  " + orderedList [i].StartDate.ToString("MMMM")), BackgroundColor = Color.White, TextColor = Color.FromHex("00e1c4"), FontSize=25});
					month = orderedList [i].StartDate.Month;
				}
				EventListTest.Children.Add (new SearchEventTemplate (new EventForLists (orderedList [i])));
			}
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
				//SearchEventList.IsVisible = true;
				SearchButton.FontAttributes = FontAttributes.Bold;
				searchLine.IsVisible = false;
			} else if (number == 1) {
				CreateButton.IsVisible = true;
				CreateImage.IsVisible = true;
				CreateImage.Source = "ic_add.png";
				//ManageEventList.IsVisible = true;
				ManageButton.FontAttributes = FontAttributes.Bold;
				manageLine.IsVisible = false;
			} else if (number == 2) {
				//YourEventList.IsVisible = true;
				YoursButton.FontAttributes = FontAttributes.Bold;
				yoursLine.IsVisible = false;
			} else if (number == 3) {
				//FollowedEventList.IsVisible = true;
				FollowedButton.FontAttributes = FontAttributes.Bold;
				followLine.IsVisible = false;
			} else if (number == 4) {
				//InviteEventList.IsVisible = true;
				InviteButton.FontAttributes = FontAttributes.Bold;
				inviteLine.IsVisible = false;
			}
		}
	}
}

