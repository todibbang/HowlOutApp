/*

using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public class OrganizationDesignView : GenericDesignView
	{
		Organization org;

		public OrganizationDesignView(Organization org, int dims, Design design) : base(dims)
		{
			this.org = org;
			SetInfo(org.ImageSource, org.Name, org.Description, design, ModelType.Organization);
			subjBtn.Clicked += (sender, e) => { App.coreView.setContentViewWithQueue(new InspectController(org)); };
			setInfo();
		}

		async void setInfo()
		{
			org = await new DataManager().OrganizationApiManager.GetOrganization(org.OrganizationId);
			if (org.Members.Exists(p => p.ProfileId == App.StoredUserFacebookId))
			{
				editBtn.IsVisible = true;
				editBtn.Text = "Edit";
				editBtn.Clicked += (sender, e) =>
				{
					App.coreView.setContentViewWithQueue(new CreateOrganization(org, false));
				};

				addBtn.IsVisible = true;
				addBtn.Text = "Invite";
				addBtn.Clicked += (sender, e) =>
				{
					App.coreView.setContentViewWithQueue(new InviteListView(org));
				};

				removeBtn.IsVisible = true;
				removeBtn.Text = "Leave";
				removeBtn.Clicked += async (sender, e) =>
				{
					await _dataManager.OrganizationApiManager.AcceptDeclineLeaveOrganization(org.OrganizationId, OrganizationApiManager.OrganizationHandlingType.Leave);
					App.coreView.setContentViewReplaceCurrent(new InspectController(org), 1);
				};
			}
			else {
				if (App.userProfile.OrganizationsInviteTo.Exists(o => o.OrganizationId == org.OrganizationId))
				{
					addBtn.IsVisible = true;
					addBtn.Text = "Accept";
					addBtn.Clicked += async (sender, e) =>
					{
						await _dataManager.OrganizationApiManager.AcceptDeclineLeaveOrganization(org.OrganizationId, OrganizationApiManager.OrganizationHandlingType.Accept);
						removeBtn.IsVisible = false;
						addBtn.IsEnabled = false;
					};
					removeBtn.IsVisible = true;
					removeBtn.Text = "Decline";
					removeBtn.Clicked += async (sender, e) =>
					{
						await _dataManager.OrganizationApiManager.AcceptDeclineLeaveOrganization(org.OrganizationId, OrganizationApiManager.OrganizationHandlingType.Decline);
						removeBtn.IsEnabled = false;
						addBtn.IsVisible = false;
					};
					setPillButtonLayout(new List<Button>() { addBtn, removeBtn });
				}
				else if (App.userProfile.Organizations.Exists(o => o.OrganizationId == org.OrganizationId))
				{
					removeBtn.IsVisible = true;
					removeBtn.Text = "Leave";
					removeBtn.Clicked += async (sender, e) =>
					{
						await _dataManager.OrganizationApiManager.AcceptDeclineLeaveOrganization(org.OrganizationId, OrganizationApiManager.OrganizationHandlingType.Leave);
					};
				}
			}
		}
	}
}
*/