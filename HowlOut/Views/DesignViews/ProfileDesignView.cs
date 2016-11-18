using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;


namespace HowlOut
{
	public class ProfileDesignView : GenericDesignView
	{
		Profile profile;
		Plugin.Media.Abstractions.MediaFile mediaFile;
		Design design = Design.ShowAll;

		public ProfileDesignView(Profile profile, Event eveInvitingTo, int dims) : base(dims)
		{
			this.profile = profile;
			design = Design.NameAndButtons;
			SetupButtons(true);
			if (!eveInvitingTo.Attendees.Exists(p => p.ProfileId == profile.ProfileId))
			{
				HandleButtonRequests(delegate ()
				{
					return _dataManager.EventApiManager.InviteProfilesToEvent(eveInvitingTo.EventId, new List<Profile> { profile });
				}, addBtn, "Invite", "Invited");
			}
		}

		public ProfileDesignView(Profile profile, Group grpInvitingTo, int dims) : base(dims)
		{
			this.profile = profile;
			design = Design.NameAndButtons;
			SetupButtons(true);
			if (grpInvitingTo.ProfilesRequestingToJoin.Exists(p => p.ProfileId == profile.ProfileId))
			{
				HandleButtonRequests(delegate ()
				{
					return _dataManager.GroupApiManager.InviteDeclineToGroup(grpInvitingTo.GroupId, true, new List<Profile> { profile });
				}, addBtn, "Accept", "Acceptd");
				HandleButtonRequests(delegate ()
				{
					return _dataManager.GroupApiManager.InviteDeclineToGroup(grpInvitingTo.GroupId, false, new List<Profile> { profile });
				}, removeBtn, "Decline", "Declined");
			} else  if (!grpInvitingTo.Members.Exists(p => p.ProfileId == profile.ProfileId))
			{
				HandleButtonRequests(delegate ()
				{
					return _dataManager.GroupApiManager.InviteDeclineToGroup(grpInvitingTo.GroupId, true, new List<Profile> { profile });
				}, addBtn, "Invite", "Invited");
			}
		}

		public ProfileDesignView(Profile profile, Organization orgInvitingTo, int dims) : base(dims)
		{
			this.profile = profile;
			design = Design.NameAndButtons;
			SetupButtons(true);
			if (!orgInvitingTo.Members.Exists(p => p.ProfileId == profile.ProfileId))
			{
				HandleButtonRequests(delegate ()
				{
					return _dataManager.OrganizationApiManager.InviteToOrganization(orgInvitingTo.OrganizationId, profile);
				}, addBtn, "Invite", "Invited");
			} 
		}

		public ProfileDesignView(Profile profile, int dims, bool clickable, Design design) : base(dims)
		{
			this.profile = profile;
			this.design = design;
			SetupButtons(clickable);
			if (profile.ProfileId == App.StoredUserFacebookId) {
				editBtn.IsVisible = true;
				editBtn.Text = "Edit";
			}
			else {
				if (App.userProfile.RecievedFriendRequests != null && App.userProfile.RecievedFriendRequests.Exists(p => p.ProfileId == profile.ProfileId)) {
					HandleButtonRequests(delegate ()
					{
						return _dataManager.ProfileApiManager.RequestDeclineAcceptUnfriend(profile.ProfileId, true);
					}, addBtn, "Accept", "Accepted");
					HandleButtonRequests(delegate ()
					{
						return _dataManager.ProfileApiManager.RequestDeclineAcceptUnfriend(profile.ProfileId, false);
					}, removeBtn, "Decline", "Declined");
				}
				else if (App.userProfile.SentFriendRequests != null && App.userProfile.SentFriendRequests.Exists(p => p.ProfileId == profile.ProfileId)) {
					HandleButtonRequests(delegate ()
					{
						return _dataManager.ProfileApiManager.RequestDeclineAcceptUnfriend(profile.ProfileId, false);
					}, addBtn, "Cancel Request", "Canceled");
				}
				else if (App.userProfile.Friends.Exists(p => p.ProfileId == profile.ProfileId)) {
					HandleButtonRequests(delegate ()
					{
						return _dataManager.ProfileApiManager.RequestDeclineAcceptUnfriend(profile.ProfileId, false);
					}, removeBtn, "Remove", "Removed");
				} 
				else {
					HandleButtonRequests(delegate () 
					{ 
						return _dataManager.ProfileApiManager.RequestDeclineAcceptUnfriend(profile.ProfileId, true);
					}, addBtn, "Add", "RequestSent");
				}
			}
		}

