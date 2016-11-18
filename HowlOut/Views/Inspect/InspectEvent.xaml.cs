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
			if (_dataManager.IsEventJoined(eve) || 
			    (eve.OrganizationOwner != null && App.userProfile.Organizations.Exists(o => o.OrganizationId == eve.OrganizationOwner.OrganizationId))) 
			{ searchSpecific.IsVisible = false; manageSpecific.IsVisible = true; } 
			else  { searchSpecific.IsVisible = true; manageSpecific.IsVisible = false; }

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
			};

			scrollView.Scrolled += (sender, e) =>
			{
				if (scrollView.ScrollY < 100 && scrollView.ScrollY > 0)
				{
					titleInfo.TranslationY = scrollView.ScrollY;
				}
			};
				
			mapButton.Clicked += (sender, e) => {
				App.coreView.setContentViewWithQueue(new MapsView(eve), "MapsView", null);
			};

			if (_dataManager.IsEventYours(eve) ||
				(eve.OrganizationOwner != null && App.userProfile.Organizations.Exists(o => o.OrganizationId == eve.OrganizationOwner.OrganizationId)))
			{
				editLeaveButton.Text = "Edit";
			}
			else if (eve.Followers.Exists(p => p.ProfileId == App.StoredUserFacebookId)) {
				followButton.Text = "Unfollow"; 
			}
			if (eve.OrganizationOwner != null && App.userProfile.Organizations.Exists(o => o.OrganizationId == eve.OrganizationOwner.OrganizationId)) {
				joinLeaveWhenOrganizationOwnerButton.IsVisible = true;
				if (eve.Attendees.Exists(p => p.ProfileId == App.StoredUserFacebookId))
				{
					joinLeaveWhenOrganizationOwnerButton.Text = "Leave";
					joinLeaveWhenOrganizationOwnerButton.Clicked += (sender, e) =>
					{
						_dataManager.AttendTrackEvent(eve, false, true);
					};
				}
				else {
					joinLeaveWhenOrganizationOwnerButton.Clicked += (sender, e) =>
					{
						_dataManager.AttendTrackEvent(eve, true, true);
					};
				}
			}

			editLeaveButton.Clicked += (sender, e) => {
				if (_dataManager.IsEventYours(eve) ||
				(eve.OrganizationOwner != null && App.userProfile.Organizations.Exists(o => o.OrganizationId == eve.OrganizationOwner.OrganizationId))) {
					App.coreView.setContentViewWithQueue (new CreateEvent (eve, false), "CreateEvent", null);
				} else {
					_dataManager.AttendTrackEvent(eve, false, true);
				}
			};

			joinButton.Clicked += (sender, e) => {
				_dataManager.AttendTrackEvent(eve,true,true);
			};

			followButton.Clicked += (sender, e) =>  {
				if (eve.Followers.Exists(p => p.ProfileId == App.StoredUserFacebookId))
				{
					_dataManager.AttendTrackEvent(eve, false, false);
				}
				else {
					_dataManager.AttendTrackEvent(eve, true, false);
				}
			};

			inviteButton.Clicked += (sender, e) => {
				App.coreView.setContentViewWithQueue (new InviteListView (eve), "InviteView", null);
			};

			groupSpecificButton.Clicked += (sender, e) =>
			{
				App.coreView.setContentViewWithQueue(new InspectController(eve.GroupSpecific), "Inspect", null);
			};
		}

		public async void setInfo (Event eve)
		{
			quickInfo.IsVisible = true;
			detailedInfo.IsVisible = false;

			topTime.Text = eve.StartDate.ToString("ddd dd MMM");
			bottomTime.Text = eve.StartDate.ToString("HH:mm") + "-" + eve.EndDate.ToString("HH:mm");

			topDist.Text = efl.Distance + " km";
			eventDescription.Text = eve.Description;
			StartTime.Text = "" + eve.StartDate.DayOfWeek + " the " + eve.StartDate.Day + " " + eve.StartDate.ToString("MMMM").ToLower();
			EndTime.Text =  _dataManager.UtilityManager.getTime(eve.StartDate) + " - " + _dataManager.UtilityManager.getTime(eve.EndDate);

			string [] addressList = new string [3];
			addressList = Regex.Split(eve.AddressName, ",");
			for (int i = 0; i < addressList.Length; i++) { 
				Label label = new Label () {TextColor = Color.FromHex("646464")}; 
				label.Text = addressList [i];
				label.FontSize = 14;
				addressLayout.Children.Add(label);
			}
			if (addressList.Length == 2)
			{
				bottomDist.Text = addressList[0].Substring(5).Trim();
			}
			else {
				bottomDist.Text = addressList[1].Substring(5).Trim();
			}
		}
	}
}