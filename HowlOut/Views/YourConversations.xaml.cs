using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace HowlOut
{
	public partial class YourConversations : ContentView, ViewModelInterface
	{
		private DataManager _dataManager;
		public List<Conversation> conList = new List<Conversation>();

		public string modelId;
		public ConversationModelType modelType;

		int conversationListType = 0;

		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}

		public YourConversations(ConversationModelType type, string id, int clt)
		{
			InitializeComponent();
			try
			{
				if (type != ConversationModelType.Profile)
				{
					listHeaderHeight.HeightRequest = 50;
				}
				_dataManager = new DataManager();
				modelType = type;
				modelId = id;
				conversationListType = clt;
				UpdateConversations(true);

				updateList.ItemSelected += OnListItemSelected;
				updateList.Refreshing += async (sender, e) => { await UpdateConversations(true); };
				if (conversationListType == 1)
				{
					CreateButton(type, id);
				}
				else {
					createNewConversation.IsVisible = false;
				}

				createNewConversation.Clicked += (sender, e) =>
				{
					App.coreView.setContentViewWithQueue(new InviteListView(new Conversation() { ModelId = modelId, ModelType = modelType }, true));
				};
			}
			catch (Exception ex) {}
		}

		public void viewInFocus(UpperBar bar)
		{
			if (conversationListType == 1)
			{
				CreateButton(modelType, modelId);
			}
		}
		public void reloadView() { }
		public void viewExitFocus() { }

		public ContentView getContentView() { return this; }

		async Task CreateButton(ConversationModelType type, string id)
		{
			try {
				if (type != ConversationModelType.Profile)
				{
					listHeaderHeight.HeightRequest = 50;
					//conversationInfo.IsVisible = true;
					if (type == ConversationModelType.Event)
					{
						Event eve = await _dataManager.EventApiManager.GetEventById(id);
						App.coreView.topBar.setNavigationLabel(eve.Title, null);
						//conversationInfoImage.Source = eve.ImageSource;
						//conversationInfoModelLabel.Text = eve.Title;
					}
					else if (type == ConversationModelType.Group)
					{
						Group grp = await _dataManager.GroupApiManager.GetGroup(id);
						//conversationInfoImage.Source = grp.ImageSource;
						//conversationInfoModelLabel.Text = grp.Name;
						App.coreView.topBar.setNavigationLabel(grp.Name, null);
					}
				}
			}catch (Exception ex) { }
		}

		public async Task UpdateConversations(bool update)
		{
			try
			{
				nothingToLoad.IsVisible = false;
				if (update) { conList = await _dataManager.MessageApiManager.GetConversations(modelId, modelType); }
				if (conList == null)
				{
					nothingToLoad.IsVisible = true;
					updateList.IsRefreshing = false;
					return;
				}

				if (conList.Count > 1) conList = conList.OrderByDescending(c => c.LastUpdated).ToList();

				if (conversationListType == 1)
				{
					//conList.RemoveAll(c => c.ModelType != ConversationModelType.Profile);
				}
				else if (conversationListType == 2)
				{
					//conList.RemoveAll(c => c.ModelType == ConversationModelType.Profile);
				}

				if (conList.Count == 0) nothingToLoad.IsVisible = true;


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
			}catch (Exception ex) { }
		}

		public async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			try
			{
				if (updateList.SelectedItem == null) { return; }
				var selectedConversation = updateList.SelectedItem as Conversation;
				if (selectedConversation == null) return;
				_dataManager.setConversationSeen(selectedConversation.ConversationID, selectedConversation.ModelType);
				App.coreView.setContentViewWithQueue(new ConversationView(selectedConversation));
				updateList.SelectedItem = null;
				DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			} catch (Exception ex) { }
		}
		public ScrollView getScrollView() { return null; }
	}
}
