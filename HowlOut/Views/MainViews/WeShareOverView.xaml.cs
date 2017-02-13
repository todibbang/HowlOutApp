using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class WeShareOverView : ContentView, ViewModelInterface
	{
		public void reloadView() {
			this.Content = new WeShareOverView(conversation);
		}
		Conversation conversation;
		public void viewInFocus(UpperBar bar) { }
		public async Task<UpperBar> getUpperBar()
		{
			var ub = new UpperBar();
			return ub;
		}
		public void viewExitFocus() { }
		public ContentView getContentView() { return this; }
		double totalExpense = 0.0;
		double totalAmountYouNeedToBePaid = 0.0;
		double totalAmountYouMustPay = 0.0;

		Dictionary<string, double> profilesBalances = new Dictionary<string, double>();

		Dictionary<string, CustomEntry> payEntrys = new Dictionary<string, CustomEntry>();
		System.ComponentModel.PropertyChangedEventHandler action;

		public Grid SecondGrid { get { return secondGrid;} }

		public WeShareOverView(Conversation conv)
		{
			InitializeComponent();
			conversation = conv;
			foreach (Profile p in conv.Profiles)
			{

				totalExpense = 0.0;
				totalAmountYouMustPay = 0.0;
				totalAmountYouNeedToBePaid = 0.0;



				foreach (Comment weShareComment in conv.Messages)
				{
					if (weShareComment.weShareComment)
					{
						totalExpense += weShareComment.totalAmount;
						if (weShareComment.expensePaiedById == p.ProfileId)
						{
							if (weShareComment.uniqueAmounts.Count > 0)
							{
								if (weShareComment.uniqueAmounts.ContainsKey(p.ProfileId))
								{
									totalAmountYouNeedToBePaid += weShareComment.totalAmount - weShareComment.uniqueAmounts[p.ProfileId];
								}
								else {
									double othersPart = 0;

									foreach (KeyValuePair<string, double> entry in weShareComment.uniqueAmounts)
									{
										othersPart += entry.Value;
									}

									totalAmountYouNeedToBePaid += (((weShareComment.totalAmount - othersPart) / (conv.Profiles.Count - weShareComment.uniqueAmounts.Count)) * ((conv.Profiles.Count - weShareComment.uniqueAmounts.Count) - 1)) + othersPart;
								}
							}
							else {
								totalAmountYouNeedToBePaid += (weShareComment.totalAmount / conv.Profiles.Count) * (conv.Profiles.Count - 1);
							}
						}
						else {
							if (weShareComment.uniqueAmounts.Count > 0)
							{
								if (weShareComment.uniqueAmounts.ContainsKey(p.ProfileId))
								{
									totalAmountYouMustPay += weShareComment.uniqueAmounts[p.ProfileId];
								}
								else {
									double othersPart = 0;

									foreach (KeyValuePair<string, double> entry in weShareComment.uniqueAmounts)
									{
										othersPart += entry.Value;
									}

									totalAmountYouMustPay += (weShareComment.totalAmount - othersPart) / (conv.Profiles.Count - weShareComment.uniqueAmounts.Count);
								}
							}
							else
							{
								totalAmountYouMustPay += (weShareComment.totalAmount / conv.Profiles.Count);
							}
						}
					}



					//System.Diagnostics.Debug.WriteLine("totalAmountYouMustPay: " + totalAmountYouMustPay);
					//System.Diagnostics.Debug.WriteLine("totalAmountYouNeedToBePaid: " + totalAmountYouNeedToBePaid);
				}

				double sentByProfile = 0.0;
				double receivedToProfile = 0.0;

				foreach (Profile pro in conv.Profiles)
				{
					foreach (Tuple<string, string, string, StatusOptions> tuple in conv.subTypeDictionary[pro.ProfileId])
					{
						if (tuple != null && tuple.Item4 == StatusOptions.Completed)
						{
							if (pro.ProfileId == p.ProfileId)
							{
								sentByProfile += double.Parse(tuple.Item2);
							}
							else if(tuple.Item1 == p.ProfileId)
							{
								receivedToProfile += double.Parse(tuple.Item2);
							}
						}
					}

				}

				double balance = (totalAmountYouNeedToBePaid - totalAmountYouMustPay + sentByProfile - receivedToProfile);


				profilesBalances.Add(p.ProfileId, balance);

				firstGridTitle.Text = "Total expense: " + Math.Round(totalExpense, 2);

				//System.Diagnostics.Debug.WriteLine(pro.Name + ", total expense: " + totalExpense);
				//System.Diagnostics.Debug.WriteLine("Must pay total: " + totalAmountYouMustPay);
				//System.Diagnostics.Debug.WriteLine("Must be payed total: " + totalAmountYouNeedToBePaid);
				//System.Diagnostics.Debug.WriteLine(pro.Name + " balance: " + (totalAmountYouNeedToBePaid - totalAmountYouMustPay) + "(" + totalExpense + ")");


				//specialSettingLabels.Add(p.ProfileId, new CustomEntry() { HorizontalOptions = LayoutOptions.FillAndExpand, Placeholder = "0.0", WidthRequest = 70 });
				//specialSettingCancelButtons.Add(p.ProfileId, new Button() { HorizontalOptions = LayoutOptions.EndAndExpand, WidthRequest = 30, HeightRequest = 30, BackgroundColor = Color.Gray, Text = "X", TextColor = Color.White, BorderRadius = 15 });
				var specialSettingRow = new StackLayout() { Orientation = StackOrientation.Horizontal, Padding = new Thickness(0, 10, 0, 10) };
				specialSettingRow.Children.Add(new Label() { Text = p.Name, WidthRequest = 200, VerticalTextAlignment = TextAlignment.Center });
				specialSettingRow.Children.Add(new Label() { Text = Math.Round(balance,2) + "", WidthRequest = 70, VerticalTextAlignment = TextAlignment.Center, TextColor = balance > 0 ? Color.Green : Color.Red });
				//specialSettingRow.Children.Add(specialSettingCancelButtons[p.ProfileId]);
				balanceGrid.Children.Add(specialSettingRow);



			}
			if ((profilesBalances.ContainsKey(App.userProfile.ProfileId) && profilesBalances[App.userProfile.ProfileId] > 0.0)) { payRequestMyShare.Text = "Request your share"; }

			payRequestMyShare.Clicked += (sender, e) =>
			{
				secondGrid.IsVisible = true;
				firstGrid.IsVisible = false;


				Dictionary<string, Profile> profilesToRequestFrom = new Dictionary<string, Profile>();
				Dictionary<string, Profile> profilesToPay = new Dictionary<string, Profile>();
				double sumToBePaid = 0.0;
				double sumToPay = 0.0;

				foreach (Profile pro in conv.Profiles)
				{
					if (profilesBalances[pro.ProfileId] < 0.0)
					{
						profilesToRequestFrom.Add(pro.ProfileId, pro);
						sumToPay += profilesBalances[pro.ProfileId];
					}
					else if (profilesBalances[pro.ProfileId] > 0.0)
					{
						profilesToPay.Add(pro.ProfileId, pro);
						sumToBePaid += profilesBalances[pro.ProfileId];

					}


				}

				if (profilesBalances[App.userProfile.ProfileId] < 0.0)
				{
					secondGridTitle.Text = "You owe these profiles a total of " + Math.Round(Math.Abs(profilesBalances[App.userProfile.ProfileId]));
				}
				else if (profilesBalances[App.userProfile.ProfileId] > 0.0)
				{
					secondGridTitle.Text = "These profiles owe you a total of " + Math.Round(Math.Abs(profilesBalances[App.userProfile.ProfileId]));
				}



				if (profilesBalances[App.userProfile.ProfileId] > 0.0)
				{
					double myPercentageOfTotalToBePaid = (profilesBalances[App.userProfile.ProfileId] / sumToBePaid);
					System.Diagnostics.Debug.WriteLine(myPercentageOfTotalToBePaid);

					foreach (KeyValuePair<string, Profile> dic in profilesToRequestFrom)
					{
						System.Diagnostics.Debug.WriteLine(dic.Key + ", " + dic.Value.Name + " must pay me " + Math.Round(Math.Abs(profilesBalances[dic.Key] * myPercentageOfTotalToBePaid), 2));

						createPayGrid(dic.Value, profilesBalances[dic.Key], Math.Abs(profilesBalances[dic.Key] * myPercentageOfTotalToBePaid), Math.Abs(profilesBalances[App.userProfile.ProfileId]));
						//conv.amountsPaid[dic.Key].Add( Tuple.Create(App.userProfile.ProfileId, Math.Abs(profilesBalances[dic.Key] * myPercentageOfTotalToBePaid), "MobilePay", true) );
					}


				}
				else {
					double myPercentageOfTotalToPay = (profilesBalances[App.userProfile.ProfileId] / sumToPay);
					System.Diagnostics.Debug.WriteLine(myPercentageOfTotalToPay);

					foreach (KeyValuePair<string, Profile> dic in profilesToPay)
					{
						System.Diagnostics.Debug.WriteLine(dic.Key + ", " + dic.Value.Name + " must be paid " + Math.Round(Math.Abs(profilesBalances[dic.Key] * myPercentageOfTotalToPay), 2));

						createPayGrid(dic.Value, profilesBalances[dic.Key],Math.Abs(profilesBalances[dic.Key] * myPercentageOfTotalToPay), Math.Abs(profilesBalances[App.userProfile.ProfileId]));
						//conv.amountsPaid["1"].Add(Tuple.Create(dic.Key, Math.Abs(profilesBalances[dic.Key] * myPercentageOfTotalToPay), "MobilePay", true));
					}
				}

				//reloadView();
			};
		}

		void createPayGrid(Profile p, double floor, double suggestedPay, double totalPay)
		{
			var entryLayout = new StackLayout() { Padding = new Thickness(0, 8, 10, 8), VerticalOptions= LayoutOptions.Center };

			payEntrys.Add(p.ProfileId, new CustomEntry() { HorizontalOptions = LayoutOptions.FillAndExpand, Placeholder = "0.0", WidthRequest = 90, Text = Math.Round(suggestedPay,2)+"" });



			//specialSettingCancelButtons.Add(p.ProfileId, new Button() { HorizontalOptions = LayoutOptions.EndAndExpand, WidthRequest = 30, HeightRequest = 30, BackgroundColor = Color.Gray, Text = "X", TextColor = Color.White, BorderRadius = 15 });
			var specialSettingRow = new StackLayout() { Orientation = StackOrientation.Horizontal, Padding = new Thickness(0, 10, 0, 10) };


			var profileLayout = new StackLayout() { Orientation= StackOrientation.Horizontal, WidthRequest = 200, Spacing=0 };
			profileLayout.Children.Add(new Label() { Text = p.Name, VerticalTextAlignment = TextAlignment.Center });
			profileLayout.Children.Add(new Label() { Text = " (" + Math.Round(floor, 2) + ")", VerticalTextAlignment = TextAlignment.Center, TextColor = floor > 0 ? Color.Green : Color.Red });

			specialSettingRow.Children.Add(profileLayout);



			entryLayout.Children.Add(payEntrys[p.ProfileId]);
			specialSettingRow.Children.Add(entryLayout);
			var cashOption = new IconView() { Source = "ic_cash.png", Foreground = Color.Green, WidthRequest=40, HeightRequest=40 };
			var mobilePayOption = new Image() { Source = "ic_mobile_pay.png", WidthRequest = 50, HeightRequest = 50 };




			specialSettingRow.Children.Add(cashOption);
			specialSettingRow.Children.Add(mobilePayOption);

			//specialSettingRow.Children.Add(specialSettingCancelButtons[p.ProfileId]);
			payGrid.Children.Add(specialSettingRow);


			var tap = new TapGestureRecognizer();
			tap.Tapped += (sender, e) =>
			{

			};
			cashOption.GestureRecognizers.Add(tap);

			tap = new TapGestureRecognizer();
			tap.Tapped += async (sender, e) =>
			{
				bool cont = false;
				if (profilesBalances[App.userProfile.ProfileId] < 0.0)
				{
					cont = await App.rootPage.displayConfirmMessage("Confirm", "Would you like to continue to MobilePay to send " + p.Name + " " + double.Parse(payEntrys[p.ProfileId].Text), "Yes", "No");
					var str = "mobilepay://send?amount=" + double.Parse(payEntrys[p.ProfileId].Text) + "&phone=88888888";
					if (cont) Device.OpenUri(new Uri(String.Format(str.Replace(",", "."))));
				}
				else if (profilesBalances[App.userProfile.ProfileId] > 0.0)
				{
					cont = await App.rootPage.displayConfirmMessage("Confirm", "Would you like to continue to MobilePay to request " + double.Parse(payEntrys[p.ProfileId].Text) + " from " + p.Name, "Yes", "No");
					var str = "mobilepay://request?amount=" + double.Parse(payEntrys[p.ProfileId].Text) + "&phone=88888888";
					if (cont) Device.OpenUri(new Uri(String.Format(str.Replace(",", "."))));
				}
			};
			mobilePayOption.GestureRecognizers.Add(tap);



			payEntrys[p.ProfileId].Focused += async (sender, e) =>
			{
				foreach (KeyValuePair<string, CustomEntry> dic in payEntrys)
				{
					try
					{
						dic.Value.PropertyChanged -= action;
					}
					catch (Exception exc) { }
				}




				action = new System.ComponentModel.PropertyChangedEventHandler((sender2, e2) =>
				{
					try
					{
						if (string.IsNullOrWhiteSpace(payEntrys[p.ProfileId].Text) || double.Parse(payEntrys[p.ProfileId].Text) < 0.0)
						{
							payEntrys[p.ProfileId].Text = "";
						}

						else 
						{
							if (double.Parse(payEntrys[p.ProfileId].Text) > Math.Abs(floor) || double.Parse(payEntrys[p.ProfileId].Text) > totalPay)
							{
								payEntrys[p.ProfileId].Text = Math.Round(Math.Min(floor, totalPay), 2) + "";
							}

						}
						 



						payEntrys[p.ProfileId].TextColor = Color.Green;
					}
					catch (Exception exc)
					{
						payEntrys[p.ProfileId].TextColor = Color.Red;
					}
				});

				payEntrys[p.ProfileId].PropertyChanged += action;

				while (true)
				{
					await Task.Delay(100);
					if (!payEntrys[p.ProfileId].IsFocused)
					{
						payEntrys[p.ProfileId].PropertyChanged -= action;
						System.Diagnostics.Debug.WriteLine("BREAKED");
						break;
					}
				}
			};
		}

		public void returnToFirstFrid() {
			secondGrid.IsVisible = false;
			reloadView();
		}
	}
}
