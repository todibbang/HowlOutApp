using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class HomeView : ContentView
	{
		DataManager _dataManager = new DataManager ();
		ListsAndButtons listMaker = new ListsAndButtons();

		InspectController inspect;

		int currentView = 0;

		public HomeView ()
		{
			InitializeComponent ();

			/*
			App.coreView.topBar.showSearchBar(true);
			App.coreView.topBar.showCreateNewButton(true);
			*/
			inspect = new InspectController(App.userProfile);
			profileContent.Content = inspect;

			searchBar.TextChanged += (sender, e) =>
			{
				if (String.IsNullOrWhiteSpace(searchBar.Text))
				{
					if (currentView != 0)
					{
						setViewDesign(0);
						profileGrid.Children.Clear();
						groupGrid.Children.Clear();
						organizationGrid.Children.Clear();
					}
				}
				else {
					if (currentView != 1)
					{
						setViewDesign(1);
					}
					updateAutoCompleteProfileList(searchBar.Text);
					updateAutoCompleteGroupList(searchBar.Text);
				}
			};

			searchBar.Focused += (sender, e) =>
			{
				if (String.IsNullOrWhiteSpace(searchBar.Text))
				{
					setViewDesign(0);
				}
				else {
					setViewDesign(1);
				}
			};

			App.selectButton(new List<Button> { optionOne, optionTwo, optionThree }, optionOne);
			optionOne.Clicked += (sender, e) =>
			{
				App.selectButton(new List<Button> { optionOne, optionTwo, optionThree }, optionOne);
				profileGrid.IsVisible = true;
				groupGrid.IsVisible = false;
				organizationGrid.IsVisible = false;
			};
			optionTwo.Clicked += (sender, e) => {
				App.selectButton(new List<Button> { optionOne, optionTwo, optionThree }, optionTwo);
				profileGrid.IsVisible = false;
				groupGrid.IsVisible = true;
				organizationGrid.IsVisible = false;
			};
			optionThree.Clicked += (sender, e) => {
				App.selectButton(new List<Button> { optionOne, optionTwo, optionThree }, optionThree);
				organizationGrid.IsVisible = true;
				profileGrid.IsVisible = false;
				groupGrid.IsVisible = false;
			};

			/*
			ListsAndButtons listMaker = new ListsAndButtons();
			listMaker.createList(profileGrid, App.userProfile.Friends, null, null, null, null, null);
			listMaker.createList(groupGrid, null, App.userProfile.Groups, null, null, null, null);
			listMaker.createList(organizationGrid, null, null, App.userProfile.Organizations, null, null, null);
			*/
		}
		/*
		public async void updateLists()
		{
			App.userProfile = await _dataManager.ProfileApiManager.GetLoggedInProfile();
			listMaker.createList(profileGrid, App.userProfile.Friends, null, null, null, null, null);
			listMaker.createList(groupGrid, null, App.userProfile.Groups, null, null, null, null);
			listMaker.createList(organizationGrid, null, null, App.userProfile.Organizations, null, null, null);
		}
		*/

		public async void updateAutoCompleteProfileList(string input)
		{
			var profileSearchResult = await _dataManager.ProfileApiManager.GetProfilesFromName(input);
			profileGrid.Children.Clear ();
			listMaker.createList (profileGrid, profileSearchResult, null, null, null, null, null);
		}

		public async void updateAutoCompleteGroupList(string input)
		{
			var groupSearchResult = await _dataManager.GroupApiManager.GetGroupsFromName (input);
			groupGrid.Children.Clear ();
			listMaker.createList (groupGrid, null, groupSearchResult, null, null, null, null);
		}

		public async void updateAutoCompleteOrganizationsList(string input)
		{
			var orgsSearchResult = await _dataManager.OrganizationApiManager.GetOrganizationsFromName(input);
			organizationGrid.Children.Clear();
			listMaker.createList(groupGrid, null, null, orgsSearchResult, null, null, null);
		}

		private void setViewDesign(int number){
			profileContent.IsVisible = false;
			findContent.IsVisible = false;
			currentView = number;

			if (number == 0) {
				profileContent.IsVisible = true;
			} else if (number == 1) {
				findContent.IsVisible = true;
			} 

		}

		public ScrollView getScrollView()
		{
			return inspect.getScrollView();
		}
	}
}

