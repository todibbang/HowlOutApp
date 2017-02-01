using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Xamarin.Forms.Maps;
using System.Text.RegularExpressions;
using System.Linq;
namespace HowlOut
{
	public partial class InspectEvent : ContentView
	{
		ObservableCollection<Comment> comments = new ObservableCollection<Comment>();
		bool mapInitialized = false;
		ExtMap map = new ExtMap() { IsShowingUser = true, VerticalOptions = LayoutOptions.FillAndExpand };
		DataManager _dataManager = new DataManager();
		EventForLists efl;

		public InspectEvent(Event eve, bool inspectType, ScrollView scrollView)
		{
			InitializeComponent();
			efl = new EventForLists(eve);
			BindingContext = efl;
			_dataManager = new DataManager();
			setInfo(eve);


			mapBtn.Clicked += (sender, e) =>
			{
				mapGrid.IsVisible = !mapGrid.IsVisible;
			};

			scrollView.Scrolled += (sender, e) =>
			{
				if (scrollView.ScrollY > 0)
				{
					bannerElement.TranslationY = Math.Abs(scrollView.ScrollY) / 3.0;
				}
				else {
					bannerElement.Scale = 1 + (Math.Abs(scrollView.ScrollY) / 100.0);
					bannerElement.TranslationY = scrollView.ScrollY;
				}

				if (scrollView.ScrollY < 100 && scrollView.ScrollY > 0)
				{
					//titleInfo.TranslationY = scrollView.ScrollY;
				}
			};

			mapButton.Clicked += (sender, e) =>
			{
				string lat = (eve.Latitude + "").Replace(",", ".");
				string lon = (eve.Longitude + "").Replace(",", ".");
				Device.OpenUri(new Uri("http://maps.apple.com/?daddr=" + lat + "," + lon));
			};

			shareOption.Clicked += (sender, e) =>
			{
				App.coreView.DisplayShare(eve);
			};

			if (eve.Followers.Exists(p => p.ProfileId == App.StoredUserFacebookId)) followOptionBg.BackgroundColor = App.HowlOut;
			followOption.Clicked += (sender, e) =>
			{
				_dataManager.AttendTrackEvent(eve.EventId, !eve.Followers.Exists(p => p.ProfileId == App.StoredUserFacebookId), false);
			};

			PayButton.Clicked += (sender, e) =>
			{
				//Device.OpenUri(new Uri(String.Format("mobilepay://send?amount=100&phone=88888888")));

				/*
				Conversation con = new Conversation() { ModelType = ConversationModelType.None };
				con.Messages.Add(new Comment() { });
				App.coreView.setContentViewWithQueue(new ConversationView(new Conversation() {ModelType = ConversationModelType.None }, true)); */
			};

			if (!_dataManager.IsEventJoined(eve) && (eve.ProfileOwners == null || (eve.ProfileOwners != null && !eve.ProfileOwners.Exists(p => p.ProfileId == App.userProfile.ProfileId))))
			{
				JoinLeaveButton.Clicked += (sender, e) =>
				{
					_dataManager.AttendTrackEvent(eve.EventId, true, true);
				};
			}
			else {
				JoinLeaveButton.IsVisible = false;
				UpdateToolBox(eve);
			}

			if (App.userProfile.EventsInviteToAsOwner.Exists(e => e.EventId == eve.EventId))
			{
				invitedOwnerLayout.IsVisible = true;
				AcceptOwnerButton.Clicked += async (sender, e) =>
				{
					await _dataManager.EventApiManager.AcceptDeclineLeaveEventAsOwner(eve.EventId, OwnerHandlingType.Accept);
					App.coreView.setContentView(1);
					App.coreView.setContentViewWithQueue(new InspectController(await App.coreView._dataManager.EventApiManager.GetEventById(eve.EventId)));
				};
				DeclineOwnerButton.Clicked += async (sender, e) =>
				{
					await _dataManager.EventApiManager.AcceptDeclineLeaveEventAsOwner(eve.EventId, OwnerHandlingType.Decline);
					App.coreView.setContentView(1);
					App.coreView.setContentViewWithQueue(new InspectController(await App.coreView._dataManager.EventApiManager.GetEventById(eve.EventId)));
				};
			}

			BannerImage.Clicked += (sender, e) =>
			{
				OtherFunctions of = new OtherFunctions();
				of.ViewImages(new List<string>() { eve.ImageSource });
			};

			if (mapInitialized != true)
			{
				mapInitialized = true;
				_dataManager.UtilityManager.setMapForEvent(new Position(eve.Latitude, eve.Longitude), map, mapLayout, 1.2);
				_dataManager.UtilityManager.setPin(new Position(eve.Latitude, eve.Longitude), map, eve.Title, eve.AddressName);
			}

			TapGestureRecognizer tgr = new TapGestureRecognizer();
			tgr.Tapped += (sender, e) =>
			{
				App.coreView.setContentViewWithQueue(new ListsAndButtons(eve.ProfileOwners, null, false, false));
			};
			ownerImg.GestureRecognizers.Add(tgr);

			groupOwnerBtn.Clicked += (sender, e) =>
			{
				App.coreView.GoToSelectedGroup(eve.GroupOwner.GroupId);
			};

			toolboxBtn.Clicked += (sender, e) =>
			{
				App.coreView.setContentViewWithQueue(new YourConversations(ConversationModelType.Event, eve.EventId, 0));
			};
		}

