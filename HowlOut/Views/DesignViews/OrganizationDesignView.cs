using System;

namespace HowlOut
{
	public class OrganizationDesignView : GenericDesignView
	{
		Organization org;

		public OrganizationDesignView(Organization org, int dims) : base(dims)
		{
			this.org = org;
			subjBtn.Clicked += (sender, e) => { App.coreView.setContentViewWithQueue(new InspectController(org), "", null); };
			setInfo();
		}

		async void setInfo()
		{
			org = await new DataManager().OrganizationApiManager.GetOrganization(org.OrganizationId);
			SetImage(org.ImageSource, org.Name);
			if (org.Members.Exists(p => p.ProfileId == App.StoredUserFacebookId))
			{
				addBtn.IsVisible = true;
				addBtn.Text = "Edit";
				addBtn.Clicked += (sender, e) =>
				{
					App.coreView.setContentViewWithQueue(new CreateOrganization(org, false), "", null);
				};

			}
			else {
				if (App.userProfile.OrganizationsInviteTo.Exists(o => o.OrganizationId == org.OrganizationId))
				{
					addBtn.IsVisible = true;
					addBtn.Text = "Accept";
					addBtn.Clicked += async (sender, e) =>
					{
						await _dataManager.OrganizationApiManager.AcceptInviteDeclineLeaveOrganization(org.OrganizationId, App.userProfile.ProfileId, OrganizationApiManager.OrganizationHandlingType.Accept);
					};
					removeBtn.IsVisible = true;
					removeBtn.Text = "Decline";
					removeBtn.Clicked += async (sender, e) =>
					{
						await _dataManager.OrganizationApiManager.AcceptInviteDeclineLeaveOrganization(org.OrganizationId, App.userProfile.ProfileId, OrganizationApiManager.OrganizationHandlingType.Decline);
					};
				}
				else if (App.userProfile.Organizations.Exists(o => o.OrganizationId == org.OrganizationId))
				{
					removeBtn.IsVisible = true;
					removeBtn.Text = "Leave";
					removeBtn.Clicked += async (sender, e) =>
					{
						await _dataManager.OrganizationApiManager.AcceptInviteDeclineLeaveOrganization(org.OrganizationId, App.userProfile.ProfileId, OrganizationApiManager.OrganizationHandlingType.Leave);
					};
				}
			}
		}
	}
}
