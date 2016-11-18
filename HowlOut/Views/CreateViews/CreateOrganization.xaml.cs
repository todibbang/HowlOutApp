using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class CreateOrganization : ContentView
	{
		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}

		public Organization newOrg;
		Plugin.Media.Abstractions.MediaFile mediaFile;
		DataManager _dataManager;

		public CreateOrganization(Organization org, bool isCreate)
		{
			_dataManager = new DataManager();
			InitializeComponent();
			newOrg = org;
			if (isCreate)
			{
				cancelButton.IsVisible = false;
				newOrg.OrganizationId = "0";
			}
			else {
				setEditEvent();
			}

			title.TextChanged += (sender, e) => { newOrg.Name = title.Text; };
			description.TextChanged += (sender, e) => { newOrg.Description = description.Text; };

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

			selectBannerButton.Clicked += (sender, e) =>
			{
				SelectBannerView selectBannerView = new SelectBannerView();
				selectBannerView.createOrganizationView = this;
				App.coreView.setContentViewWithQueue(selectBannerView, "", null);
			};



			launchButton.Clicked += (sender, e) =>
			{

					LaunchOrganization(newOrg);

			};

			cancelButton.Clicked += (sender, e) =>
			{
				DeleteOrganization(newOrg);
			};
		}

		private void setEditEvent()
		{
			cancelButton.IsVisible = true;
			launchButton.Text = "Update";

			title.Text = newOrg.Name;
			description.Text = newOrg.Description;
			SelectedBannerImage.Source = newOrg.ImageSource;
		}

		private async void LaunchOrganization(Organization orgToCreate)
		{
			App.coreView.IsLoading(true);
			if (mediaFile != null)
			{
				orgToCreate.ImageSource = await _dataManager.UtilityManager.UploadImageToStorage(mediaFile.GetStream(), App.StoredUserFacebookId + "." + DateTime.Now.ToString("G"));
			}

			if (String.IsNullOrWhiteSpace(orgToCreate.Name))
			{
				await App.coreView.displayAlertMessage("Name Missing", "Name is missing", "Ok");
			}
			else if (String.IsNullOrWhiteSpace(orgToCreate.Description))
			{
				await App.coreView.displayAlertMessage("Description Missing", "Description is missing", "Ok");
			}
			else if (String.IsNullOrWhiteSpace(orgToCreate.ImageSource))
			{
				await App.coreView.displayAlertMessage("Banner Missing", "No banner has been selected", "Ok");
			}
			else {
				var orgCreated = new Organization();
				orgCreated = await _dataManager.OrganizationApiManager.CreateEditOrganization(orgToCreate);

				if (orgCreated != null)
				{
					InspectController inspect = new InspectController(orgCreated);
					App.coreView.setContentViewWithQueue(inspect, "Organization", inspect.getScrollView());
					App.coreView.updateHomeView();
					App.coreView.createOrganization = new CreateOrganization(new Organization(), true);
					App.coreView.updateCreateViews();

					//App.coreView.homeView = new HomeView();
				}
				else {
					await App.coreView.displayAlertMessage("Error", "Organization not created, try again", "Ok");
				}
			}
			App.coreView.IsLoading(false);
		}
		/*
		private async void UpdateOrganization(Organization orgToUpdate)
		{
			if (mediaFile != null)
			{
				orgToUpdate.ImageSource = await _dataManager.UtilityManager.UploadImageToStorage(mediaFile.GetStream(), App.StoredUserFacebookId + "." + DateTime.Now.ToString("G"));
			}

			var orgUpdated = await _dataManager.OrganizationApiManager.CreateEditOrganization(orgToUpdate);

			if (orgUpdated != null)
			{
				InspectController inspect = new InspectController(orgUpdated);
				App.coreView.setContentViewWithQueue(inspect, "Organization", inspect.getScrollView());
			}
			else {
				await App.coreView.displayAlertMessage("Error", "Organization not updated, try again", "Ok");
			}
		}
		*/

		public async void DeleteOrganization(Organization orgToDelete)
		{
			bool confirmDelete = await App.coreView.displayConfirmMessage("Warning", "You are about to delete this group permanently, would you like to continue", "Yes", "No");

			if (confirmDelete)
			{
				bool orgDeleted = await _dataManager.OrganizationApiManager.DeleteOrganization(orgToDelete.OrganizationId);
				if (orgDeleted)
				{
					await App.coreView.displayAlertMessage("Organization Deleted", "The organization was successfully deleted", "Ok");
					App.coreView.setContentView(2);
				}
				else {
					App.coreView.displayAlertMessage("Organization Not Deleted", "The organization was not deleted, try again", "Ok");
				}
			}
		}

		public void setBanner(string banner)
		{
			SelectedBannerImage.Source = banner;
			newOrg.ImageSource = banner;
			mediaFile = null;
		}
	}
}