		public async void setInfo(Event eve)
		{
			quickInfo.IsVisible = true;
			eventDescription.Text = eve.Description;
			string[] addressList = new string[3];
			addressList = Regex.Split(eve.AddressName, ",");
			for (int i = 0; i < addressList.Length; i++)
			{
				Label label = new Label() { TextColor = Color.FromHex("646464") };
				label.Text = addressList[i].Trim();
				label.FontSize = 14;
				addressLayout.Children.Add(label);
			}
			if (addressList.Length == 2)
			{
				//bottomDist.Text = addressList[0].Substring(5).Trim();
			}
			else {
				//bottomDist.Text = addressList[1].Substring(5).Trim();
			}
		}

		async void UpdateToolBox(Event eve)
		{
			toolBoxGrid.IsVisible = true;

			var conList = await _dataManager.MessageApiManager.GetConversations(eve.EventId, ConversationModelType.Event);


			/* TEST START */

			var pTest = new List<Profile>
					{
						new Profile{ProfileId = "10153817903667221", Name="Tob B Bang"},
						new Profile{ProfileId = "191571161232364", Name="Emma Stone"}
					};

			{

				Conversation conv = new Conversation()
				{
					ModelType = ConversationModelType.Profile,
					ConversationID = "12344567",
					Messages = new List<Comment>(),
					LastMessage = null,
					ModelId = "23456",
					Profiles = pTest,
					Title = "ExpenShare Test",
					SubType = ConversationSubType.ExpenShare,
					subTypeDictionary = new Dictionary<string, List<Tuple<string, string, string, StatusOptions>>>()
				};

				foreach (Profile p in conv.Profiles)
				{
					conv.subTypeDictionary.Add(p.ProfileId, new List<Tuple<string, string, string, StatusOptions>>());
				}
				conList.Add(conv);
			}
			{


				var dick = new Dictionary<string, List<Tuple<string, string, string, StatusOptions>>>();
				dick.Add("Options", new List<Tuple<string, string, string, StatusOptions>>()
					{ Tuple.Create("0","0","", StatusOptions.Confirmed) });
				dick.Add("OptionSettings", new List<Tuple<string, string, string, StatusOptions>>()
						{ Tuple.Create("0","0","", StatusOptions.Confirmed), });
				foreach (Profile p in pTest) {
					dick.Add(p.ProfileId, new List<Tuple<string, string, string, StatusOptions>>()); }
				Conversation conv = new Conversation()
				{
					ModelType = ConversationModelType.Profile,
					ConversationID = "12344568",
					Messages = new List<Comment>(),
					LastMessage = null,
					ModelId = "234567",
					Profiles = pTest,
					Title = "Doodle Test",
					SubType = ConversationSubType.Doodle,
					subTypeDictionary = dick,
				};
				conList.Add(conv);
			}

			{
				Conversation conv = new Conversation { subTypeDictionary = new Dictionary<string, List<Tuple<string, string, string, StatusOptions>>>(), Profiles = pTest, SubType = ConversationSubType.ToDoList, Title = "To Do list test" };
				conv.subTypeDictionary.Add("ToDoList", new List<Tuple<string, string, string, StatusOptions>>());
				conv.subTypeDictionary.Add("ToDoListInfo", new List<Tuple<string, string, string, StatusOptions>>());
				conv.subTypeDictionary["ToDoList"].Add(Tuple.Create("", "0", "", StatusOptions.NotStarted));
				conv.subTypeDictionary["ToDoListInfo"].Add(Tuple.Create("1", "0", "", StatusOptions.NotStarted));
				conList.Add(conv);
			}


			/* TEST END */

			if (conList.Count > 1) conList = conList.OrderByDescending(c => c.LastUpdated).ToList();


			for (int i = 0; i < Math.Min(3, conList.Count); i++)
			{
				var grid = new Grid() { };
				var sl = new StackLayout() { Orientation = StackOrientation.Horizontal };
				var btn = new Button();
				var con = conList[i];
				grid.Children.Add(sl);
				grid.Children.Add(btn);
				if (con.SubType == ConversationSubType.Doodle)
				{
					sl.Children.Add(new IconView() { Source = "ic_vote.png", Foreground = App.HowlOut, HeightRequest=30, WidthRequest=30, HorizontalOptions = LayoutOptions.Start });
					btn.Clicked += (sender, e) =>
					{
						App.coreView.setContentViewWithQueue(new DoodleView(con));
					};
				}
				else if (con.SubType == ConversationSubType.ToDoList)
				{
					sl.Children.Add(new IconView() { Source = "ic_to_do.png", Foreground = App.HowlOut, HeightRequest = 30, WidthRequest = 30, HorizontalOptions = LayoutOptions.Start });
					btn.Clicked += (sender, e) =>
					{
						App.coreView.setContentViewWithQueue(new ToDoListView(con));
					};
				}
				else if (con.SubType == ConversationSubType.ExpenShare)
				{
					sl.Children.Add(new IconView() { Source = "ic_expense_share.png", Foreground = App.HowlOut, HeightRequest = 30, WidthRequest = 30, HorizontalOptions = LayoutOptions.Start });
					btn.Clicked += (sender, e) =>
					{
						App.coreView.setContentViewWithQueue(new ConversationView(con));
					};
				}
				else 
				{
					sl.Children.Add(new IconView() { Source = "ic_message.png", Foreground = App.HowlOut, HeightRequest = 30, WidthRequest = 30, HorizontalOptions = LayoutOptions.Start });
					btn.Clicked += (sender, e) =>
					{
						App.coreView.setContentViewWithQueue(new ConversationView(con));
					};
				}
				sl.Children.Add(new Label() { Text = con.Title, VerticalOptions= LayoutOptions.Center });
				toolBoxOverView.Children.Add(grid);
			}
		}
	}
}