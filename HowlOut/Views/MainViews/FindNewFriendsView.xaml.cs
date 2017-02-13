using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

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

		//CarouselList cv;

		public FindNewFriendsView(int i)
		{
			InitializeComponent();

			//
			setView(i);




			searchBar.TextChanged += (sender, e) =>
			{
				if (String.IsNullOrWhiteSpace(searchBar.Text))
				{
					profiles.createList(App.userProfile.Friends, null, null, null, false, false, false);
					groups.createList(null, App.userProfile.Groups, null, null, false, false, false);
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

			//TODO
			/*
			cv = new CarouselList(
				new List<ContentViewContainer>() { new ContentViewContainer(profiles), new ContentViewContainer(groups), new ContentViewContainer(facebookFriends) },
				new List<string>() { "Profiles", "Groups", "Facebook" },
				CarouselList.ViewType.Create
			); */

			//carouselView.Content = cv;
			App.coreView.IsLoading(true);
			await Task.Delay(1000);
			App.coreView.IsLoading(false);
			//cv.setCarousel(i);

			if (i == 0)
			{
				carouselView.Content = profiles;
				profileBtn.BackgroundColor = App.HowlOut;
				groupBtn.BackgroundColor = Color.Transparent;
				facebookBtn.BackgroundColor = Color.Transparent;
			}
			if (i == 1)
			{
				carouselView.Content = groups;
				profileBtn.BackgroundColor = Color.Transparent;
				groupBtn.BackgroundColor = App.HowlOut;
				facebookBtn.BackgroundColor = Color.Transparent;
			}
			if (i == 2)
			{
				carouselView.Content = facebookFriends;
				profileBtn.BackgroundColor = Color.Transparent;
				groupBtn.BackgroundColor = Color.Transparent;
				facebookBtn.BackgroundColor = App.HowlOut;
			}


			profileBtn.Clicked += (sender, e) =>
			{
				carouselView.Content = profiles;
				profileBtn.BackgroundColor = App.HowlOut;
				groupBtn.BackgroundColor = Color.Transparent;
				facebookBtn.BackgroundColor = Color.Transparent;
			};
			groupBtn.Clicked += (sender, e) =>
			{
				carouselView.Content = groups;
				carouselView.Content = groups;
				profileBtn.BackgroundColor = Color.Transparent;
				groupBtn.BackgroundColor = App.HowlOut;
				facebookBtn.BackgroundColor = Color.Transparent;
			};
			facebookBtn.Clicked += (sender, e) =>
			{
				carouselView.Content = facebookFriends;
				profileBtn.BackgroundColor = Color.Transparent;
				groupBtn.BackgroundColor = Color.Transparent;
				facebookBtn.BackgroundColor = App.HowlOut;
			};
		}

		public void viewInFocus(UpperBar bar)
		{
			//var grid = new Grid();
			//grid.Children.Add()




			bar.showCenterLayout().Children.Add(topGrid);
		}
		public async Task<UpperBar> getUpperBar()
		{
			var ub = new UpperBar();
			ub.showCenterLayout().Children.Add(topGrid);
			return ub;
		}

		public void reloadView() { }
		public void viewExitFocus()
		{
			//cv.viewExitFocus();
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
