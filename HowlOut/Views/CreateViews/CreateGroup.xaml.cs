using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.IO;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class CreateGroup : ContentView, ViewModelInterface
	{
		public Task<UpperBar> getUpperBar() { return null; }
		public ContentView getContentView() { return this; }
		public void reloadView() { }

		public ContentView otherViews { get { return OtherViews; } set { } }

		public Group newGroup;
		List<byte[]> imageStreams;
		DataManager _dataManager;
		private bool Launching = false;
		public bool IsCreate = false;

		public CreateGroup(Group group, bool isCreate)
		{
			_dataManager = new DataManager();
			IsCreate = isCreate;
			InitializeComponent();
			newGroup = group;
			if (isCreate)
			{
				cancelButton.IsVisible = false;
				newGroup.GroupId = "0";
			}
			else {
				setEditEvent();
				mainGrid.Padding = new Thickness(0, 55, 0, 250);
			}

			title.TextChanged += (sender, e) => { newGroup.Name = title.Text; };
			description.TextChanged += (sender, e) => { newGroup.Description = description.Text; };

			// Here's the visibility of the group selected
			visibilityPicker.SelectedIndexChanged += (sender, e) =>
			{
				if (visibilityPicker.SelectedIndex == 0) { newGroup.Visibility = GroupVisibility.Public; }
				if (visibilityPicker.SelectedIndex == 1) { newGroup.Visibility = GroupVisibility.Closed; }
				if (visibilityPicker.SelectedIndex == 2) { newGroup.Visibility = GroupVisibility.Private; }
				visibilityString.Text = visibilityPicker.Items[visibilityPicker.SelectedIndex];
			};
			visibilityString.Text = visibilityPicker.Title;

			var pictureImage = new TapGestureRecognizer();
			pictureImage.Tapped += async (sender, e) =>
			{
				try
				{
					imageStreams = await _dataManager.UtilityManager.TakePicture(SelectedBannerImage);
				}
				catch (Exception ex) { }
			};
			takePictureButton.GestureRecognizers.Add(pictureImage);

			var albumImage = new TapGestureRecognizer();
			albumImage.Tapped += async (SenderOfEvent, e) =>
			{
				try
				{
					imageStreams = await _dataManager.UtilityManager.PictureFromAlbum(SelectedBannerImage);
				}
				catch (Exception ex) { }
			};
			albumPictureButton.GestureRecognizers.Add(albumImage);

			selectBannerButton.Clicked += async (sender, e) =>
			{
				SelectBannerView selectBannerView = new SelectBannerView();
				selectBannerView.createGroupView = this;
				OtherViews.Content = selectBannerView;
				OtherViews.IsVisible = true;
				App.coreView.IsLoading(true);
				await Task.Delay(100);
				if (isCreate) 
				{ 
					App.coreView.createView.displayBackButton(true, 2);
					App.coreView.setContentView(0);
				}
				await Task.Delay(500);
				App.coreView.IsLoading(false);
			};



			launchButton.Clicked += (sender, e) =>
			{
				if (isCreate && !Launching)
				{
					LaunchGroup(newGroup);
					Launching = true;
				}
				else if (!isCreate && !Launching)
				{
					LaunchGroup(newGroup);
				}
			};

			cancelButton.Clicked += (sender, e) =>
			{
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

			if (String.IsNullOrWhiteSpace(groupToCreate.Name))
			{
				await App.rootPage.displayAlertMessage("Name Missing", "Name is missing", "Ok");
			}
			else if (String.IsNullOrWhiteSpace(groupToCreate.Description))
			{
				await App.rootPage.displayAlertMessage("Description Missing", "Description is missing", "Ok");
			}
			else if (String.IsNullOrWhiteSpace(groupToCreate.ImageSource) && imageStreams == null)
			{
				await App.rootPage.displayAlertMessage("Banner Missing", "No banner has been selected", "Ok");
			}
			else {
				if (imageStreams != null)
				{
					groupToCreate.SmallImageSource = await _dataManager.UtilityManager.UploadImageToStorage(new MemoryStream(imageStreams[0]), App.StoredUserFacebookId + "." + DateTime.Now.ToString("G") + ".small");
					groupToCreate.ImageSource = await _dataManager.UtilityManager.UploadImageToStorage(new MemoryStream(imageStreams[1]), App.StoredUserFacebookId + "." + DateTime.Now.ToString("G") + ".medium");
					groupToCreate.LargeImageSource = await _dataManager.UtilityManager.UploadImageToStorage(new MemoryStream(imageStreams[2]), App.StoredUserFacebookId + "." + DateTime.Now.ToString("G") + ".large");
				}
				groupToCreate.ProfileOwners = new List<Profile> { new Profile() { ProfileId = App.userProfile.ProfileId, } };


				var groupCreated = await _dataManager.GroupApiManager.CreateEditGroup(groupToCreate);

				if (groupCreated != null)
				{
					await App.coreView.updateMainViews(4);
					App.coreView.setContentView(4);

					InspectController inspect = new InspectController(groupCreated);
					App.coreView.setContentViewWithQueue(inspect);

					App.coreView.updateMainViews(0);
				}
				else {
					await App.rootPage.displayAlertMessage("Error", "Event not created, try again", "Ok");
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
				await App.rootPage.displayAlertMessage ("Error", "Group not updated, try again", "Ok");
			}
		}
		*/

		public async void DeleteGroup(Group groupToDelete)
		{
			bool confirmDelete = await App.rootPage.displayConfirmMessage("Warning", "You are about to delete this group permanently, would you like to continue", "Yes", "No");
			App.coreView.IsLoading(true);
			if (confirmDelete)
			{
				bool groupDeleted = await _dataManager.GroupApiManager.DeleteGroup(groupToDelete.GroupId);
				if (groupDeleted)
				{
					await App.rootPage.displayAlertMessage("Group Deleted", "The group was successfully deleted", "Ok");
					await App.coreView.updateMainViews(4);
					App.coreView.setContentView(4);
				}
				else {
					App.rootPage.displayAlertMessage("Group Not Deleted", "The group was not deleted, try again", "Ok");
				}
			}
			App.coreView.IsLoading(false);
		}

		public void setBanner(string banner)
		{
			selectBannerButton.BackgroundColor = Color.Transparent;
			SelectedBannerImage.Source = banner;

			newGroup.ImageSource = banner;
			newGroup.LargeImageSource = banner;
			imageStreams = null;
		}
	}
}

