using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class FindNewFriendsView : ContentView
	{
		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}

		ListsAndButtons profiles;
		ListsAndButtons groups;
		ListsAndButtons organizations;

		public FindNewFriendsView(int i)
		{
			InitializeComponent();


			profiles = new ListsAndButtons(App.userProfile.Friends, null, null, false, false);
			groups = new ListsAndButtons(null, App.userProfile.Groups, null, false, false);
			organizations = new ListsAndButtons(null, null, App.userProfile.Organizations, false, false);

			CarouselList cv = new CarouselList (
				new List<VisualElement>() { profiles, groups, organizations },
				new List<string>() { "Profiles", "Groups", "Organizations" }
			);

			carouselView.Content = cv;

			cv.setCarousel(i);



			searchBar.TextChanged += (sender, e) =>
			{
				if (String.IsNullOrWhiteSpace(searchBar.Text))
				{
					profiles.createList(App.userProfile.Friends, null, null, null, null, null, false, true);
					groups.createList(null, App.userProfile.Groups, null, null, null, null, false, true);
					organizations.createList(null, null, App.userProfile.Organizations, null, null, null, false, true);
				}
				else {
					updateAutoCompleteProfileList(searchBar.Text);
					updateAutoCompleteGroupList(searchBar.Text);
					updateAutoCompleteOrganizationsList(searchBar.Text);
				}
			};
		}

		public async void updateAutoCompleteProfileList(string input)
		{
			var profileSearchResult = await App.coreView._dataManager.ProfileApiManager.GetProfilesFromName(input);
			profiles.createList(profileSearchResult, null, null, null, null, null, false, true);
		}

		public async void updateAutoCompleteGroupList(string input)
		{
			var groupSearchResult = await App.coreView._dataManager.GroupApiManager.GetGroupsFromName(input);
			groups.createList(null, groupSearchResult, null, null, null, null, false, true);
		}

		public async void updateAutoCompleteOrganizationsList(string input)
		{
			var orgsSearchResult = await App.coreView._dataManager.OrganizationApiManager.GetOrganizationsFromName(input);
			organizations.createList(null, null, orgsSearchResult, null, null, null, false, true);
		}
	}
}
