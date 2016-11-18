using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace HowlOut
{
	public class GroupDesignView : GenericDesignView
	{
		Group group;
		//Design design = Design.ShowAll;

		public GroupDesignView(Group group, int dims, Design design) : base(dims)
		{
			this.group = group;
			try
			{
				SetInfo(group.ImageSource, group.Name, group.Description, design, ModelType.Group);
			}
			catch (Exception ex) { }
			subjBtn.Clicked += (sender, e) => { App.coreView.setContentViewWithQueue(new InspectController(group), "", null); };
			setInfo();
		}

		async void setInfo()
		{
			if(group != null)group = await new DataManager().GroupApiManager.GetGroup(group.GroupId);
			try
			{
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
					if (group.OrganizationOwner != null)
					{
						if (group.Members.Exists(p => p.ProfileId == App.StoredUserFacebookId))
						{
							removeBtn.IsVisible = true;
							removeBtn.Text = "Leave";
							removeBtn.Clicked += async (sender, e) =>
							{
								await _dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroup(group.GroupId, GroupApiManager.GroupHandlingType.Leave);
								App.coreView.setContentViewWithQueue(new InspectController(group), "", null);
							};
						}
						else {
							removeBtn.IsVisible = true;
							removeBtn.Text = "Joine";
							removeBtn.Clicked += async (sender, e) =>
							{
								await _dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroup(group.GroupId, GroupApiManager.GroupHandlingType.Request);
								App.coreView.setContentViewWithQueue(new InspectController(group), "", null);
							};
						}

					}
				}
				else if (App.userProfile.GroupsInviteTo.Exists(g => g.GroupId == group.GroupId))
				{
					addBtn.IsVisible = true;
					addBtn.Text = "Accept";
					addBtn.Clicked += async (sender, e) =>
					{
						await _dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroup(group.GroupId, GroupApiManager.GroupHandlingType.Accept);
						addBtn.IsEnabled = false;
						removeBtn.IsVisible = false;
					};
					removeBtn.IsVisible = true;
					removeBtn.Text = "Decline";
					removeBtn.Clicked += async (sender, e) =>
					{
						await _dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroup(group.GroupId, GroupApiManager.GroupHandlingType.Decline);
						removeBtn.IsEnabled = false;
						addBtn.IsVisible = false;
					};
				}
				else if (App.userProfile.Groups.Exists(g => g.GroupId == group.GroupId))
				{
					removeBtn.IsVisible = true;
					removeBtn.Text = "Leave";
					removeBtn.Clicked += async (sender, e) =>
					{
						await _dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroup(group.GroupId, GroupApiManager.GroupHandlingType.Leave);
						removeBtn.IsEnabled = false;
					};
				}
				else
				{
					addBtn.IsVisible = true;
					addBtn.Text = "Join";
					addBtn.Clicked += async (sender, e) =>
					{
						await _dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroup(group.GroupId, GroupApiManager.GroupHandlingType.Request);
						addBtn.IsEnabled = false;
					};
				}

				if (group.OrganizationOwner != null)
				{
					organizationImage.Source = group.OrganizationOwner.ImageSource;
					subName.Text = "owned by " + group.OrganizationOwner.Name;
					organizationImage.IsVisible = true;
					subName.IsVisible = true;
					var createImage = new TapGestureRecognizer();
					createImage.Tapped += async (sender, e) =>
					{
						App.coreView.GoToSelectedOrganization(group.OrganizationOwner.OrganizationId);
					};
					organizationImage.GestureRecognizers.Add(createImage);
				}

			}
			catch (Exception ex) {}
		}
	}
}
