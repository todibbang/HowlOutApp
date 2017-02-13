using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class UpperBar : ContentView
	{

		ScrollView scrollView;
		YourConversations CVList;
		ConversationView CV;

		public UpperBar()
		{
			InitializeComponent();

			backButton.Clicked += async (sender, e) =>
			{
				//await backBtn.ScaleTo(0.7, 50, Easing.Linear);
				//await backBtn.ScaleTo(1, 50, Easing.Linear);
				//App.coreView.returnToPreviousView();
				App.tappedPageTest.popView();
			};


			notiButton.Clicked += (sender, e) =>
			{
				App.coreView.setContentViewWithQueue(App.coreView.notifications);
			};

			searchBarDelete.Clicked += (sender, e) =>
			{
				searchBar.Text = "";
			};

			leftButton.Clicked += (sender, e) =>
			{
				App.rootPage.OpenAsync();
			};

			lateSetup();


			/*
			var newMessage = new TapGestureRecognizer();
			newMessage.Tapped += async (sender, e) =>
			{
				await newConversationBtn.ScaleTo(0.7, 50, Easing.Linear);
				await newConversationBtn.ScaleTo(1, 50, Easing.Linear);
				App.coreView.setContentViewWithQueue(new InviteListView(new Conversation() { ModelId = CVList.modelId, ModelType = CVList.modelType }, true ));
			};
			newConversationBtn.GestureRecognizers.Add(newMessage);

			var addPeopleToCv = new TapGestureRecognizer();
			addPeopleToCv.Tapped += async (sender, e) =>
			{
				await addPeopleToCvBtn.ScaleTo(0.7, 50, Easing.Linear);
				await addPeopleToCvBtn.ScaleTo(1, 50, Easing.Linear);
				App.coreView.setContentViewWithQueue(new InviteListView(CV.conversation, false));
			};
			addPeopleToCvBtn.GestureRecognizers.Add(addPeopleToCv);

			leaveCvBtn.Clicked += async (sender, e) =>
			{
				await App.coreView.displayConfirmMessage("Leaving Conversation", "You are about to leave this conversation, do you wish to continue", "Yes", "No");
				await App.coreView._dataManager.MessageApiManager.leaveConversation(CV.ConversationId);
			};

			var createImage = new TapGestureRecognizer();
			createImage.Tapped += async (sender, e) =>
			{
				await filterSearchBtn.ScaleTo(0.7, 50, Easing.Linear);
				await filterSearchBtn.ScaleTo(1, 50, Easing.Linear);
				App.coreView.setContentViewWithQueue(new FilterSearch(App.userProfile.SearchPreference));
			};
			filterSearchBtn.GestureRecognizers.Add(createImage);
			*/

			navigationButton.Clicked += (sender, e) =>
			{
				if (scrollView == null)
				{
					System.Diagnostics.Debug.WriteLine("ScrollView is null ");
				}

				if (scrollView != null)
				{
					scrollView.ScrollToAsync(scrollView.X, 0, true);
				}
			};
			hideAll();
		}

		async void lateSetup()
		{
			await Task.Delay(100);
			//notiBadg.Children.Add(App.coreView.notiButton);
			//hideAll();
		}

		public void hideAll()
		{
			//thisGrid.IsVisible = false;
			showBackButton(false);
			scrollView = null;
			//App.coreView.TopBarLayout.IsVisible = false;
			rightImg.IsVisible = false;
			rightButton.IsVisible = false;
			leftImg.IsVisible = false;
			leftButton.IsVisible = false;
			centerLayout.IsVisible = false;
			notiLayout.IsVisible = false;

			searchBarLayout.IsVisible = false;

			try
			{
				navigationButtonLayout.Children.Remove(navigationButton);
			}
			catch (Exception e) { }

			//setNavigationLabel("", null);
		}

		/*
		public void showNewConversationButton(bool show, YourConversations yc)
		{
			newConversationBtn.IsVisible = show;
			CVList = yc;
		}

		public void showShareBtn(bool show, Event eve)
		{
			shareEventBtn.IsVisible = show;
			if (show)
			{
				TapGestureRecognizer tgr = new TapGestureRecognizer();
				tgr.Tapped += async (sender, e) =>
				{
					//App.coreView.DisplayShare(eve);

					App.coreView._dataManager.AttendTrackEvent(eve, false, true);
				};
				shareEventBtn.GestureRecognizers.Add(tgr);
			}
			else {
				for (int i = shareEventBtn.GestureRecognizers.Count - 1; i > -1; i--)
				{
					shareEventBtn.GestureRecognizers.RemoveAt(i);
				}
			}
		}
		*/

		/*
		public void showAddPeopleToConversationButton(bool show, ConversationView cv)
		{
			addPeopleToCvBtn.IsVisible = show;
			leaveCvBtn.IsVisible = false;
			this.CV = cv;
		}

		public void showFilterSearchButton(bool show)
		{
			filterSearchBtn.IsVisible = show;
		} */

		public void displayNotiLayout()
		{
			notiLayout.IsVisible = true;
		}

		public Button setRightButton(string imageSource)
		{
			thisGrid.IsVisible = true;
			rightImg.IsVisible = true;
			rightButton.IsVisible = true;
			App.coreView.TopBarLayout.IsVisible = true;
			thisGrid.Children.Remove(rightButton);

			rightButton = new Button() { WidthRequest=50 };
			thisGrid.Children.Add(rightButton, 2, 0);

			rightImg.Source = imageSource;
			return rightButton;
		}

		public void showLeftButton()
		{
			App.coreView.TopBarLayout.IsVisible = true;
			thisGrid.IsVisible = true;
			leftImg.IsVisible = true;
			leftButton.IsVisible = true;
		}

		public StackLayout showCenterLayout()
		{
			App.coreView.TopBarLayout.IsVisible = true;
			thisGrid.IsVisible = true;
			centerLayout = new StackLayout() { VerticalOptions= LayoutOptions.EndAndExpand, Padding = new Thickness(0,-10,0,4) };
			thisGrid.Children.Add(centerLayout, 1, 0);
			return centerLayout;
		}

		public void showBackButton(bool active)
		{
			App.coreView.TopBarLayout.IsVisible = active;
			backBtn.IsVisible = active;
			backButton.IsVisible = active;
			thisGrid.IsVisible = active;

		}

		public Button setNavigationlabel(string label)
		{
			App.coreView.TopBarLayout.IsVisible = true;
			thisGrid.IsVisible = true;
			navigationButtonLayout.Children.Remove(navigationButton);
			navigationButton = new Button() { HorizontalOptions = LayoutOptions.FillAndExpand, Text = label, TextColor= Color.White, HeightRequest = 30, BorderColor = Color.White };
			navigationButtonLayout.Children.Add(navigationButton);
			return navigationButton;
		}

		public Entry getSearchBar()
		{
			App.coreView.TopBarLayout.IsVisible = true;
			thisGrid.IsVisible = true;
			searchBarLayout.IsVisible = true;
			return searchBar;
		}

		public async void hideTopbarOnScroll(ScrollView sv)
		{
			scrollView = sv;
			var lastPosition = scrollView.ScrollY;
			while (scrollView != null)
			{
				await Task.Delay(500);
				if (scrollView.ScrollY > lastPosition + 5)
				{
					this.IsVisible = false;
				}
				else if (scrollView.ScrollY < lastPosition - 5)
				{
					this.IsVisible = true;
				}


				lastPosition = scrollView.ScrollY;
			}
		}
	}
}

