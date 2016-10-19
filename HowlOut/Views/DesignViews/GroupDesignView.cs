using System;
using System.Collections.Generic;
namespace HowlOut
{
	public class GroupDesignView : GenericDesignView
	{
		Group group;

		public GroupDesignView(Group group, int dims) : base(dims)
		{
			this.group = group;
			SetImage(group.ImageSource, group.Name);
			subjBtn.Clicked += (sender, e) => { App.coreView.setContentViewWithQueue(new InspectController(group), "", null); };
			setInfo();
		}

		async void setInfo()
		{
			group = await new DataManager().GroupApiManager.GetGroup(group.GroupId);

			if (group.ProfileOwner.ProfileId == App.StoredUserFacebookId)
			{
				addBtn.IsVisible = true;
				addBtn.Text = "Edit";
				addBtn.Clicked += (sender, e) =>
				{
					App.coreView.setContentViewWithQueue(new CreateGroup(group, false), "", null);
				};
			}
			else if (App.userProfile.GroupsInviteTo.Exists(g => g.GroupId == group.GroupId))
			{
				addBtn.IsVisible = true;
				addBtn.Text = "Accept";
				addBtn.Clicked += async (sender, e) =>
				{
					await _dataManager.GroupApiManager.InviteAcceptDeclineLeaveGroup(group.GroupId, new List<Profile> { App.userProfile }, GroupApiManager.GroupHandlingType.Accept);
				};
				removeBtn.IsVisible = true;
				removeBtn.Text = "Decline";
				removeBtn.Clicked += async (sender, e) =>
				{
					await _dataManager.GroupApiManager.InviteAcceptDeclineLeaveGroup(group.GroupId, new List<Profile> { App.userProfile }, GroupApiManager.GroupHandlingType.Decline);
				};
			}
			else if (App.userProfile.Groups.Exists(g => g.GroupId == group.GroupId))
			{
				removeBtn.IsVisible = true;
				removeBtn.Text = "Leave";
				removeBtn.Clicked += async (sender, e) =>
				{
					await _dataManager.GroupApiManager.InviteAcceptDeclineLeaveGroup(group.GroupId, new List<Profile> { App.userProfile }, GroupApiManager.GroupHandlingType.Leave);
				};
			}
			else
			{
				addBtn.IsVisible = true;
				addBtn.Text = "Join";
				addBtn.Clicked += async (sender, e) =>
				{
					await _dataManager.GroupApiManager.InviteAcceptDeclineLeaveGroup(group.GroupId, new List<Profile> { App.userProfile }, GroupApiManager.GroupHandlingType.Request);
				};
			}
		}
	}
}
