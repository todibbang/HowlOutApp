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

			var newMessage = new TapGestureRecognizer();
			newMessage.Tapped += async (sender, e) =>
			{
				await newConversationBtn.ScaleTo(0.7, 50, Easing.Linear);
				await newConversationBtn.ScaleTo(1, 50, Easing.Linear);
				App.coreView.setContentViewWithQueue(new InviteListView(new Conversation() { ModelId = CVList.modelId, ModelType = CVList.modelType }, true ), "Create Group", null);
			};
			newConversationBtn.GestureRecognizers.Add(newMessage);

			var addPeopleToCv = new TapGestureRecognizer();
			addPeopleToCv.Tapped += async (sender, e) =>
			{
				await addPeopleToCvBtn.ScaleTo(0.7, 50, Easing.Linear);
				await addPeopleToCvBtn.ScaleTo(1, 50, Easing.Linear);
				App.coreView.setContentViewWithQueue(new InviteListView(CV.conversation, false), "Create Group", null);
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
				App.coreView.setContentViewWithQueue(new FilterSearch(App.userProfile.SearchPreference), "FilterSearch", null);
			};
			filterSearchBtn.GestureRecognizers.Add(createImage);

			navigationLabel.Clicked += (sender, e) =>
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
			showNewConversationButton(false, null);
			showFilterSearchButton(false);
			showBackButton(false);
			showAddPeopleToConversationButton(false, null);
			scrollView = null;
		}

		public void showNewConversationButton(bool show, YourConversations yc)
		{
			newConversationBtn.IsVisible = show;
			CVList = yc;
		}

		public void showAddPeopleToConversationButton(bool show, ConversationView cv)
		{
			addPeopleToCvBtn.IsVisible = show;
			leaveCvBtn.IsVisible = false;
			this.CV = cv;
		}

		public void showFilterSearchButton(bool show)
		{
			filterSearchBtn.IsVisible = show;
		}

		public void setNavigationLabel(string label, ScrollView s)
		{
			navigationLabel.Text = label;
			scrollView = s;

			System.Diagnostics.Debug.WriteLine("label " + label );
			if (s == null)
			{
				System.Diagnostics.Debug.WriteLine("ScrollView is null ");
			}
		}

		public void showBackButton(bool active)
		{
			backBtn.IsVisible = active;
			backButton.IsVisible = active;
		}
	}
}

