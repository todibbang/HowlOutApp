using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace HowlOut
{
	public partial class YourConversations : ContentView
	{
		private DataManager _dataManager;
		public List<Conversation> conList = new List<Conversation>();

		public string modelId;
		public ConversationModelType modelType;

		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}

		public YourConversations(ConversationModelType type, string id)
		{
			InitializeComponent();
			_dataManager = new DataManager();
			modelType = type;
			modelId = id;
			UpdateConversations(true);

			updateList.ItemSelected += OnListItemSelected;
			updateList.Refreshing += async (sender, e) => { await UpdateConversations(true); };
			CreateButton(type, id);

			createNewConversation.Clicked += (sender, e) =>
			{
				App.coreView.setContentViewWithQueue(new InviteListView(new Conversation() { ModelId = modelId, ModelType = modelType }, true), "Create Group", null);
			};


		}

		async Task CreateButton(ConversationModelType type, string id)
		{
			if (type != ConversationModelType.Profile)
			{
				conversationInfo.IsVisible = true;
				if (type == ConversationModelType.Event)
				{
					Event eve = await _dataManager.EventApiManager.GetEventById(id);
					conversationInfoImage.Source = eve.ImageSource;
					conversationInfoModelLabel.Text = "Event: " + eve.Title;
				}
				else if (type == ConversationModelType.Group)
				{
					Group grp = await _dataManager.GroupApiManager.GetGroup(id);
					conversationInfoImage.Source = grp.ImageSource;
					conversationInfoModelLabel.Text = "Group: " + grp.Name;
				}
				else if (type == ConversationModelType.Organization)
				{
					Organization org = await _dataManager.OrganizationApiManager.GetOrganization(id);
					conversationInfoImage.Source = org.ImageSource;
					conversationInfoModelLabel.Text = "Organization: " + org.Name;
				}
			}
		}

		public async Task UpdateConversations(bool update)
		{
			nothingToLoad.IsVisible = false;
			if (update) { conList = await _dataManager.MessageApiManager.GetConversations(modelId, modelType); }
			if (conList == null) return;
			if (conList.Count == 0) nothingToLoad.IsVisible = true;
			if (conList.Count > 1) conList = conList.OrderByDescending(c => c.LastUpdated).ToList();


			int n = 0;
			foreach (Conversation c in conList)
			{
				if (_dataManager.checkIfUnseen(c.ConversationID, NotificationModelType.ProfileConversation))
				{
					n++;
				}
			}
			App.coreView.setConversationsNoti(n);

			ObservableCollection<GroupedConversations> groupedConversations = new ObservableCollection<GroupedConversations>();
			if (conList.Count > 0)
			{
				GroupedConversations monthGroup = null;
				int month = conList[0].LastUpdated.Month;
				for (int d = 0; d < conList.Count; d++)
				{
					if (d == 0) { monthGroup = new GroupedConversations() { Date = (conList[d].LastUpdated.ToString("MMMMM")) }; }
					if (month != conList[d].LastUpdated.Month)
					{
						month = conList[d].LastUpdated.Month;
						groupedConversations.Add(monthGroup);
						monthGroup = new GroupedConversations() { Date = (conList[d].LastUpdated.ToString("MMMMM")) };
					}
					monthGroup.Add(conList[d]);
					if (d == conList.Count - 1) { groupedConversations.Add(monthGroup); }
				}
			}

			updateList.ItemsSource = groupedConversations;
			updateList.IsRefreshing = false;
		}

		public async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (updateList.SelectedItem == null) { return; }
			var selectedConversation = updateList.SelectedItem as Conversation;
			if (selectedConversation == null) return;
			_dataManager.setConversationSeen(selectedConversation.ConversationID, selectedConversation.ModelType);
			App.coreView.setContentViewWithQueue(new ConversationView(selectedConversation), "Conversation", null);
			updateList.SelectedItem = null;
			DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
		}
		public ScrollView getScrollView() { return null; }
	}
}
