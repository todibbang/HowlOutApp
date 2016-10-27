using System;
using System.Collections.Generic;
namespace HowlOut
{
	public class GroupDesignView : GenericDesignView
	{
		Group group;
		//Design design = Design.ShowAll;

		public GroupDesignView(Group group, int dims, Design design) : base(dims)
		{
			this.group = group;
			SetInfo(group.ImageSource, group.Name, group.Description, design);
			subjBtn.Clicked += (sender, e) => { App.coreView.setContentViewWithQueue(new InspectController(group), "", null); };
			setInfo();
		}

		async void setInfo()
		{
			group = await new DataManager().GroupApiManager.GetGroup(group.GroupId);

			if (App.coreView._dataManager.AreYouGroupOwner(group))
			{
				editBtn.IsVisible = true;
				editBtn.Text = "Edit";
				editBtn.Clicked += (sender, e) =>
				{
					App.coreView.setContentViewWithQueue(new CreateGroup(group, false), "", null);
				};

				addBtn.IsVisible = true;
				addBtn.Text = "Invite";
				addBtn.Clicked += (sender, e) =>
				{
					App.coreView.setContentViewWithQueue(new InviteListView(group), "", null);
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
