using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class MenuPage : ContentPage
	{
		RootPage root;
		List<HomeMenuItem> menuItems;
		public MenuPage(RootPage root)
		{

			this.root = root;
			InitializeComponent();
			//BackgroundColor = Color.FromHex("#66bc93");

			//ListViewMenu.BackgroundColor = Color.FromHex("#66bc93");
			ListViewMenu.BackgroundColor = Color.Transparent;
			//ListViewMenu 
			Title = "wigwam";
			//Icon = "ic_party.png";



			//getOwnCamp();
			//getOwnInfo();

			setMenuItems();

			ListViewMenu.ItemSelected += async (sender, e) =>
			{
				if (ListViewMenu.SelectedItem == null)
					return;
				await this.root.NavigateAsync(((HomeMenuItem)e.SelectedItem).MenuType, false);
				await this.root.CloseAsync();
				await Task.Delay(5);

				ListViewMenu.SelectedItem = null;
			};
			/*
			MyCampButton.Clicked += (sender, e) =>
			{
				if (App.UserCamp != null && !string.IsNullOrWhiteSpace(App.UserCamp.Name))
				{
					App.campFocus = App.UserCamp;
					App.rootPage.NavigateAsync(Models.MenuType.Home, false);
				}
			};*/
		}

		public void updateMenuPage()
		{
			//getOwnInfo();
			//getOwnCamp();
		}


		public async Task getOwnInfo()
		{
			//await App.CampApiManager.GetUserInfo();
			if (App.userProfile != null)
			{

				if (App.userProfile.Name.Contains(" "))
				{
					var name = new string[100];

					name = App.userProfile.Name.Split(null);

					YouLabel.Text = name[0];
				}
				else {
					YouLabel.Text = App.userProfile.Name;
				}
			}
			else {
				YouLabel.Text = "Error Loading Name";
			}
		}
		/*
		public async Task getOwnCamp()
		{
			await App.CampApiManager.GetOwnCamp();
			if (App.UserCamp != null && !string.IsNullOrWhiteSpace(App.UserCamp.Name))
			{
				YourCampLabel.Text = App.UserCamp.Name;
				menuImage.Source = App.UserCamp.ImageSource;
			}
			else {
				YourCampLabel.Text = "Your are not in a camp";
				menuImage.Source = "ic_defualtimage.png";
			}
		} */

		public async void setMenuItems()
		{
			while (true)
			{
				await Task.Delay(1000);
				try
				{
					menuImage.Source = App.userProfile.ImageSource;
					YouLabel.Text = App.userProfile.Name;
					break;
				}
				catch (Exception exc) {}

			}
			var newMenuItems = new List<HomeMenuItem>();
			//newMenuItems.Add(new HomeMenuItem { Title = "View Profile", MenuType = MenuType.ViewProfile, Icon = "ic_me.png" });
			//newMenuItems.Add(new HomeMenuItem { Title = "View Friends", MenuType = MenuType.ViewFriends, Icon = "ic_group.png" });
			ListViewMenu.ItemsSource = newMenuItems;
			FriendsList.ItemsSource = App.userProfile.Friends;
			/*
			await getOwnInfo();
			//getOwnInfo ();
			await getOwnCamp();

			var newMenuItems = new List<HomeMenuItem>();

			if (string.IsNullOrWhiteSpace(App.UserCamp.Name))
			{
				newMenuItems.Add(new HomeMenuItem { Title = "SET UP CAMP", MenuType = MenuType.SetUpCamp, Icon = "ic_setupcamp.png" });
				newMenuItems.Add(new HomeMenuItem { Title = "JOIN CAMP", MenuType = MenuType.JoinCamp, Icon = "ic_joincamp.png" });
			}
			else {
				newMenuItems.Add(new HomeMenuItem { Title = "THROW PARTY", MenuType = MenuType.ThrowParty, Icon = "ic_party.png" });
			}
			newMenuItems.Add(new HomeMenuItem { Title = "PARTY CALENDER", MenuType = MenuType.PartyCalender, Icon = "ic_party.png" });
			newMenuItems.Add(new HomeMenuItem { Title = "CAMP RATINGS", MenuType = MenuType.CampRatings, Icon = "ic_camprating.png" });
			//new HomeMenuItem { Title = "HELP", MenuType = MenuType.Help, Icon = "ic_help.png" },
			newMenuItems.Add(new HomeMenuItem { Title = "ABOUT US", MenuType = MenuType.AboutUs, Icon = "ic_aboutus.png" });
			newMenuItems.Add(new HomeMenuItem { Title = "HELP", MenuType = MenuType.Help, Icon = "ic_help.png" });
			if (!string.IsNullOrWhiteSpace(App.UserCamp.Name))
			{
				newMenuItems.Add(new HomeMenuItem { Title = "LEAVE/DELETE CAMP", MenuType = MenuType.Settings, Icon = "ic_settings.png" });
			}

			if (string.IsNullOrWhiteSpace(App.UserCamp.Name))
			{
				menuImage.Source = "ic_defualtimage.png";
			}
			else {
				menuImage.Source = App.UserCamp.ImageSource;
			}

			ListViewMenu.ItemsSource = newMenuItems;*/
		}
	}
}
