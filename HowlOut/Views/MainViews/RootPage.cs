using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace HowlOut
{
	public class RootPage : MasterDetailPage
	{
		Dictionary<MenuType, NavigationPage> Pages { get; set; }

		TappedPageTest tappedPageTest;
		public MenuPage menuPage;

		public RootPage(TappedPageTest tpt)
		{
			tappedPageTest = tpt;
			MasterBehavior = MasterBehavior.Default;
			Pages = new Dictionary<MenuType, NavigationPage>();
			menuPage = new MenuPage(this);

			Master = menuPage;

			//MasterBehavior = mas



			//App.homePage = new HomePage(null, null, 0, 0, 0);
			var homeItem = new ToolbarItem { Text = "MAP" };

			ToolbarItems.Add(homeItem);

			homeItem.Clicked += (object sender, System.EventArgs e) =>
			{
				NavigateAsync(MenuType.Home, false); 
			};

			NavigateAsync(MenuType.Home, true);

			InvalidateMeasure();
		}

		public async Task CloseAsync()
		{
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			IsPresented = false;
		}

		public async Task OpenAsync()
		{
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			IsPresented = true;
		}

		public async Task NavigateAsync(MenuType id, bool initial)
		{
			if (!Pages.ContainsKey(id))
			{
				//TODO tilføj party calender page
				switch (id)
				{
					case MenuType.Home:
						//Pages.Add(id, new HowlOutNavigationPage(App.coreView));
						Pages.Add(id, new HowlOutNavigationPage(tappedPageTest));
						break;
					case MenuType.ViewProfile:
						App.coreView.setContentViewWithQueue(new InspectController(App.userProfile));
						return;
					case MenuType.ViewFriends:
						App.coreView.setContentViewWithQueue(new FindNewFriendsView(0));
						return;
				}
			}

			Page newPage = Pages[id];
			if (newPage == null)
			{
				return;
			}

			Detail = newPage;
		}
		public void RefreshMenuPage()
		{
			this.menuPage = new MenuPage(this);
		}

		public async Task displayAlertMessage(string title, string message, string buttonText)
		{
			await DisplayAlert(title, message, buttonText);
		}

		public async Task<bool> displayConfirmMessage(string title, string message, string acceptText, string cancelText)
		{
			bool accept = await DisplayAlert(title, message, acceptText, cancelText);
			if (accept)
			{
				return true;
			}
			else {
				return false;
			}
		}
	}
}
