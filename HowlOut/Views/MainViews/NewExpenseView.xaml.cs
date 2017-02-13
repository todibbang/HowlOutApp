using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class NewExpenseView : ContentView, ViewModelInterface
	{
		public void reloadView() { }
		public void viewInFocus(UpperBar bar) {
			bar.setNavigationlabel("Expense");
		}
		public async Task<UpperBar> getUpperBar()
		{
			var ub = new UpperBar();
			ub.setNavigationlabel("Expense");
			return ub;
		}
		public void viewExitFocus() { }
		public ContentView getContentView() { return this; }

		double totalExpenceAmount = 0.0;
		Dictionary<string, int> pickerDictionary = new Dictionary<string, int>();
		Dictionary<string, double> specialAmounts = new Dictionary<string, double>();
		Dictionary<string, CustomEntry> specialSettingLabels = new Dictionary<string, CustomEntry>();
		Dictionary<string, Button> specialSettingCancelButtons = new Dictionary<string, Button>();
		Profile profileExpensePaidBy;

		System.ComponentModel.PropertyChangedEventHandler action;

		public NewExpenseView(Conversation conv, Comment oldCom)
		{
			InitializeComponent();


			whoIsPayingPicker.PropertyChanged += (sender, e) =>
			{
				profileExpensePaidBy = conv.Profiles[whoIsPayingPicker.SelectedIndex];
				secondGridTitle.Text = "New expense of " + totalExpenceAmount + " paid by " + profileExpensePaidBy.Name;
			};

			foreach (Profile p in conv.Profiles)
			{
				pickerDictionary.Add(p.ProfileId, whoIsPayingPicker.Items.Count);
				whoIsPayingPicker.Items.Add(p.Name);
				if (p.ProfileId == App.userProfile.ProfileId)
				{
					whoIsPayingPicker.SelectedIndex = whoIsPayingPicker.Items.Count-1;
				}

				specialSettingLabels.Add(p.ProfileId, new CustomEntry() { HorizontalOptions = LayoutOptions.FillAndExpand, Placeholder="0.0", WidthRequest=70 });
				specialSettingCancelButtons.Add(p.ProfileId, new Button() { HorizontalOptions = LayoutOptions.EndAndExpand, WidthRequest = 30, HeightRequest = 30, BackgroundColor = Color.Gray, Text="X", TextColor = Color.White, BorderRadius=15 });
				var specialSettingRow = new StackLayout() { Orientation = StackOrientation.Horizontal, Padding = new Thickness(0,10,0,10)};
				specialSettingRow.Children.Add(new Label() { Text = p.Name, WidthRequest = 200, VerticalTextAlignment = TextAlignment.Center});
				specialSettingRow.Children.Add(specialSettingLabels[p.ProfileId]);
				specialSettingRow.Children.Add(specialSettingCancelButtons[p.ProfileId]);
				specialSettingsGrid.Children.Add(specialSettingRow);

				if (oldCom == null)
				{
					specialSettingLabels[p.ProfileId].Focused += async (sender, e) =>
					{
						foreach (KeyValuePair<string, CustomEntry> dic in specialSettingLabels)
						{
							try
							{
								dic.Value.PropertyChanged -= action;
							}
							catch (Exception exc) { }
						}




						action = new System.ComponentModel.PropertyChangedEventHandler((sender2, e2) =>
						{
							if (!specialAmounts.ContainsKey(p.ProfileId))
							{
								specialAmounts.Add(p.ProfileId, 0.0);
								specialSettingCancelButtons[p.ProfileId].BackgroundColor = App.HowlOut;
							}
							try
							{
								specialAmounts[p.ProfileId] = Math.Round(double.Parse(specialSettingLabels[p.ProfileId].Text), 2);
								if (double.Parse(specialSettingLabels[p.ProfileId].Text) > totalExpenceAmount)
								{
									specialAmounts[p.ProfileId] = totalExpenceAmount;
									specialSettingLabels[p.ProfileId].Text = totalExpenceAmount + "";
								}
								specialSettingLabels[p.ProfileId].TextColor = App.NormalTextColor;
							}
							catch (Exception exc)
							{
								specialAmounts[p.ProfileId] = 0.0;
								specialSettingLabels[p.ProfileId].TextColor = Color.Red;
							}

							updateSpecialSettingLabels();
						});

						specialSettingLabels[p.ProfileId].PropertyChanged += action;

						while (true)
						{
							await Task.Delay(100);
							if (!specialSettingLabels[p.ProfileId].IsFocused)
							{
								specialSettingLabels[p.ProfileId].PropertyChanged -= action;
								System.Diagnostics.Debug.WriteLine("BREAKED");
								break;
							}
						}
					};
					
					specialSettingCancelButtons[p.ProfileId].Clicked += (sender, e) =>
					{
						try
						{
							specialSettingLabels[p.ProfileId].PropertyChanged -= action;
						}
						catch (Exception exc) {} 
						specialSettingLabels[p.ProfileId].TextColor = App.NormalTextColor;
						specialSettingCancelButtons[p.ProfileId].BackgroundColor = Color.Gray;
						specialAmounts.Remove(p.ProfileId);
						updateSpecialSettingLabels();
					};
				}
			}

			totalExpenseEntry.PropertyChanged += (sender, e) =>
			{
				try
				{
					totalExpenceAmount = double.Parse(totalExpenseEntry.Text);
					totalExpenseEntry.TextColor = App.NormalTextColor;
				}
				catch (Exception exc)
				{
					totalExpenceAmount = 0.0;
					totalExpenseEntry.TextColor = Color.Red;
				}

				secondGridTitle.Text = "New expense of " + totalExpenceAmount + " paid by " + profileExpensePaidBy.Name;

				updateSpecialSettingLabels();
			};

			finishBtn.Clicked += (sender, e) =>
			{
				var com = new Comment();
				com.Content = desciption.Text;
				com.ImageSource = "";
				com.conversation = conv;
				com.expensePaiedById = profileExpensePaidBy.ProfileId;
				com.SenderId = App.userProfile.ProfileId;
				com.totalAmount = totalExpenceAmount;
				com.uniqueAmounts = specialAmounts;
				com.DateAndTime = System.DateTime.Now;
				conv.Messages.Add(com);
				App.coreView.returnToPreviousView();
			};

			if (oldCom != null)
			{
				displayingOldExpense.IsVisible = true;
				finishBtn.IsVisible = false;


				profileExpensePaidBy = new Profile() { Name = oldCom.expensePaiedByName, ProfileId = oldCom.expensePaiedById };
				totalExpenseEntry.Text = oldCom.totalAmount + "";
				whoIsPayingPicker.SelectedIndex = pickerDictionary[oldCom.expensePaiedById];
				whoIsPayingPicker.Title = oldCom.expensePaiedByName;
				specialAmounts = oldCom.uniqueAmounts;
				foreach (KeyValuePair<string, double> dic in specialAmounts)
				{
					specialSettingLabels[dic.Key].Text = dic.Value + "";
				}
				updateSpecialSettingLabels();
				desciption.Text = oldCom.MessageText;


				delaySetup();
			}
		}

		async void delaySetup()
		{
			await Task.Delay(100);
			secondGridTitle.Text = "This expense of " + totalExpenceAmount + " was paid by " + profileExpensePaidBy.Name;
		}

		void updateSpecialSettingLabels()
		{
			double totalSpecialAmounts = 0.0;
			foreach (KeyValuePair<string, double> dic in specialAmounts)
			{
				//specialSettingLabels[dic.Key].Text = dic.Value + "";
				totalSpecialAmounts += dic.Value;
			}

			foreach (KeyValuePair<string, CustomEntry> dic in specialSettingLabels)
			{
				if (!specialAmounts.ContainsKey(dic.Key))
				{
					dic.Value.Text = Math.Round(((totalExpenceAmount - totalSpecialAmounts) / (specialSettingLabels.Count - specialAmounts.Count)),2) + "";
				}
			}
		}
	}
}
