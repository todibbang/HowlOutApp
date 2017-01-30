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
				string name = dims == 200 ? "" : group.Name;
				string imgs = dims == 200 ? group.LargeImageSource : group.ImageSource;
				SetInfo(imgs, name, group.Description, design, ModelType.Group, group.GroupId);
			}
			catch (Exception ex) { }

			subjBtn.Clicked += (sender, e) =>
			{
				if (dims >= 200)
				{
					OtherFunctions of = new OtherFunctions();
					of.ViewImages(new List<string>() { group.LargeImageSource });
				}
				else {
					App.coreView.setContentViewWithQueue(new InspectController(group));
				}
			};

			setInfo(dims);
		}

		async void setInfo(int dims)
		{
			if (group != null) group = await new DataManager().GroupApiManager.GetGroup(group.GroupId);
			try
			{
				if (App.coreView._dataManager.AreYouGroupOwner(group) || App.coreView._dataManager.AreYouGroupMember(group))
				{
					/*
					editBtn.IsVisible = true;
					editBtn.Text = "Edit";
					editBtn.Clicked += (sender, e) =>
					{
						App.coreView.setContentViewWithQueue(new CreateGroup(group, false));
					};

					addBtn.IsVisible = true;
					addBtn.Text = "Invite";
					addBtn.Clicked += (sender, e) =>
					{
						App.coreView.setContentViewWithQueue(new InviteListView(group, false));
					};

					removeBtn.IsVisible = true;
					removeBtn.Text = "Invite as owner";
					removeBtn.Clicked += (sender, e) =>
					{
						App.coreView.setContentViewWithQueue(new InviteListView(group, true));
					}; */
					/*
					if (group.OrganizationOwner != null)
					{
						if (group.Members.Exists(p => p.ProfileId == App.StoredUserFacebookId))
						{
							removeBtn.IsVisible = true;
							removeBtn.Text = "Leave";
							removeBtn.Clicked += async (sender, e) =>
							{
								await _dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroup(group.GroupId, GroupApiManager.GroupHandlingType.Leave);
								App.coreView.setContentViewWithQueue(new InspectController(group));
							};
						}
						else {
							removeBtn.IsVisible = true;
							removeBtn.Text = "Joine";
							removeBtn.Clicked += async (sender, e) =>
							{
								await _dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroup(group.GroupId, GroupApiManager.GroupHandlingType.Request);
								App.coreView.setContentViewWithQueue(new InspectController(group));
							};
						}

					}
					*/
				}
				else if (App.userProfile.GroupsInviteTo.Exists(g => g.GroupId == group.GroupId) || App.userProfile.GroupsInviteToAsOwner.Exists(g => g.GroupId == group.GroupId))
				{
					bool owner = false;
					if (App.userProfile.GroupsInviteToAsOwner.Exists(g => g.GroupId == group.GroupId)) { owner = true; }

					if (dims == 200)
					{
						displayGroupInvitedButtons(owner);
					}

					addBtn.IsVisible = true;
					addBtn.Text = "Accept";
					addBtn.Clicked += async (sender, e) =>
					{
						App.coreView.IsLoading(true);
						if (owner)
						{
							await _dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroupAsOwner(group.GroupId, OwnerHandlingType.Accept);
						}
						else {
							await _dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroup(group.GroupId, GroupApiManager.GroupHandlingType.Accept);
						}
						await _dataManager.ProfileApiManager.GetLoggedInProfile();

						App.coreView.reloadCurrentView();
						/*
						if (dims >= 200)
						{
							App.coreView.setContentViewReplaceCurrent(new InspectController(group), 1);
						}
						else {
							App.coreView.setContentView(4);
						} */
						App.coreView.IsLoading(false);
					};
					removeBtn.IsVisible = true;
					removeBtn.Text = "Decline";
					removeBtn.Clicked += async (sender, e) =>
					{
						App.coreView.IsLoading(true);
						if (owner)
						{
							await _dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroupAsOwner(group.GroupId, OwnerHandlingType.Decline);
						}
						else {
							await _dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroup(group.GroupId, GroupApiManager.GroupHandlingType.Decline);
						}
						await _dataManager.ProfileApiManager.GetLoggedInProfile();
						App.coreView.reloadCurrentView();
						/*
						if (dims >= 200)
						{
							App.coreView.setContentView(4);
						}
						else {
							App.coreView.setContentView(4);
						}*/
						App.coreView.IsLoading(false);
					};
					setPillButtonLayout(new List<Button>() { addBtn, removeBtn });
				}
				/*
				else if (App.userProfile.Groups.Exists(g => g.GroupId == group.GroupId))
				{
					removeBtn.IsVisible = true;
					removeBtn.Text = "Leave";
					removeBtn.Clicked += async (sender, e) =>
					{
						await _dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroup(group.GroupId, GroupApiManager.GroupHandlingType.Leave);
						await _dataManager.ProfileApiManager.GetLoggedInProfile();
						App.coreView.setContentView(4);
					};
				} */
				else if (!group.ProfilesRequestingToJoin.Exists(p => p.ProfileId == App.userProfile.ProfileId))
				{
					addBtn.IsVisible = true;
					addBtn.Text = "Join";
					addBtn.Clicked += async (sender, e) =>
					{
						await _dataManager.GroupApiManager.RequestAcceptDeclineLeaveGroup(group.GroupId, GroupApiManager.GroupHandlingType.Request);
						await _dataManager.ProfileApiManager.GetLoggedInProfile();
						App.coreView.reloadCurrentView();
						/*
						if (dims >= 200)
						{
							App.coreView.setContentViewReplaceCurrent(new InspectController(group), 1);
						}
						else {
							App.coreView.setContentView(4);
						}*/
					};
				}
			}
			catch (Exception ex) { }
		}
	}
}
