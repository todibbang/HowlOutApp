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
					//CreateButton(type, id);
				}
				/*
				createNewConversation.IsVisible = true;

				createNewConversation.Clicked += (sender, e) =>
				{
					App.coreView.setContentViewWithQueue(new InviteListView(new Conversation() { ModelId = modelId, ModelType = modelType }, true));
				};*/
			}
			catch (Exception ex) { }
		}

		public async Task<UpperBar> getUpperBar()
		{
			var ub = new UpperBar();
			CreateButton(modelType, modelId, ub);
			return ub;
		}

		public void viewInFocus(UpperBar bar)
		{
			CreateButton(modelType, modelId, bar);
		}
		public void reloadView() { }
		public void viewExitFocus() { }

		public ContentView getContentView() { return this; }

		async Task CreateButton(ConversationModelType type, string id, UpperBar ub)
		{
			try
			{
				listHeaderHeight.HeightRequest = 50;
				if (type != ConversationModelType.Profile)
				{
					//conversationInfo.IsVisible = true;
					if (type == ConversationModelType.Event)
					{
						Event eve = await _dataManager.EventApiManager.GetEventById(id);
						ub.setNavigationlabel("Conversations: " + eve.Title);
						//conversationInfoImage.Source = eve.ImageSource;
						//conversationInfoModelLabel.Text = eve.Title;
					}
					else if (type == ConversationModelType.Group)
					{
						Group grp = await _dataManager.GroupApiManager.GetGroup(id);
						//conversationInfoImage.Source = grp.ImageSource;
						//conversationInfoModelLabel.Text = grp.Name;
						ub.setNavigationlabel("Conversations: " + grp.Name);
					}
				}
				else {
					ub.setNavigationlabel("My Conversations");
				}
			}
			catch (Exception ex) { }

			ub.setRightButton("ic_add.png").Clicked += (sender, e) =>
			{
				App.coreView.setContentViewWithQueue(new InviteListView(new Conversation() { ModelId = modelId, ModelType = modelType }, true));
			};
			ub.setPadding();
		}

		public async Task UpdateConversations(bool update)
		{
			try
			{
				closeAsync();
				nothingToLoad.IsVisible = false;
				if (update) { 
					conList = await _dataManager.MessageApiManager.GetConversations(modelId, modelType); 

					conList.Add(new Conversation()
					{
						ModelType = ConversationModelType.Profile,
						ConversationID = "12344567",
						Messages = new List<Comment>(),
						LastMessage = null,
						ModelId = "23456",
						Profiles = new List<Profile>(),
						Title = "ExpenShare Test",
						SubType = ConversationSubType.ExpenShare
					});


					var pTest = new List<Profile>
					{
						new Profile{ProfileId = "1", Name="Tob"},
						new Profile{ProfileId = "2", Name="Wills"},
						new Profile{ProfileId = "3", Name="Peiter"},
						new Profile{ProfileId = "4", Name="Pernille"},
						new Profile{ProfileId = "5", Name="Mor"},
					};

					var dick = new Dictionary<string, List<Tuple<string, string, string, StatusOptions>>>();
					dick.Add("Options", new List<Tuple<string, string, string, StatusOptions>>()
					{
						Tuple.Create("1","0","Køb ind", StatusOptions.Confirmed)
					});
					conList.Add(new Conversation()
					{
						ModelType = ConversationModelType.Profile,
						ConversationID = "12344568",
						Messages = new List<Comment>(),
						LastMessage = null,
						ModelId = "234567",
						Profiles = pTest,
						Title = "Doodle Test",
						SubType = ConversationSubType.Vote,
						subTypeDictionary = dick,
					});

					conList.Add(new Conversation()
					{
						ModelType = ConversationModelType.Profile,
						ConversationID = "12344568",
						Messages = new List<Comment>(),
						LastMessage = null,
						ModelId = "234567",
						Profiles = new List<Profile>() { App.userProfile },
						Title = "ToDoList Test",
						SubType = ConversationSubType.ToDoList,
						subTypeDictionary = dick,
					});
				}
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
					if (App.notificationController.checkIfUnseen(c.ConversationID, NotificationModelType.ProfileConversation))
					{
						n++;
					}
				}
				App.notificationController.setConversationsNoti(n);

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
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}

		async void closeAsync()
		{
			await Task.Delay(2000);
			if (updateList.IsRefreshing) updateList.IsRefreshing = false;
			if (updateList.ItemsSource == null) UpdateConversations(true);
		}

		public async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			try
			{
				if (updateList.SelectedItem == null) { return; }
				var selectedConversation = updateList.SelectedItem as Conversation;
				if (selectedConversation == null) return;
				try
				{
					App.notificationController.setConversationSeen(selectedConversation.ConversationID, selectedConversation.ModelType);

				}
				catch (Exception exc)
				{

				}
				finally
				{
					if (selectedConversation.SubType != null && selectedConversation.SubType == ConversationSubType.Vote)
					{
						//App.coreView.setContentViewWithQueue(new DoodleView(selectedConversation));
						App.coreView.setContentViewWithQueue(new VoteView(null, false));
					}
					else if (selectedConversation.SubType != null && selectedConversation.SubType == ConversationSubType.ToDoList)
					{
						//App.coreView.setContentViewWithQueue(new DoodleView(selectedConversation));
						App.coreView.setContentViewWithQueue(new ToDoListView(selectedConversation, false));
					}
					else {
						App.coreView.setContentViewWithQueue(new ConversationView(selectedConversation));
					}
				}
				updateList.SelectedItem = null;
				DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			}
			catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
		}
		public ScrollView getScrollView() { return null; }
	}
}
