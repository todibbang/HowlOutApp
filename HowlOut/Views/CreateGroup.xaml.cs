using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class CreateGroup : ContentView
	{
		public Group newGroup;
		DataManager _dataManager;

		public CreateGroup ()
		{
			_dataManager = new DataManager ();
			InitializeComponent ();

			newGroup = new Group ();

			title.TextChanged += (sender, e) => { newGroup.Name = title.Text; };

			launchButton.Clicked += (sender, e) => {
				LaunchGroup(newGroup);
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
				App.coreView.setContentView (new InspectController (null, groupCreated, null), "UserProfile");
			} else {
				await App.coreView.displayAlertMessage ("Error", "Event not updated, try again", "Ok");
			}
		}
	}
}

