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

		public UpperBar ()
		{
			InitializeComponent ();

			backButton.Clicked += async (sender, e) => 
			{
				await backBtn.ScaleTo(0.7, 50, Easing.Linear);
				await backBtn.ScaleTo(1, 50, Easing.Linear);
				App.coreView.returnToPreviousView();
			};				

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

		}

		public void hideAll()
		{
			thisGrid.IsVisible = false;
			showBackButton(false);
			scrollView = null;
			App.coreView.TopBarLayout.IsVisible = false;
			rightImg.IsVisible = false;
			rightButton.IsVisible = false;
			navigationLabel.Text = "";
			try
			{
				thisGrid.Children.Remove(navigationButton);
			}
			catch (Exception e) {}

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


		public void setNavigationLabel(string label, ScrollView s)
		{
			
			navigationLabel.Text = label;
			App.coreView.TopBarLayout.IsVisible = true;
			scrollView = s;

			System.Diagnostics.Debug.WriteLine("label " + label );
			if (s == null)
			{
				System.Diagnostics.Debug.WriteLine("ScrollView is null ");
			}
		} 

		public Button setRightButton(string imageSource)
		{
			thisGrid.IsVisible = true;
			rightImg.IsVisible = true;
			rightButton.IsVisible = true;
			App.coreView.TopBarLayout.IsVisible = true;
			thisGrid.Children.Remove(rightButton);

			rightButton = new Button();
			thisGrid.Children.Add(rightButton, 2, 0);

			rightImg.Source = imageSource;
			return rightButton;
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
			thisGrid.Children.Remove(navigationButton);
			navigationLabel.Text = label;
			navigationButton = new Button() {HorizontalOptions= LayoutOptions.FillAndExpand};
			thisGrid.Children.Add(navigationButton, 1, 0);
			return navigationButton;
		}
	}
}

