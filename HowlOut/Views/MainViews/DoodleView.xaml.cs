using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class DoodleView : ContentView, ViewModelInterface
	{
		public void reloadView()
		{
			this.Content = new DoodleView(conversation);
		}
		public void viewInFocus(UpperBar bar)
		{
			bar.setNavigationlabel("Doodle");
		}
		public async Task<UpperBar> getUpperBar()
		{
			var ub = new UpperBar();
			ub.setNavigationlabel("Doodle");
			return ub;
		}
		public void viewExitFocus() { }
		public ContentView getContentView() { return this; }
		Conversation conversation;
		List<CustomEditor> headerEntries = new List<CustomEditor>();
		List<Button> deleteOptionButtons = new List<Button>();
		Button addBtn = new Button();
		public DoodleView(Conversation conv)
		{
			InitializeComponent();
			conversation = conv;
			DoodleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = 150 });

			var profileCounter = 1;
			foreach (Profile p in conversation.Profiles)
			{
				profileCounter++;
				DoodleGrid.Children.Add(new Label { Text = p.Name }, 0, profileCounter);
				DoodleGrid.RowDefinitions.Add(new RowDefinition { Height = 40 });
			}

			int optionCounter = 0;
			foreach (Tuple<string, string, string, StatusOptions> t in conv.subTypeDictionary["Options"])
			{
				optionCounter++;
				addOption(optionCounter, t);
			}



			addBtn = new Button()
			{
				Text = "Add",
			};
			DoodleGrid.Children.Add(addBtn, optionCounter + 1, 0);
			addBtn.Clicked += (sender, e) =>
			{
				conversation.subTypeDictionary["OptionSettings"][0] = Tuple.Create((int.Parse(conversation.subTypeDictionary["OptionSettings"][0].Item1) + 1) + "", "0", "", StatusOptions.NotStarted);
				conversation.subTypeDictionary["Options"].Add(Tuple.Create(conversation.subTypeDictionary["OptionSettings"][0].Item1, "0", "", StatusOptions.Confirmed));
				//drawProfiles();

				addOption(conversation.subTypeDictionary["Options"].Count, conversation.subTypeDictionary["Options"][conversation.subTypeDictionary["Options"].Count - 1]);


			};




			updateBtn.Clicked += (sender, e) =>
			{
				Update();
				reloadView();
			};
		}

		void Update()
		{
			for (int i = 0; i < headerEntries.Count; i++)
			{
				conversation.subTypeDictionary["Options"][i] = Tuple.Create(conversation.subTypeDictionary["Options"][i].Item1,
					conversation.subTypeDictionary["Options"][i].Item2, headerEntries[i].Text, conversation.subTypeDictionary["Options"][i].Item4);
			}
		}

		void addOption(int optionCounter, Tuple<string, string, string, StatusOptions> tup) {
			DoodleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = 150 });
			var stly = new StackLayout() { Padding = new Thickness(3), VerticalOptions = LayoutOptions.FillAndExpand, HeightRequest = 50 };
			headerEntries.Add(new CustomEditor { Text = tup.Item3, BackgroundColor = Color.White, Placeholder = "Option description...", VerticalOptions = LayoutOptions.FillAndExpand });
			stly.Children.Add(headerEntries[headerEntries.Count - 1]);
			deleteOptionButtons.Add(new Button { HorizontalOptions = LayoutOptions.Center, WidthRequest = 20, HeightRequest = 20, BackgroundColor = Color.Gray, Text = "X", TextColor = Color.White, BorderRadius = 10 });
			DoodleGrid.Children.Add(deleteOptionButtons[deleteOptionButtons.Count - 1], optionCounter, 0);
			DoodleGrid.Children.Add(stly, optionCounter, 1);
			var tu = tup;
			var he = headerEntries[headerEntries.Count - 1];
			deleteOptionButtons[deleteOptionButtons.Count - 1].Clicked += (sender, e) =>
			{
				conversation.subTypeDictionary["Options"].Remove(tu);
				headerEntries.Remove(he);
				Update();
				reloadView();
			};



			var profileCounter = 1;
			foreach (Profile p in conversation.Profiles)
			{
				var b = new Button() { HorizontalOptions = LayoutOptions.Center, WidthRequest = 30, HeightRequest = 30, BorderRadius = 15, BorderColor = Color.Gray, BorderWidth = 1 };
				profileCounter++;






				var sl = new Button { BorderColor = Color.Gray, BorderWidth = 1 , HeightRequest=38, BorderRadius = 0 };
				var bsl = new Grid { Padding = new Thickness(5) };
				DoodleGrid.Children.Add(sl, optionCounter, profileCounter);
				bsl.Children.Add(b);
				DoodleGrid.Children.Add(bsl, optionCounter, profileCounter);
				Tuple<string, string, string, StatusOptions> t = null;
				try
				{
					t = conversation.subTypeDictionary[p.ProfileId].Find(tub => tub.Item1 == conversation.subTypeDictionary["Options"][optionCounter].Item1);
					b.BackgroundColor = t.Item4 == StatusOptions.Confirmed ? Color.Green : Color.Red;
				}
				catch (Exception exc) { b.BackgroundColor = Color.White; }

				b.Clicked += (sender, e) =>
				{
					if (t != null)
					{
						if (t.Item4 == StatusOptions.Confirmed) // arr.IndexOf("bbb")
						{
							t = Tuple.Create(t.Item1, t.Item2, t.Item3, StatusOptions.Declined);
							conversation.subTypeDictionary[p.ProfileId][optionCounter - 1] = t;
							b.BackgroundColor = Color.Red;
						}
						else if (t.Item4 == StatusOptions.Declined)
						{
							t = Tuple.Create(t.Item1, t.Item2, t.Item3, StatusOptions.Confirmed);
							conversation.subTypeDictionary[p.ProfileId][optionCounter - 1] = t;
							b.BackgroundColor = Color.Green;
						}
					}
					else {
						t = Tuple.Create(conversation.subTypeDictionary["Options"][optionCounter - 1].Item1, "0", "", StatusOptions.Confirmed);

						conversation.subTypeDictionary[p.ProfileId].Add(t);
						b.BackgroundColor = Color.Green;
					}
				};
			}
			DoodleGrid.Children.Add(addBtn, optionCounter + 1, 0);
		}
	}
}
