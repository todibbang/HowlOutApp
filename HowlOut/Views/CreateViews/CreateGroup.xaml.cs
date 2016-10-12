using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class CreateGroup : ContentView
	{
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
			};

			// Group Image Settings
			takePictureButton.Clicked += async (sender, e) => {
				mediaFile = await _dataManager.UtilityManager.TakePicture();
				if (mediaFile != null) {
					SelectedBannerImage.Source = ImageSource.FromStream(mediaFile.GetStream);
					selectBannerButton.BackgroundColor = Color.Transparent;
				}
			};

			albumPictureButton.Clicked += async (SenderOfEvent, e) => {
				mediaFile = await _dataManager.UtilityManager.PictureFromAlbum();
				if (mediaFile != null) {
					SelectedBannerImage.Source = ImageSource.FromStream(mediaFile.GetStream);
					selectBannerButton.BackgroundColor = Color.Transparent;
				}
			};
			selectBannerButton.Clicked += (sender, e) => {
				SelectBannerView selectBannerView = new SelectBannerView();
				selectBannerView.createGroupView = this;
				App.coreView.setContentViewWithQueue(selectBannerView, "", null);
			};



			launchButton.Clicked += async (sender, e) => {
				if (isCreate && !Launching)
				{
					bool continueCreating = await App.SenderOfEvent(SelectSenderLayout);
					if (continueCreating)
					{
						LaunchGroup(newGroup);
						Launching = true;
					}
				}
				else if (!isCreate && !Launching)
				{
					UpdateGroup(newGroup);
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
			if (mediaFile != null)
			{
				groupToCreate.ImageSource = await _dataManager.UtilityManager.UploadImageToStorage(mediaFile.GetStream(), App.StoredUserFacebookId + "." + DateTime.Now.ToString("G"));
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
				GroupDBO newGroupAsDBO = new GroupDBO
				{
					Name = groupToCreate.Name,
					Description = groupToCreate.Description,
					ImageSource = groupToCreate.ImageSource,
					Visibility = groupToCreate.Visibility,
					Owner = App.userProfile,
					OrganizationOwner = groupToCreate.OrganizationOwner,
				};

				var groupCreated = await _dataManager.GroupApiManager.CreateGroup(newGroupAsDBO);

				if (groupCreated != null)
				{
					InspectController inspect = new InspectController(null, groupCreated, null);
					App.coreView.setContentViewWithQueue(inspect, "UserProfile", inspect.getScrollView());
				}
				else {
					await App.coreView.displayAlertMessage("Error", "Event not created, try again", "Ok");
				}
			}
			Launching = false;
		}

		private async void UpdateGroup(Group groupToUpdate)
		{
			bool groupUpdated = await _dataManager.GroupApiManager.UpdateGroup(groupToUpdate);

			if (groupUpdated) {
				InspectController inspect = new InspectController(null, groupToUpdate, null);
				App.coreView.setContentViewWithQueue(inspect, "Group", inspect.getScrollView());
			} else {
				await App.coreView.displayAlertMessage ("Error", "Group not updated, try again", "Ok");
			}
		}

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

