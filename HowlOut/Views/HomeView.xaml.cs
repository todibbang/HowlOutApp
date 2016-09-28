﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class HomeView : ContentView
	{
		DataManager _dataManager = new DataManager ();
		ListsAndButtons listMaker = new ListsAndButtons();

		int currentView = 0;

		public HomeView ()
		{
			InitializeComponent ();

			/*
			App.coreView.topBar.showSearchBar(true);
			App.coreView.topBar.showCreateNewButton(true);
			*/

			profileContent.Content = new InspectController (App.userProfile, null, null);
			createGroupContent.Content = new CreateGroup (null);

			searchBar.TextChanged += (sender, e) =>
			{
				if (String.IsNullOrWhiteSpace(searchBar.Text))
				{
					if (currentView != 0)
					{
						setViewDesign(0);
						profileGrid.Children.Clear();
						groupGrid.Children.Clear();
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

			App.selectButton(new Button[] { optionOne, optionTwo }, optionOne);
			optionOne.Clicked += (sender, e) =>
			{
				App.selectButton(new Button[] { optionOne, optionTwo }, optionOne);
				profileGrid.IsVisible = true;
				groupGrid.IsVisible = false;
			};
			optionTwo.Clicked += (sender, e) => {
				App.selectButton(new Button[] { optionOne, optionTwo }, optionTwo);
				profileGrid.IsVisible = false;
				groupGrid.IsVisible = true;
			};
		}

		public async void updateAutoCompleteProfileList(string input)
		{
			var profileSearchResult = await _dataManager.ProfileApiManager.GetProfilesFromName(input);
			profileGrid.Children.Clear ();
			listMaker.createList (profileGrid, profileSearchResult, null, null, ListsAndButtons.ListType.Normal, null, null);
		}

		public async void updateAutoCompleteGroupList(string input)
		{
			var groupSearchResult = await _dataManager.GroupApiManager.GetGroupsFromName (input);
			groupGrid.Children.Clear ();
			listMaker.createList (groupGrid, null, groupSearchResult, null, ListsAndButtons.ListType.Normal, null, null);
		}

		private void setViewDesign(int number){
			profileContent.IsVisible = false;
			findContent.IsVisible = false;
			createGroupContent.IsVisible = false;
			currentView = number;

			if (number == 0) {
				profileContent.IsVisible = true;
			} else if (number == 1) {
				findContent.IsVisible = true;
			} else if (number == 2) {
				createGroupContent.IsVisible = true;
			}

		}
	}
}

