using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class HomeView : ContentView
	{
		DataManager _dataManager = new DataManager ();
		ListsAndButtons listMaker = new ListsAndButtons();

		public HomeView ()
		{
			InitializeComponent ();

			profileContent.Content = new InspectController (App.userProfile, null, null);
			createGroupContent.Content = new CreateGroup (null);

			profileButton.Clicked += (sender, e) => {
				setViewDesign(0);
			};
			findButton.Clicked += (sender, e) => {
				setViewDesign(1);
			};
			createGroupButton.Clicked += (sender, e) => {
				setViewDesign(2);
			};

			friendsButton.Clicked += (sender, e) => {
				profileGrid.IsVisible = true;
				groupGrid.IsVisible = false;
			};
			groupsButton.Clicked += (sender, e) => {
				profileGrid.IsVisible = false;
				groupGrid.IsVisible = true;
			};
			searchBar.TextChanged += (sender, e) => {
				if(searchBar.Text == "" || searchBar.Text == null) { 

				} else {
					updateAutocompleteList();
				}
			};
		}

		public async void updateAutocompleteList()
		{
			var profileSearchResult = App.userProfile.Friends;
			var groupSearchResult = await _dataManager.GroupApiManager.GetAllGroups ();
			listMaker.createList (profileGrid, profileSearchResult, null, null, ListsAndButtons.ListType.Normal, null, null);
			listMaker.createList (groupGrid, null, groupSearchResult, null, ListsAndButtons.ListType.Normal, null, null);
		}

		private void setViewDesign(int number){
			profileContent.IsVisible = false;
			findContent.IsVisible = false;
			createGroupContent.IsVisible = false;
			profileButton.FontAttributes = FontAttributes.None;
			findButton.FontAttributes = FontAttributes.None;
			createGroupButton.FontAttributes = FontAttributes.None;
			profileLine.IsVisible = true;
			findLine.IsVisible = true;
			createGroupLine.IsVisible = true;

			if (number == 0) {
				profileContent.IsVisible = true;
				profileButton.FontAttributes = FontAttributes.Bold;
				profileLine.IsVisible = false;
			} else if (number == 1) {
				findContent.IsVisible = true;
				findButton.FontAttributes = FontAttributes.Bold;
				findLine.IsVisible = false;
			} else if (number == 2) {
				createGroupContent.IsVisible = true;
				createGroupButton.FontAttributes = FontAttributes.Bold;
				createGroupLine.IsVisible = false;
			}
		}
	}
}

