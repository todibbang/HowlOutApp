using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class ToDoListView : ContentView, ViewModelInterface
	{
		public void reloadView()
		{
			this.Content = new ToDoListView(conversation, Edit);
		}
		public async Task<UpperBar> getUpperBar()
		{
			var ub = new UpperBar();
			ub.setNavigationlabel("Doodle");
			return ub;
		}
		public ContentView getContentView() { return this; }

		//public List<ToDoItem> ToDoItems;
		bool Edit;
		Conversation conversation;
		public List<Profile> profiles;

		public ToDoListView(Conversation conv, bool edit)
		{
			InitializeComponent();
			Edit = edit;
			conversation = conv;
			profiles = conv.Profiles;

			var tap = new TapGestureRecognizer();
			tap.Tapped += (sender, e) =>
			{
				conversation.ToDoItems.Add(new ToDoItem() { ProfilesNeeded = 1 });
				Edit = true;
				reloadView();
				//App.coreView.setContentViewReplaceCurrent(new VoteView(VoteItems, true));
			};
			addNewIcon.GestureRecognizers.Add(tap);

			if (conversation.ToDoItems == null)
			{
				conversation.ToDoItems = new List<ToDoItem>();
				conversation.ToDoItems.Add(new ToDoItem() { OptionDescription = "Test mulighed nr 1.", ProfilesNeeded = 1, ToDoListView = this });
				conversation.ToDoItems.Add(new ToDoItem() { OptionDescription = "Test mulighed Test mulighed nr 2.", ProfilesNeeded = 2, ToDoListView = this, Profiles = new List<Profile> { App.userProfile } });
				conversation.ToDoItems.Add(new ToDoItem() { OptionDescription = "Test mulighed Test mulighed Test mulighed Test mulighed Test mulighed nr 3.", ProfilesNeeded = 3, ToDoListView = this });
			}
			//ToDoList.ItemsSource = conversation.ToDoItems;


			if (edit)
			{
				ToDoEditScrollView.IsVisible = true;
				foreach (ToDoItem td in conversation.ToDoItems)
				{
					var editVoteItem = new ToDoItemEditTemplate() { BindingContext = td };
					ToDoEditList.Children.Add(editVoteItem);
					editVoteItem.deleteTapped.Tapped += (sender, e) =>
					{
						conversation.ToDoItems.Remove(td);
						reloadView();
						//App.coreView.setContentViewReplaceCurrent(new ToDoListView(conv, true));
					};
				}
				ToDoEditList.Children.Add(addNewLayout);

				updateBtn.IsVisible = true;
				updateBtn.Clicked += (sender, e) =>
				{
					Edit = false;
					reloadView();
					//App.coreView.setContentViewReplaceCurrent(new ToDoListView(conv, false));
				};
				scrollToBottom();
			}
			else {

				ToDoList.ItemsSource = conversation.ToDoItems;
				footerLayout.Children.Add(addNewLayout);
			}

			/*




			conversation = conv;

			var enties = new List<CustomEntry>();
			var profilePicker = new List<CustomPicker>();
			//var statusPicker = new List<CustomPicker>();
			var statusButtons = new List<Tuple<StatusOptions, Button>>();

			for (int i = 0; i < conv.subTypeDictionary["ToDoList"].Count; i++)
			{
				var t = i;

				var newGrid = new Grid() {Padding = new Thickness(0, 10, 0, 10) };
				newGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
				newGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
				newGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

				var leftStack = new StackLayout() { Padding = new Thickness(0, 0, 0, 0), HorizontalOptions = LayoutOptions.FillAndExpand};
				enties.Add(new CustomEntry() { Text = conversation.subTypeDictionary["ToDoList"][t].Item3, HorizontalOptions = LayoutOptions.FillAndExpand });
				leftStack.Children.Add(enties[i]);

				profilePicker.Add(new CustomPicker() { Title = "None", HorizontalOptions = LayoutOptions.FillAndExpand});
				leftStack.Children.Add(profilePicker[i]);
				newGrid.Children.Add(leftStack,0,0);
				profilePicker[t].Items.Add("None");
				foreach (Profile p in conversation.Profiles)
				{
					profilePicker[t].Items.Add(p.Name);
				}

				try
				{
					profilePicker[t].SelectedIndex = conversation.Profiles.FindIndex(pro => pro.ProfileId == conversation.subTypeDictionary["ToDoList"][t].Item1) + 1;
					profilePicker[t].Title = conversation.Profiles.Find(pro => pro.ProfileId == conversation.subTypeDictionary["ToDoList"][t].Item1).Name;
				}
				catch (Exception exc) { }


				Button btn = new Button() { WidthRequest = 40 };
				if (conversation.subTypeDictionary["ToDoList"][t].Item4 == StatusOptions.NotStarted) { btn.BackgroundColor = Color.Red; };
				if (conversation.subTypeDictionary["ToDoList"][t].Item4 == StatusOptions.InProgress) { btn.BackgroundColor = Color.Yellow; };
				if (conversation.subTypeDictionary["ToDoList"][t].Item4 == StatusOptions.Completed) { btn.BackgroundColor = Color.Green; };

				statusButtons.Add(Tuple.Create(conversation.subTypeDictionary["ToDoList"][t].Item4, btn));
				newGrid.Children.Add(statusButtons[i].Item2,1,0);

				var tub = statusButtons[i];
				tub.Item2.Clicked += (sender, e) =>
				{
					var Tuple_4 = conversation.subTypeDictionary["ToDoList"][t];
					var newStatus = StatusOptions.Completed;

					if (Tuple_4.Item4 == StatusOptions.NotStarted) 
					{
						newStatus = StatusOptions.InProgress;
						btn.BackgroundColor = Color.Yellow; 
					};
					if (Tuple_4.Item4 == StatusOptions.InProgress)
					{
						newStatus = StatusOptions.Completed;
						btn.BackgroundColor = Color.Green;
					};
					if (Tuple_4.Item4 == StatusOptions.Completed)
					{
						newStatus = StatusOptions.NotStarted;
						btn.BackgroundColor = Color.Red;
					};

					conversation.subTypeDictionary["ToDoList"][t] = Tuple.Create(Tuple_4.Item1, Tuple_4.Item2, Tuple_4.Item3, newStatus);
				};



				var delBtn = new Button() { HorizontalOptions = LayoutOptions.EndAndExpand, WidthRequest = 30, HeightRequest = 30, BackgroundColor = Color.Red, Text = "X", TextColor = Color.White, BorderRadius = 15 };
				newGrid.Children.Add(delBtn, 2,0);
				specialSettingsGrid.Children.Add(newGrid);
				delBtn.Clicked += (sender, e) =>
				{
					updateDic(enties, profilePicker, statusButtons);
					conversation.subTypeDictionary["ToDoList"].Remove(conversation.subTypeDictionary["ToDoList"][t]);
					reloadView();
				};
			}



			var addNewBtn = new Button() { BackgroundColor = App.HowlOut, HeightRequest = 40, Text = "Add new item +", TextColor = Color.White, BorderRadius = 15 };
			addNewLayout.Children.Add(addNewBtn);
			addNewBtn.Clicked += (sender, e) =>
			{
				updateDic(enties, profilePicker, statusButtons);


				conversation.subTypeDictionary["ToDoListInfo"][0] =
								Tuple.Create((int.Parse(conversation.subTypeDictionary["ToDoListInfo"][0].Item1) + 1) + "", "0", "", StatusOptions.NotStarted);

				conversation.subTypeDictionary["ToDoList"].Add(
					Tuple.Create("", conversation.subTypeDictionary["ToDoListInfo"][0].Item1, "", StatusOptions.NotStarted)
				);
				reloadView();
			};
			*/

		}

		async void scrollToBottom()
		{
			await Task.Delay(100);
			ToDoEditScrollView.ScrollToAsync(addNewLayout, ScrollToPosition.MakeVisible, true);
		}
		/* 
		void updateDic(List<CustomEntry> enties, List<CustomPicker> profilePicker, List<Tuple<StatusOptions, Button>> statusButtons)
		{
			for (int i = 0; i < conversation.subTypeDictionary["ToDoList"].Count; i++)
			{
				var id = "";
				if (profilePicker != null && profilePicker[i].SelectedIndex > 0)
				{
					id = conversation.Profiles[profilePicker[i].SelectedIndex - 1].ProfileId + "";
				}


				var tdlist = conversation.subTypeDictionary["ToDoList"][i];
				conversation.subTypeDictionary["ToDoList"][i] = Tuple.Create(id, tdlist.Item2, enties[i].Text, statusButtons[i].Item1);
			}
		}*/
	}
}
