using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class InviteListView : ContentView
	{
		List<Profile> profilesAdded = new List<Profile>();
		DataManager _dataManager = new DataManager();

		public InviteListView(Conversation conversation, bool create)
		{
			InitializeComponent();
			setup();
			createConversation.Clicked += async (sender, e) =>
			{
				App.coreView.returnToPreviousView();
				if (create)
				{
					profilesAdded.Add(App.userProfile);
					Conversation conv = await _dataManager.MessageApiManager.CreateConversations(profilesAdded);
					if (conv != null)
					{
						App.coreView.setContentViewWithQueue(new ConversationView(conv), "", null);
					}
				}
				else {
					Conversation newConv = await _dataManager.MessageApiManager.AddProfilesToConversation(conversation.ConversationID, profilesAdded);
					if (newConv != null)
					{
						App.coreView.setContentViewWithQueue(new ConversationView(newConv), "", null);
					}
				}
			};

		}
		public InviteListView(Event eve)
		{
			InitializeComponent();
			setup();
		}
		public InviteListView(Group grp)
		{
			InitializeComponent();
			setup();
		}
		public InviteListView(Organization org)
		{
			InitializeComponent();
			setup();
		}

		void setup()
		{
			peopleToAddConversationList.ItemSelected += OnPeopleToAddListItemSelected;
			addedToConversationList.ItemSelected += OnAddedPeopleListItemSelected;

			cancelCreateConversation.Clicked += (sender, e) => { App.coreView.returnToPreviousView(); };

		}

		public void OnPeopleToAddListItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (peopleToAddConversationList.SelectedItem == null) { return; }
			var selectedProfile = peopleToAddConversationList.SelectedItem as Profile;
			if (!profilesAdded.Exists(p => p.ProfileId == selectedProfile.ProfileId))
			{
				profilesAdded.Add(selectedProfile);
				addedToConversationList.ItemsSource = null;
				addedToConversationList.ItemsSource = profilesAdded;
			}
			peopleToAddConversationList.SelectedItem = null;
		}
		public void OnAddedPeopleListItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (addedToConversationList.SelectedItem == null) { return; }
			var selectedProfile = addedToConversationList.SelectedItem as Profile;
			if (profilesAdded.Exists(p => p.ProfileId == selectedProfile.ProfileId))
			{
				profilesAdded.Remove(profilesAdded.Find(p => p.ProfileId == selectedProfile.ProfileId));
				addedToConversationList.ItemsSource = null;
				addedToConversationList.ItemsSource = profilesAdded;
			}
			addedToConversationList.SelectedItem = null;
		}
	}
}