		public async void SetupButtons(bool clickable)
		{
			


			if (clickable)
				subjBtn.Clicked += (sender, e) => {
				_dataManager.setUpdateSeen(profile.ProfileId, NotificationModelType.Profile);
				App.coreView.setContentViewWithQueue(new InspectController(profile), "", null); 
			};

			Profile updateProfile = App.userProfile;
			bool edit = false;
			editBtn.Clicked += (sender, e) =>
			{
				ShowHideEditLayout(!edit);

				if (edit)
				{
					SetInfo(profile.ImageSource, profile.Name, profile.Description, design, ModelType.Profile);
				}


				edit = !edit;


				/*
				if (editLayout.IsVisible)
				{
					await App.scrollTo(0);
					await Task.Delay(500);
					editLayout.IsVisible = false;
				}
				else {
					editLayout.IsVisible = true;
					await App.scrollTo(editBtn);

				}
				*/
			};




			Image newImage = new Image();

			var pictureImage = new TapGestureRecognizer();
			pictureImage.Tapped += async (sender, e) =>
			{
				mediaFile = await _dataManager.UtilityManager.TakePicture();
				if (mediaFile != null)
				{
					profileImage.Source = ImageSource.FromStream(mediaFile.GetStream);
					//SelectedBannerImage.Source = ImageSource.FromStream(mediaFile.GetStream);
				}
			};
			pictureButton.GestureRecognizers.Add(pictureImage);

			var albumImage = new TapGestureRecognizer();
			albumImage.Tapped += async (SenderOfEvent, e) =>
			{
				
				mediaFile = await _dataManager.UtilityManager.PictureFromAlbum();
				if (mediaFile != null)
				{
					profileImage.Source = ImageSource.FromStream(mediaFile.GetStream);
				}
			};
			albumButton.GestureRecognizers.Add(albumImage);

			fbImageButton.Clicked += (sender, e) =>
			{
				updateProfile.ImageSource = "https://graph.facebook.com/v2.5/" + App.userProfile.ProfileId + "/picture?height=200&width=200";
				profileImage.Source = updateProfile.ImageSource;
				mediaFile = null;
			};

			updateProfileBtn.Clicked += async  (sender, e) =>
			{
				App.coreView.IsLoading(true);
				if (mediaFile != null)
				{
					updateProfile.ImageSource = await _dataManager.UtilityManager.UploadImageToStorage(mediaFile.GetStream(), App.StoredUserFacebookId + "." + DateTime.Now.ToString("G"));
				}
				updateProfile.Description = descriptionEdit.Text;
				bool success = await _dataManager.ProfileApiManager.CreateUpdateProfile(updateProfile, false);
				if (success)
				{
					//App.coreView.homeView = new HomeView();
					App.coreView.setContentView(4);
				}
				App.coreView.IsLoading(false);
				App.coreView.updateHomeView();
				ShowHideEditLayout(false);
				edit = false;
			};

			profileLogOutBtn.Clicked += async (sender, e) =>
			{
				await App.storeToken("","","");
				await Navigation.PushModalAsync(new LoginPage());
			};

			profile = await _dataManager.ProfileApiManager.GetProfile(profile.ProfileId);
			SetInfo(profile.ImageSource, profile.Name, profile.Description, design, ModelType.Profile);
		}
	}
}
