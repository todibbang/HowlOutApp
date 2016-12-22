using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Http;
using ModernHttpClient;

namespace HowlOut
{
	public partial class InspectEvent : ContentView
	{
		ObservableCollection<Comment> comments = new ObservableCollection<Comment>();
		bool mapInitialized = false;
		ExtMap map = new ExtMap () { IsShowingUser = true, VerticalOptions = LayoutOptions.FillAndExpand };
		DataManager _dataManager = new DataManager();
		EventForLists efl;

		public InspectEvent (Event eve, bool inspectType, ScrollView scrollView)
		{
			InitializeComponent ();
			efl = new EventForLists(eve);
			BindingContext = efl;
			_dataManager = new DataManager();
			setInfo (eve);
			/*
			if (_dataManager.IsEventJoined(eve) || _dataManager.IsEventYours(eve))  { 	
				//trackImg.IsVisible = false; 
				inviteImg.IsVisible = true; } 
			else  { 
				//trackImg.IsVisible = true; 
				inviteImg.IsVisible = false; 
			} */


			mapBtn.Clicked += (sender, e) =>
			{
				mapGrid.IsVisible = !mapGrid.IsVisible;
			};

			/*
			detailsButton.Clicked += (sender, e) => 
			{
				if(detailedInfo.IsVisible == false) { 
					detailedInfo.IsVisible = true; 
					quickInfo.IsVisible = false;
					detailsButton.Text = "Hide Map";
					App.coreView.otherFunctions.scrollTo(100);
				} 
				else { 
					detailedInfo.IsVisible = false; 
					quickInfo.IsVisible = true;
					detailsButton.Text = "Show Map";
					App.coreView.otherFunctions.scrollTo(200);
				}

				if(mapInitialized != true) { 
					mapInitialized = true;
					_dataManager.UtilityManager.setMapForEvent (new Position(eve.Latitude, eve.Longitude), map, mapLayout);
					_dataManager.UtilityManager.setPin(new Position(eve.Latitude, eve.Longitude), map, eve.Title, eve.AddressName);
				}
			}; */

			scrollView.Scrolled += (sender, e) =>
			{
				


				if (scrollView.ScrollY > 0)
				{
					bannerElement.TranslationY = Math.Abs(scrollView.ScrollY) / 3.0;
				}
				else {
					bannerElement.Scale = 1 + (Math.Abs(scrollView.ScrollY) / 100.0);
					//if (bannerElement.Scale > 2) bannerElement.Scale = 2;
					bannerElement.TranslationY = scrollView.ScrollY;
					bannerElementBackground.TranslationY = scrollView.ScrollY;
					//titleInfo.TranslationY = scrollView.ScrollY;
				}


				if (scrollView.ScrollY < 100 && scrollView.ScrollY > 0)
				{
					//titleInfo.TranslationY = scrollView.ScrollY;
				}
			};
				
			mapButton.Clicked += (sender, e) => {
				App.coreView.setContentViewWithQueue(new MapsView(eve));
			};

			//JoinLeaveButton.IsVisible = false;
			//editImg.IsVisible = true;

			/*
			if (eve.Attendees.Exists(p => p.ProfileId == App.StoredUserFacebookId) || eve.ProfileOwners.Exists(p => p.ProfileId == App.StoredUserFacebookId))
			{
				JoinLeaveButton.IsVisible = false;
			}
			else {
				JoinLeaveButton.Clicked += (sender, e) =>
				{
					_dataManager.AttendTrackEvent(eve, true, true);
				};
			} */

			if (!_dataManager.IsEventJoined(eve))
			{
				JoinLeaveButton.Clicked += (sender, e) =>
				{
					_dataManager.AttendTrackEvent(eve, true, true);
				};
			}
			else {
				JoinLeaveButton.IsVisible = false;
			}

			if (App.userProfile.EventsInviteToAsOwner.Exists(e => e.EventId == eve.EventId))
			{
				AcceptOwnerButton.IsVisible = true;
				DeclineOwnerButton.IsVisible = true;
				AcceptOwnerButton.Clicked += (sender, e) =>
				{
					_dataManager.EventApiManager.AcceptDeclineLeaveEventAsOwner(eve.EventId, OwnerHandlingType.Accept);
				};
				DeclineOwnerButton.Clicked += (sender, e) =>
				{
					_dataManager.EventApiManager.AcceptDeclineLeaveEventAsOwner(eve.EventId, OwnerHandlingType.Decline);
				};
			}


			BannerImage.Clicked += (sender, e) =>
			{
				OtherFunctions of = new OtherFunctions();
				of.ViewImages(new List<string>() { eve.ImageSource });
			};
			/*
			if (eve.Followers.Exists(p => p.ProfileId == App.StoredUserFacebookId))
			{
				trackImg.Foreground = App.HowlOut;
			}

			TapGestureRecognizer tgr = new TapGestureRecognizer();
			tgr.Tapped += (sender, e) =>
			{
				App.coreView.DisplayShare(eve);
			};
			shareImg.GestureRecognizers.Add(tgr);

			tgr = new TapGestureRecognizer();
			tgr.Tapped += async (sender, e) =>
			{
				bool success = false;
				if (eve.Followers.Exists(p => p.ProfileId == App.StoredUserFacebookId))
				{
					success = await _dataManager.AttendTrackEvent(eve, false, false);
					if (success)
					{
						trackImg.Foreground = App.HowlOutFade;
					}
				}
				else {
					success = await _dataManager.AttendTrackEvent(eve, true, false);
					if (success)
					{
						trackImg.Foreground = App.HowlOut;
					}
				}
			};
			trackImg.GestureRecognizers.Add(tgr);

			tgr = new TapGestureRecognizer();
			tgr.Tapped += (sender, e) =>
			{
				App.coreView.setContentViewWithQueue(new CreateEvent(eve, false));
			};
			editImg.GestureRecognizers.Add(tgr);

			tgr = new TapGestureRecognizer();
			tgr.Tapped += (sender, e) =>
			{
				App.coreView.setContentViewWithQueue(new InviteListView(eve));
			};
			inviteImg.GestureRecognizers.Add(tgr);
			*/
			if (mapInitialized != true)
			{
				mapInitialized = true;
				_dataManager.UtilityManager.setMapForEvent(new Position(eve.Latitude, eve.Longitude), map, mapLayout);
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
		}

		public async void setInfo (Event eve)
		{
			quickInfo.IsVisible = true;
			//detailedInfo.IsVisible = false;

			//topTime.Text = eve.StartDate.ToString("ddd dd MMM");
			//bottomTime.Text = eve.StartDate.ToString("HH:mm") + "-" + eve.EndDate.ToString("HH:mm");

			//topDist.Text = efl.Distance + " km";
			eventDescription.Text = eve.Description;
			//StartTime.Text = "" + eve.StartDate.DayOfWeek + " the " + eve.StartDate.Day + " " + eve.StartDate.ToString("MMMM").ToLower();
			//EndTime.Text =  _dataManager.UtilityManager.getTime(eve.StartDate) + " - " + _dataManager.UtilityManager.getTime(eve.EndDate);

			string [] addressList = new string [3];
			addressList = Regex.Split(eve.AddressName, ",");
			for (int i = 0; i < addressList.Length; i++) { 
				Label label = new Label () {TextColor = Color.FromHex("646464")}; 
				label.Text = addressList [i].Trim();
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
	}
}