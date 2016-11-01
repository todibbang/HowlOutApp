using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class CreateGroup : ContentView
	{
		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}

		public Group newGroup;
		Plugin.Media.Abstractions.MediaFile mediaFile;
		DataManager _dataManager;
		private bool Launching = false;

		public CreateGroup (Group group, bool isCreate)
		{
			_dataManager = new DataManager ();
			InitializeComponent ();
			newGroup = group;
			if (isCreate) {
				cancelButton.IsVisible = false;
				newGroup.GroupId = "0";
			}
			else {
				setEditEvent();
			}

			title.TextChanged += (sender, e) => { newGroup.Name = title.Text; };
			description.TextChanged += (sender, e) => { newGroup.Description = description.Text; };

			// Here's the visibility of the group selected
			visibilityPicker.SelectedIndexChanged += (sender, e) =>
			{
				if (visibilityPicker.SelectedIndex == 0) { newGroup.Visibility = Visibility.Open; }
				if (visibilityPicker.SelectedIndex == 1) { newGroup.Visibility = Visibility.Closed; }
				if (visibilityPicker.SelectedIndex == 2) { newGroup.Visibility = Visibility.Secret; }
				visibilityString.Text = visibilityPicker.Items[visibilityPicker.SelectedIndex];
			};
			visibilityString.Text = visibilityPicker.Title;

			var pictureImage = new TapGestureRecognizer();
			pictureImage.Tapped += async (sender, e) =>
			{
				try
				{
					mediaFile = await _dataManager.UtilityManager.TakePicture();
					if (mediaFile != null)
					{
						SelectedBannerImage.Source = ImageSource.FromStream(mediaFile.GetStream);
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
					mediaFile = await _dataManager.UtilityManager.PictureFromAlbum();
					if (mediaFile != null)
					{
						SelectedBannerImage.Source = ImageSource.FromStream(mediaFile.GetStream);
					}
				}
				catch (Exception ex) { }
			};
			albumPictureButton.GestureRecognizers.Add(albumImage);

			selectBannerButton.Clicked += (sender, e) => {
				SelectBannerView selectBannerView = new SelectBannerView();
				selectBannerView.createGroupView = this;
				App.coreView.setContentViewWithQueue(selectBannerView, "", null);
			};



			launchButton.Clicked += async (sender, e) => {
				if (isCreate && !Launching)
				{
					bool continueCreating = await App.coreView.otherFunctions.SenderOfEvent(SelectSenderLayout, null, newGroup);
					if (continueCreating)
					{
						LaunchGroup(newGroup);
						Launching = true;
					}
				}
				else if (!isCreate && !Launching)
				{
					LaunchGroup(newGroup);
				}
			};

			cancelButton.Clicked += (sender, e) => {
				DeleteGroup(group);
			};
		}

		private void setEditEvent()
		{
			cancelButton.IsVisible = true;
			launchButton.Text = "Update";

			title.Text = newGroup.Name;
			description.Text = newGroup.Description;
			visibilityPicker.SelectedIndex = Array.IndexOf(Enum.GetValues(newGroup.Visibility.GetType()), newGroup.Visibility);
			SelectedBannerImage.Source = newGroup.ImageSource;
		}

		private async void LaunchGroup(Group groupToCreate)
		{
			App.coreView.IsLoading(true);
			if (mediaFile != null)
			{
				groupToCreate.ImageSource = await _dataManager.UtilityManager.UploadImageToStorage(mediaFile.GetStream(), App.StoredUserFacebookId + "." + DateTime.Now.ToString("G"));
			}
			if (groupToCreate.OrganizationOwner == null)
			{
				groupToCreate.ProfileOwner = App.userProfile;
			}

			if (String.IsNullOrWhiteSpace(groupToCreate.Name))
			{
				await App.coreView.displayAlertMessage("Name Missing", "Name is missing", "Ok");
			}
			else if (String.IsNullOrWhiteSpace(groupToCreate.Description))
			{
				await App.coreView.displayAlertMessage("Description Missing", "Description is missing", "Ok");
			}
			else if (String.IsNullOrWhiteSpace(groupToCreate.ImageSource))
			{
				await App.coreView.displayAlertMessage("Banner Missing", "No banner has been selected", "Ok");
			}
			else {
				
				var groupCreated = await _dataManager.GroupApiManager.CreateEditGroup(groupToCreate);

				if (groupCreated != null)
				{
					InspectController inspect = new InspectController(groupCreated);
					App.coreView.setContentViewWithQueue(inspect, "UserProfile", inspect.getScrollView());
					//App.coreView.createGroup = new CreateGroup(new Group(), true);
					//App.coreView.homeView = new HomeView();
				}
				else {
					await App.coreView.displayAlertMessage("Error", "Event not created, try again", "Ok");
				}
			}
			Launching = false; 
			App.coreView.IsLoading(false);
		}

		/*
		private async void UpdateGroup(Group groupToUpdate)
		{
			if (mediaFile != null)
			{
				groupToUpdate.ImageSource = await _dataManager.UtilityManager.UploadImageToStorage(mediaFile.GetStream(), App.StoredUserFacebookId + "." + DateTime.Now.ToString("G"));
			}

			var groupUpdated = await _dataManager.GroupApiManager.CreateEditGroup(groupToUpdate);

			if (groupUpdated != null) {
				InspectController inspect = new InspectController(groupUpdated);
				App.coreView.setContentViewWithQueue(inspect, "Group", inspect.getScrollView());
			} else {
				await App.coreView.displayAlertMessage ("Error", "Group not updated, try again", "Ok");
			}
		}
		*/

		public async void DeleteGroup(Group groupToDelete)
		{
			bool confirmDelete = await App.coreView.displayConfirmMessage ("Warning", "You are about to delete this group permanently, would you like to continue", "Yes", "No");

			if (confirmDelete) {
				bool groupDeleted = await _dataManager.GroupApiManager.DeleteGroup (groupToDelete.GroupId);
				if (groupDeleted) {
					await App.coreView.displayAlertMessage ("Group Deleted", "The group was successfully deleted", "Ok");
					App.coreView.setContentView (2);
				} else {
					App.coreView.displayAlertMessage ("Group Not Deleted", "The group was not deleted, try again", "Ok");
				}
			}
		}

		public void setBanner(string banner)
		{
			selectBannerButton.BackgroundColor = Color.Transparent;
			SelectedBannerImage.Source = banner;
			newGroup.ImageSource = banner;
			mediaFile = null;
		}
	}
}

