using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace HowlOut
{
	public partial class ConversationView : ContentView, ViewModelInterface
	{
		private DataManager _dataManager = new DataManager();
		public Conversation conversation;
		public ListView ConversationList { get { return conversationList; } }
		string title;

		public List<Comment> comments;
		private StackLayout wallLayout;
		List<Profile> profilesAdded;

		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}

		bool viewInFocusNow = false;
		bool WeShare = false;
		double totalExpense = 0.0;
		double totalAmountYouNeedToBePaid = 0.0;
		double totalAmountYouMustPay = 0.0;
		int previousCount = 0;
		private double listHeight;

		public MessageApiManager.CommentType type;
		public string ConversationId;

		public ConversationView(List<Comment> coms, MessageApiManager.CommentType mT, string id, StackLayout layout)
		//public ConversationView(List<Comment> coms, MessageApiManager.CommentType mT, string id)
		{
			try
			{
				comments = coms;
				type = mT;
				ConversationId = id;
				wallLayout = layout;

				InitializeComponent();
				commentView.IsVisible = true;

				entryTop.IsVisible = true;
				commentList.ItemSelected += OnItemSelected;




				UpdateList(true);
				postCommentButtonTop.Clicked += async (sender, e) => { await PostNewComment(commentEntryTop.Text); };
				setSize(false);
			}
			catch (Exception exc) { reloadView(); }
		}

		public ConversationView(Conversation conv)
		{
			try
			{
				InitializeComponent();
				if (conv.SubType == ConversationSubType.ExpenShare)
				{
					WeShare = true;
					conversationList.ItemTemplate = new DataTemplate(() => { return new ViewCell { View = new WeShareMessageTemplate() }; });
					//conv = testWeShareConversation();
					newExpenseButton.IsVisible = true;
					type = MessageApiManager.CommentType.Converzation;
					newExpenseButton.Clicked += (sender, e) =>
					{
						App.coreView.setContentViewWithQueue(new NewExpenseView(conversation, null));
					};
				}
				conversation = conv;
				//


				conversationScrollView.IsVisible = true;
				if (conversation.ModelType != ConversationModelType.Profile)
				{
					setConversationInfo(conv);
				}
				entryBottom.IsVisible = true;
				conversationList.ItemSelected += OnItemSelected;
				type = MessageApiManager.CommentType.Converzation;
				ConversationId = conv.ConversationID;

				UpdateList(true);
				setSize(true);



				bool scrollDelay = false;
				commentEntryBottom.Focused += async (sender, e) =>
				{
					conversationList.TranslationY = 90;
					conversationListLayoutHeader.HeightRequest = 300;
					conversationScrollView.HeightRequest = 1000;
					conversationInfo.IsVisible = false;
					try
					{
						await Task.Delay(1);
						if (conversationList.ItemsSource.OfType<Comment>().Last() != null)
						{
							conversationList.ScrollTo(conversationList.ItemsSource.OfType<Comment>().Last(), ScrollToPosition.End, true);
						}
					}
					catch (Exception ex) { }

					scrollDelay = true;
					await Task.Delay(200);
					scrollDelay = false;

				};
				commentEntryBottom.Unfocused += (sender, e) =>
				{
					conversationList.TranslationY = 0;
					conversationListLayoutHeader.HeightRequest = 60;
				};

				conversationScrollView.Scrolled += (sender, e) =>
				{
					System.Diagnostics.Debug.WriteLine(conversationScrollView.ScrollY);
					if (conversationScrollView.ScrollY > 0 && conversationScrollView.ScrollY < 100 && !scrollDelay)
					{
						DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
					}

					if (conversationScrollView.ScrollY < 10 && !scrollDelay)
					{
						if (conversation.ModelType != ConversationModelType.Profile)
						{
							conversationInfo.IsVisible = true;
						}
					}
				};
				App.coreView.viewdConversation = this;

				var pictureImage = new TapGestureRecognizer();
				pictureImage.Tapped += async (sender, e) =>
				{
					try
					{
						var imageStreams = await _dataManager.UtilityManager.TakePicture(null);
						if (imageStreams != null)
						{
							var imageString = await _dataManager.UtilityManager.UploadImageToStorage(new MemoryStream(imageStreams[2]), App.StoredUserFacebookId + "." + DateTime.Now.ToString("G"));
							PostNewComment(".IMAGESTART." + imageString + ".IMAGEEND.");
						}
					}
					catch (Exception ex) { }
				};
				takePictureButton.GestureRecognizers.Add(pictureImage);

				var albumImage = new TapGestureRecognizer();
				albumImage.Tapped += async (SenderOfEvent, e) =>
				{
					try
					{
						var imageStreams = await _dataManager.UtilityManager.PictureFromAlbum(null);
						if (imageStreams != null)
						{
							var imageString = await _dataManager.UtilityManager.UploadImageToStorage(new MemoryStream(imageStreams[2]), App.StoredUserFacebookId + "." + DateTime.Now.ToString("G"));
							PostNewComment(".IMAGESTART." + imageString + ".IMAGEEND.");
						}
					}
					catch (Exception ex) { }
				};
				albumPictureButton.GestureRecognizers.Add(albumImage);
			}
			catch (Exception exc){ reloadView(); }
		}

		public async void viewInFocus(UpperBar bar)
		{
			viewInFocusNow = true;
			autoUpdater();
			if (conversation == null) return;

			title = "";

			if (conversation.Profiles != null)
			{
				foreach (Profile p in conversation.Profiles)
				{
					if (p.ProfileId != App.StoredUserFacebookId)
					{
						if (conversation.Profiles.Count > 2)
						{
							title += p.Name.Split(' ')[0] + ", ";
						}
						else {
							title += p.Name;
						}

						if (conversationInfoImage.Source == null && conversation.ModelType == ConversationModelType.Profile)
						{
							conversationInfoImage.Source = p.ImageSource;
						}
					}
				}

				if (conversation.Profiles.Count > 2)
				{
					title = title.Substring(0, title.Length - 2);
				}
			}
			if (!string.IsNullOrWhiteSpace(conversation.Title))
			{
				title = conversation.Title;
			}

			App.coreView.topBar.setRightButton("ic_menu.png").Clicked += async (sender, e) =>
			{
				List<Action> actions = new List<Action>();
				List<string> titles = new List<string>();
				List<string> images = new List<string>();

				actions.Add(() => { App.coreView.setContentViewWithQueue(new InviteListView(conversation, false)); });
				titles.Add("Invite");
				images.Add("ic_add_profiles.png");

				actions.Add(async () =>
				{
					editTitleGrid.IsVisible = !editTitleGrid.IsVisible;

				});
				titles.Add("Change Title");
				images.Add("ic_title.png");

				actions.Add(async () =>
				{
					bool success = await _dataManager.MessageApiManager.leaveConversation(conversation.ConversationID);
					if (success) { App.coreView.returnToPreviousView(); }
					else { await App.coreView.displayAlertMessage("Error", "Error", "Ok"); }
				});
				titles.Add("Leave");
				images.Add("ic_settings.png");

				await App.coreView.DisplayOptions(actions, titles, images);
			};

			bar.setNavigationlabel(title).Clicked += (sender, e) =>
			{
				if (WeShare)
				{
					App.coreView.setContentViewWithQueue(new WeShareOverView(conversation));
				}
				else {
					App.coreView.setContentViewWithQueue(new ListsAndButtons(conversation.Profiles, null, false, false));
				}
			};

			if (type == MessageApiManager.CommentType.Converzation)
			{
				try
				{
					await Task.Delay(2);
					conversationList.ScrollTo(conversationList.ItemsSource.OfType<Comment>().Last(), ScrollToPosition.End, true);
				}
				catch (Exception e) { }
			}
		}
		public void reloadView() {
			if (conversation != null)
			{
				this.content = new ConversationView(conversation);
			}
			else {
				this.content = new ConversationView(comments, type, ConversationId, wallLayout);
			}
		}
		public void viewExitFocus() { viewInFocusNow = false; }
		public ContentView getContentView() { return this; }

		async void setConversationInfo(Conversation c)
		{
			try
			{
				TapGestureRecognizer t = new TapGestureRecognizer();

				conversationInfo.IsVisible = true;
				if (c.ModelType == ConversationModelType.Event)
				{
					conversationInfoProfilesButton.Clicked += (sender, e) =>
					{
						App.coreView.GoToSelectedEvent(c.ModelId);
					};

					Event eve = await _dataManager.EventApiManager.GetEventById(c.ModelId);
					conversationInfoImage.Source = eve.ImageSource;
					conversationInfoModelLabel.Text = "Event: " + eve.Title;
				}
				else if (c.ModelType == ConversationModelType.Group)
				{
					conversationInfoProfilesButton.Clicked += (sender, e) =>
					{
						App.coreView.GoToSelectedGroup(c.ModelId);
					};

					Group grp = await _dataManager.GroupApiManager.GetGroup(c.ModelId);
					conversationInfoImage.Source = grp.ImageSource;
					conversationInfoModelLabel.Text = "Group: " + grp.Name;
				}
				else {
					conversationInfo.IsVisible = false;
					conversationInfoProfilesButton.IsVisible = false;
				}

				closeEditTitle.Clicked += (sender, e) =>
				{
					editTitleGrid.IsVisible = !editTitleGrid.IsVisible;
				};
				editTitleButton.Clicked += async (sender, e) =>
				{
					await _dataManager.MessageApiManager.EditConversationTitle(conversation.ConversationID, editTitleEntry.Text);
					c = await _dataManager.MessageApiManager.GetOneConversation(c.ConversationID);
					App.coreView.setContentViewReplaceCurrent(new ConversationView(c), 1);
				};
			}
			catch (Exception exc) {}
		}

		private async Task setSize(bool add)
		{
			await Task.Delay(10);
			outerGrid.HeightRequest = App.coreView.Height;// - (62);
														  //commentList.HeightRequest = outerGrid.HeightRequest - 80;
														  //conversationList.HeightRequest = outerGrid.HeightRequest - 80;
														  //commentListLayout.HeightRequest = commentListLayout.Height;
														  //conversationListLayout.HeightRequest = conversationListLayout.Height;

			//listHeight = commentList.HeightRequest;
			//listHeight = commentList.HeightRequest;
			if (add) App.coreView.addConversationViewToActiveList(this);
		}

		public async void pushPostNewComment()
		{
			await PostNewComment(commentEntryBottom.Text);
		}

		private async Task PostNewComment(string comment)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(comment))
				{
					//TODO changed this to recieve comment object instead of event
					var commentObj = new Comment
					{
						Content = comment,
						SenderId = App.StoredUserFacebookId,
						DateAndTime = DateTime.Now.ToLocalTime(),
						ImageSource = App.userProfile.ImageSource
					};

					bool success = false;

					comments.Add(commentObj);
					UpdateList(false);

					commentEntryTop.Text = "";
					commentEntryBottom.Text = "";

					//commentEntryBottom.
					commentObj.conversation = null;
					if (type == MessageApiManager.CommentType.Converzation)
					{
						Conversation cons = await _dataManager.MessageApiManager.WriteToConversation(ConversationId, commentObj);
						if (cons != null)
						{
							success = true;
							comments = cons.Messages;
						}
					}
					else {

						List<Comment> coms = await _dataManager.MessageApiManager.CreateComment(ConversationId, type, commentObj);
						if (coms != null)
						{
							success = true;
							comments = coms;
						}
					}

					if (success)
					{
						UpdateList(true);
					}
					else {
						await App.coreView.displayAlertMessage("Comment Not Posted", "An error happened and the comment was not posted, try again.", "Ok");
					}
				}
			}
			catch (Exception exc)
			{
				System.Diagnostics.Debug.WriteLine(exc.Message);
			}
		}

		public async Task<bool> UpdateList(bool update)
		{
			try
			{



				if (update)
				{
					if (!WeShare)
					{
						if (type == MessageApiManager.CommentType.Converzation)
						{
							conversation = await _dataManager.MessageApiManager.GetOneConversation(ConversationId);
							if (conversation.Messages.Count > 0 && comments != null && comments.Count == conversation.Messages.Count) return true;
							comments = conversation.Messages;
						}
						else {

						}
					}
					else {
						//conversation = testWeShareConversation();

					}


					//

					viewInFocus(App.coreView.topBar);
				}

				if(conversation != null) comments = conversation.Messages;


				if (comments == null || comments != null && comments.Count == previousCount) return true;

				//if (WeShare) weShareCalculator(conversation);

				try
				{
					previousCount = comments.Count;
				}
				catch (Exception exc) { }

				if (conversation != null)
				{
					foreach (Comment c in comments)
					{
						c.conversation = conversation;
					}
				}

				if (type == MessageApiManager.CommentType.Converzation)
				{
					comments = comments.OrderBy(c => c.DateAndTime).ToList();
					conversationList.ItemsSource = comments;
					conversationList.IsRefreshing = false;
				}
				else {
					comments = comments.OrderByDescending(c => c.DateAndTime).ToList();
					commentList.ItemsSource = comments;
					commentList.IsRefreshing = false;
				}

				/*
				if (WeShare)
				{
					weShareCalculator(conversation);
				} */


				await Task.Delay(1000);
				if (type == MessageApiManager.CommentType.Converzation)
				{
					conversationList.ScrollTo(conversationList.ItemsSource.OfType<Comment>().Last(), ScrollToPosition.End, false);
				}
				else {
					commentList.HeightRequest = comments.Count * 120 + 100;
					listViewRow.Height = comments.Count * 120 + 100;
					wallLayout.HeightRequest = comments.Count * 120 + 100;
					commentList.ScrollTo(commentList.ItemsSource.OfType<Comment>().First(), ScrollToPosition.Start, true);
				}

			}
			catch (Exception exc) { reloadView(); }
			return true;
		}

		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			try
			{
				if (WeShare)
				{
					try
					{
						if (conversationList.SelectedItem == null) { return; }
						var selectedCom = conversationList.SelectedItem as Comment;
						if (selectedCom == null) return;

						App.coreView.setContentViewWithQueue(new NewExpenseView(conversation, selectedCom));
						/*
						double othersPart = 0;

						System.Diagnostics.Debug.WriteLine("expense paid by: " + selectedCom.expensePaiedById);
						foreach (KeyValuePair<string, double> entry in selectedCom.uniqueAmounts)
						{
							System.Diagnostics.Debug.WriteLine(entry.Key + "'s part is " + entry.Value);
							othersPart += entry.Value;
						}
						System.Diagnostics.Debug.WriteLine("The rests parts are " + (selectedCom.totalAmount - othersPart) / (conversation.Profiles.Count - selectedCom.uniqueAmounts.Count));

						*/
						conversationList.SelectedItem = null;
					}
					catch (Exception ex) {
						System.Diagnostics.Debug.WriteLine(ex.Message);
					}
				}

				commentList.SelectedItem = null;
				conversationList.SelectedItem = null;
				DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			}
			catch (Exception exc) {}
		}

		private async Task autoUpdater()
		{
			await Task.Delay(100);
			try
			{
				UpdateList(false);
				if (viewInFocusNow)
				{
					autoUpdater();
				}
			}
			catch (Exception exc)
			{
				System.Diagnostics.Debug.WriteLine("Error found here");
				autoUpdater();
			}
			await Task.Delay(5000);
		}

		Conversation testWeShareConversation()
		{
			Random rnd = new Random();

			Conversation con = new Conversation()
			{
				Profiles = new List<Profile>
				{
					new Profile{ProfileId = "1", Name="Tob"},
					new Profile{ProfileId = "2", Name="Wills"},
					new Profile{ProfileId = "3", Name="Peiter"},
					new Profile{ProfileId = "4", Name="Pernille"},
					new Profile{ProfileId = "5", Name="Mor"},
				}, 
				ModelType = ConversationModelType.Profile
			};

			con.Messages = new List<Comment>();
			con.subTypeDictionary = new Dictionary<string, List<Tuple<string, string, string, StatusOptions>>>();
			//con.AmountPaiedByProfile = new Dictionary<string, double>();
			//con.AmountReceivedByProfile = new Dictionary<string, double>();

			foreach (Profile p in con.Profiles)
			{
				con.subTypeDictionary.Add(p.ProfileId, new List<Tuple<string, string, string, StatusOptions>>());
			}

			for (int i = 0; i < 20; i++)
			{
				Comment com = new Comment();
				com.ImageSource = "";



				if (rnd.Next(1, 5) == 1)
				{
					com.Content = "Mad og vin :) .IMAGESTART.img10.jpeg.IMAGEEND.";
				}
				else {
					com.Content = "Mad og vin :)";
				}



				com.expensePaiedById = rnd.Next(1, 6) + "";
				com.SenderId = com.expensePaiedById;
				com.totalAmount = rnd.Next(0,1000);
				//com.uniqueAmounts = new Dictionary<string, double>();
				//System.Diagnostics.Debug.WriteLine(com.expensePaiedById);

				if (rnd.Next(1, 3) == 1)
				{
					com.Content += ".UNIQUEAMOUNTSSTART.";
					int uniqueKey1 = rnd.Next(1, 6);
					int uniqueValue1 = rnd.Next(0, (int)com.totalAmount);

					//com.uniqueAmounts.Add(uniqueKey1 + "", uniqueValue1);


					com.Content += rnd.Next(1, 6) + ".UA." + rnd.Next(0, (int)com.totalAmount) + ".UNIQUEAMOUNT.";

					if (rnd.Next(1, 2) == 1)
					{
						int uniqueKey2 = rnd.Next(1, 6);
						int uniqueValue2 = rnd.Next(0, (int)com.totalAmount - uniqueValue1);
						//if (uniqueKey2 != uniqueKey1) com.uniqueAmounts.Add(uniqueKey2 + "", uniqueValue2);

						com.Content += rnd.Next(1, 6) + ".UA." + rnd.Next(0, (int)com.totalAmount - uniqueValue1) + ".UNIQUEAMOUNT.";
					}
					com.Content += ".UNIQUEAMOUNTSEND.";
				}
				com.DateAndTime = System.DateTime.Now;

				con.Messages.Add(com);
			}



			//weShareCalculator(con);
			return con;
		}

		/*
		private void weShareCalculator(Conversation con){

			System.Diagnostics.Debug.WriteLine("\n\n");

			foreach (Profile pro in con.Profiles)
			{
				totalExpense = 0.0;
				totalAmountYouMustPay = 0.0;
				totalAmountYouNeedToBePaid = 0.0;



				foreach (Comment weShareComment in con.Messages)
				{
					if (weShareComment.weShareComment)
					{
						totalExpense += weShareComment.totalAmount;
						if (weShareComment.expensePaiedById == pro.ProfileId)
						{
							if (weShareComment.uniqueAmounts.Count > 0)
							{
								if (weShareComment.uniqueAmounts.ContainsKey(pro.ProfileId))
								{
									totalAmountYouNeedToBePaid += weShareComment.totalAmount - weShareComment.uniqueAmounts[pro.ProfileId];
								}
								else {
									double othersPart = 0;

									foreach (KeyValuePair<string, double> entry in weShareComment.uniqueAmounts)
									{
										othersPart += entry.Value;
									}

									totalAmountYouNeedToBePaid += (((weShareComment.totalAmount - othersPart) / (conversation.Profiles.Count - weShareComment.uniqueAmounts.Count)) * ((conversation.Profiles.Count - weShareComment.uniqueAmounts.Count) - 1)) + othersPart;
								}
							}
							else {
								totalAmountYouNeedToBePaid += (weShareComment.totalAmount / conversation.Profiles.Count) * (conversation.Profiles.Count - 1);
							}
						}
						else {
							if (weShareComment.uniqueAmounts.Count > 0)
							{
								if (weShareComment.uniqueAmounts.ContainsKey(pro.ProfileId))
								{
									totalAmountYouMustPay += weShareComment.uniqueAmounts[pro.ProfileId];
								}
								else {
									double othersPart = 0;

									foreach (KeyValuePair<string, double> entry in weShareComment.uniqueAmounts)
									{
										othersPart += entry.Value;
									}

									totalAmountYouMustPay += (weShareComment.totalAmount - othersPart) / (conversation.Profiles.Count - weShareComment.uniqueAmounts.Count);
								}
							}
							else
							{
								totalAmountYouMustPay += (weShareComment.totalAmount / conversation.Profiles.Count);
							}
						}
					}
					//System.Diagnostics.Debug.WriteLine("totalAmountYouMustPay: " + totalAmountYouMustPay);
					//System.Diagnostics.Debug.WriteLine("totalAmountYouNeedToBePaid: " + totalAmountYouNeedToBePaid);
				}

				//System.Diagnostics.Debug.WriteLine(pro.Name + ", total expense: " + totalExpense);
				//System.Diagnostics.Debug.WriteLine("Must pay total: " + totalAmountYouMustPay);
				//System.Diagnostics.Debug.WriteLine("Must be payed total: " + totalAmountYouNeedToBePaid);
				System.Diagnostics.Debug.WriteLine(pro.Name + " balance: " + (totalAmountYouNeedToBePaid - totalAmountYouMustPay) + "(" + totalExpense + ")");
			}
		}
*/
	}
}
