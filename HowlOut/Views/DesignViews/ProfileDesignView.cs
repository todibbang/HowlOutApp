using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.IO;

namespace HowlOut
{
	public class ProfileDesignView : GenericDesignView
	{
		Profile profile;
		Design design = Design.ShowAll;
		List<byte[]> imageStreams;

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
			setPillButtonLayout(new List<Button>() {addBtn, removeBtn });
		}

		/*
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
		} */

		public ProfileDesignView(Profile profile, int dims, bool clickable, Design design) : base(dims)
		{
			this.profile = profile;
			this.design = design;
			SetupButtons(clickable);
			if (profile.ProfileId != App.StoredUserFacebookId)
			{
				if (App.userProfile.RecievedFriendRequests != null && App.userProfile.RecievedFriendRequests.Exists(p => p.ProfileId == profile.ProfileId))
				{
					addBtn.Clicked += async (sender, e) =>
					{
						await _dataManager.ProfileApiManager.RequestDeclineAcceptUnfriend(profile.ProfileId, true);
						if (clickable)
						{
							await App.coreView.updateHomeView();
							App.coreView.setContentView(4);
						}
						else {
							App.coreView.setContentViewWithQueue(new InspectController(profile));
						}
					};


					HandleButtonRequests(delegate ()
					{
						return _dataManager.ProfileApiManager.RequestDeclineAcceptUnfriend(profile.ProfileId, true);
					}, addBtn, "Accept", "Accepted");
					HandleButtonRequests(delegate ()
					{
						return _dataManager.ProfileApiManager.RequestDeclineAcceptUnfriend(profile.ProfileId, false);
					}, removeBtn, "Decline", "Declined");
				}
				else if (App.userProfile.SentFriendRequests != null && App.userProfile.SentFriendRequests.Exists(p => p.ProfileId == profile.ProfileId))
				{
					HandleButtonRequests(delegate ()
					{
						return _dataManager.ProfileApiManager.RequestDeclineAcceptUnfriend(profile.ProfileId, false);
					}, addBtn, "Cancel Request", "Canceled");
				}
				else if (App.userProfile.Friends.Exists(p => p.ProfileId == profile.ProfileId))
				{
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
			} else {
				if (profile.ProfileId == App.userProfile.ProfileId && dims >= 200)
				{
					editBtn.IsVisible = true;
					/*
					bool edit = false;
					App.coreView.topBar.setRightButton("ic_menu.png").Clicked += async (sender, e) =>
					{
						List<Action> actions = new List<Action>();
						List<string> titles = new List<string>();
						List<string> images = new List<string>();

						actions.Add(() =>
						{
							ShowHideEditLayout(!edit);
							if (edit)
							{
								SetInfo(profile.ImageSource, profile.Name, profile.Description, design, ModelType.Profile);
							}
							edit = !edit;
						});
						titles.Add("Edit");
						images.Add("ic_settings.png");

						await App.coreView.DisplayOptions(actions, titles, images);
					}; */
				}
			}
		}

		public async void SetupButtons(bool clickable)
		{

			profile = await _dataManager.ProfileApiManager.GetProfile(profile.ProfileId);

			subjBtn.Clicked += (sender, e) =>
			{
				if (clickable)
				{
					_dataManager.setUpdateSeen(profile.ProfileId, NotificationModelType.Profile);
					App.coreView.setContentViewWithQueue(new InspectController(profile));
				}
				else {
					OtherFunctions of = new OtherFunctions();
					of.ViewImages(new List<string>() { profile.ImageSource });
				}
			};

			Profile updateProfile = App.userProfile;
			bool edit = false;

			editBtn.Text = "Edit";
			editBtn.Clicked += (sender, e) => { 
				ShowHideEditLayout(!edit);
				if (edit)
				{
					SetInfo(profile.ImageSource, profile.Name, profile.Description, design, ModelType.Profile);
				}
				edit = !edit;
			};

			var pictureImage = new TapGestureRecognizer();
			pictureImage.Tapped += async (sender, e) =>
			{
				imageStreams = await _dataManager.UtilityManager.TakePicture(profileImage.Source);
			};
			pictureButton.GestureRecognizers.Add(pictureImage);

			var albumImage = new TapGestureRecognizer();
			albumImage.Tapped += async (SenderOfEvent, e) =>
			{
				imageStreams = await _dataManager.UtilityManager.PictureFromAlbum(profileImage.Source);
			};
			albumButton.GestureRecognizers.Add(albumImage);

			fbImageButton.Clicked += (sender, e) =>
			{
				updateProfile.SmallImageSource = "https://graph.facebook.com/v2.5/" + App.userProfile.ProfileId + "/picture?height=50&width=50";
				updateProfile.ImageSource = "https://graph.facebook.com/v2.5/" + App.userProfile.ProfileId + "/picture?height=100&width=100";
				updateProfile.LargeImageSource = "https://graph.facebook.com/v2.5/" + App.userProfile.ProfileId + "/picture?height=300&width=300";
				profileImage.Source = updateProfile.ImageSource;
				imageStreams = null;
			};

			updateProfileBtn.Clicked += async  (sender, e) =>
			{
				App.coreView.IsLoading(true);
				if (imageStreams != null)
				{
					updateProfile.SmallImageSource = await _dataManager.UtilityManager.UploadImageToStorage(new MemoryStream(imageStreams[0]), App.StoredUserFacebookId + "." + DateTime.Now.ToString("G") + ".small");
					updateProfile.ImageSource = await _dataManager.UtilityManager.UploadImageToStorage(new MemoryStream(imageStreams[1]), App.StoredUserFacebookId + "." + DateTime.Now.ToString("G")+".medium");
					updateProfile.LargeImageSource = await _dataManager.UtilityManager.UploadImageToStorage(new MemoryStream(imageStreams[2]), App.StoredUserFacebookId + "." + DateTime.Now.ToString("G") + ".large");
				}
				updateProfile.Description = descriptionEdit.Text;
				bool success = await _dataManager.ProfileApiManager.CreateUpdateProfile(updateProfile, false);
				if (success)
				{
					//App.coreView.homeView = new HomeView();
					await App.coreView.updateHomeView();
					App.coreView.setContentView(4);
				}
				App.coreView.IsLoading(false);

				ShowHideEditLayout(false);
				edit = false;
			};

			profileLogOutBtn.Clicked += async (sender, e) =>
			{
				await App.storeToken("","","");
				await Navigation.PushModalAsync(new LoginPage());
			};

			try
			{
				//profile = await _dataManager.ProfileApiManager.GetProfile(profile.ProfileId);
				string d = profile.Description != null ? profile.Description : "";
				SetInfo(profile.ImageSource, profile.Name, d, design, ModelType.Profile);
			}
			catch (Exception e) {}
		}
	}
}
