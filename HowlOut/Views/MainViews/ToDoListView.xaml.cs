using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class ToDoListView : ContentView, ViewModelInterface
	{
		public void reloadView()
		{
			this.Content = new ToDoListView(conversation);
		}
		public void viewInFocus(UpperBar bar)
		{
			bar.setNavigationlabel("Expense");
		}
		public void viewExitFocus() { }
		public ContentView getContentView() { return this; }


		Conversation conversation;

		public ToDoListView(Conversation conv)
		{
			InitializeComponent();
			conversation = conv;

			var enties = new List<CustomEntry>();
			var profilePicker = new List<CustomPicker>();
			var statusPicker = new List<CustomPicker>();

			for (int i = 0; i < conv.subTypeDictionary["ToDoList"].Count; i++)
			{
				var t = i;

				var newItem = new StackLayout() { Orientation = StackOrientation.Horizontal, Padding = new Thickness(0, 10, 0, 10) };

				enties.Add(new CustomEntry() { WidthRequest = 200, Text = conversation.subTypeDictionary["ToDoList"][t].Item3 });
				newItem.Children.Add(enties[i]);

				profilePicker.Add(new CustomPicker() { Title = "None", WidthRequest=200 });
				newItem.Children.Add(profilePicker[i]);
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

				statusPicker.Add(new CustomPicker() { Title = "Awaiting", WidthRequest=150 });
				newItem.Children.Add(statusPicker[i]);
				for (int en = 4; en < 7; en++)
				{
					statusPicker[t].Items.Add(((StatusOptions)en).ToString());
				}
				try
				{
					statusPicker[t].SelectedIndex = Array.IndexOf(Enum.GetValues(typeof(StatusOptions)), conversation.subTypeDictionary["ToDoList"][t].Item4) - 4;
					statusPicker[t].Title = conversation.subTypeDictionary["ToDoList"][t].Item4.ToString();
				}
				catch (Exception exc) { }



				var delBtn = new Button() { HorizontalOptions = LayoutOptions.EndAndExpand, WidthRequest = 30, HeightRequest = 30, BackgroundColor = Color.Red, Text = "X", TextColor = Color.White, BorderRadius = 15 };
				newItem.Children.Add(delBtn);
				specialSettingsGrid.Children.Add(newItem);
				delBtn.Clicked += (sender, e) =>
				{
					updateDic(enties, profilePicker, statusPicker);
					conversation.subTypeDictionary["ToDoList"].Remove(conversation.subTypeDictionary["ToDoList"][t]);
					reloadView();
				};
			}



			var addNewBtn = new Button() { BackgroundColor = App.HowlOut, HeightRequest = 40, Text = "Add new item +", TextColor = Color.White, BorderRadius = 15 };
			addNewLayout.Children.Add(addNewBtn);
			addNewBtn.Clicked += (sender, e) =>
			{
				updateDic(enties, profilePicker, statusPicker);


				conversation.subTypeDictionary["ToDoListInfo"][0] =
								Tuple.Create((int.Parse(conversation.subTypeDictionary["ToDoListInfo"][0].Item1) + 1) + "", "0", "", StatusOptions.NotStarted);

				conversation.subTypeDictionary["ToDoList"].Add(
					Tuple.Create("", conversation.subTypeDictionary["ToDoListInfo"][0].Item1, "", StatusOptions.NotStarted)
				);
				reloadView();
			};

		}
		void updateDic(List<CustomEntry> enties, List<CustomPicker> profilePicker, List<CustomPicker> statusPicker)
		{
			for (int i = 0; i < conversation.subTypeDictionary["ToDoList"].Count; i++)
			{
				var id = "";
				if (profilePicker != null && profilePicker[i].SelectedIndex > 0)
				{
					id = conversation.Profiles[profilePicker[i].SelectedIndex - 1].ProfileId + "";
				}


				var tdlist = conversation.subTypeDictionary["ToDoList"][i];
				conversation.subTypeDictionary["ToDoList"][i] = Tuple.Create(id, tdlist.Item2, enties[i].Text, (StatusOptions)(statusPicker[i].SelectedIndex + 4));
			}
		}
	}
}
