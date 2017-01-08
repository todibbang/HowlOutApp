using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class FindNewFriendsView : ContentView, ViewModelInterface
	{
		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}

		ListsAndButtons profiles;
		ListsAndButtons groups;
		//ListsAndButtons organizations;
		ListsAndButtons facebookFriends;

		CarouselList cv;

		public FindNewFriendsView(int i)
		{
			InitializeComponent();

			//
			setView(i);




			searchBar.TextChanged += (sender, e) =>
			{
				if (String.IsNullOrWhiteSpace(searchBar.Text))
				{
					profiles.createList(App.userProfile.Friends, null, null, null,  false, true, false);
					groups.createList(null, App.userProfile.Groups, null, null,  false, true, false);
					//organizations.createList(null, null, App.userProfile.Organizations, null, null, null, false, true);
				}
				else {
					updateAutoCompleteProfileList(searchBar.Text);
					updateAutoCompleteGroupList(searchBar.Text);
					//updateAutoCompleteOrganizationsList(searchBar.Text);
				}
			};
		}

		public async void setView(int i)
		{
			profiles = new ListsAndButtons(App.userProfile.Friends, null, false, false);
			groups = new ListsAndButtons(null, App.userProfile.Groups, false, false);
			//organizations = new ListsAndButtons(null, null, App.userProfile.Organizations, false, false);

			List<string> fbf = await DependencyService.Get<SocialController>().getFacebookFriends();
			List<Profile> fbp = new List<Profile>();
			foreach (string s in fbf)
			{
				fbp.Add(new Profile() { ProfileId = s });
			}


			facebookFriends = new ListsAndButtons(fbp, null, false, false);

			cv = new CarouselList(
				new List<VisualElement>() { profiles, groups, facebookFriends },
				new List<string>() { "Profiles", "Groups", "Facebook" },
				CarouselList.ViewType.Create
			);

			carouselView.Content = cv;

			cv.setCarousel(i);
		}

		public void viewInFocus(UpperBar bar)
		{
			try
			{
				cv.setCarousel(cv.veryLastCarouselView);
			}
			catch (Exception e) {}
		}
		public void reloadView() { }
		public void viewExitFocus() {
			cv.viewExitFocus();
		}

		public ContentView getContentView() { return this; }

		public async void updateAutoCompleteProfileList(string input)
		{
			var profileSearchResult = await App.coreView._dataManager.ProfileApiManager.GetProfilesFromName(input);
			profiles.createList(profileSearchResult, null, null, null, false, false, false);
		}

		public async void updateAutoCompleteGroupList(string input)
		{
			var groupSearchResult = await App.coreView._dataManager.GroupApiManager.GetGroupsFromName(input);
			groups.createList(null, groupSearchResult, null, null, false, false, false);
		}

		/*
		public async void updateAutoCompleteOrganizationsList(string input)
		{
			var orgsSearchResult = await App.coreView._dataManager.OrganizationApiManager.GetOrganizationsFromName(input);
			organizations.createList(null, null, orgsSearchResult, null, null, null, false, true);
		} */
	}
}
