using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class CreateGroup : ContentView
	{
		public Group newGroup;
		DataManager _dataManager;

		public CreateGroup (Group group)
		{
			_dataManager = new DataManager ();
			InitializeComponent ();

			if (group != null) {
				newGroup = group;
				launchButton.Text = "Edit";
				cancelButton.IsVisible = true;
			} else {
				newGroup = new Group ();
			}

			title.TextChanged += (sender, e) => { newGroup.Name = title.Text; };

			launchButton.Clicked += (sender, e) => {
				if (group != null) {
					UpdateGroup(newGroup);
				} else {
					LaunchGroup(newGroup);
				}
			};
			cancelButton.Clicked += (sender, e) => {
				DeleteGroup(group);
			};
		}

		private async void LaunchGroup(Group groupToCreate)
		{
			GroupDBO newGroupAsDBO = new GroupDBO{
				Owner = App.userProfile, 
				Name = groupToCreate.Name,
				Public = true
			};

			var groupCreated = await _dataManager.GroupApiManager.CreateGroup(newGroupAsDBO);

			if (groupCreated != null) {
				App.coreView.setContentViewWithQueue (new InspectController (null, groupCreated, null), "UserProfile");
			} else {
				await App.coreView.displayAlertMessage ("Error", "Event not created, try again", "Ok");
			}
		}

		private async void UpdateGroup(Group groupToUpdate)
		{
			bool groupUpdated = await _dataManager.GroupApiManager.UpdateGroup(groupToUpdate);

			if (groupUpdated) {
				App.coreView.setContentViewWithQueue (new InspectController (null, groupToUpdate, null), "Group");
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
	}
}

