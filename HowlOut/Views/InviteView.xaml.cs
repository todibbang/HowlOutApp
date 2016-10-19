using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Net.Http;
using ModernHttpClient;
using ImageCircle.Forms.Plugin.Abstractions;
using System.Linq;

namespace HowlOut
{
	public partial class InviteView : ContentView
	{
		DataManager _dataManager = new DataManager ();
		ListsAndButtons listMaker = new ListsAndButtons();

		ListsAndButtons.ListType ListType = ListsAndButtons.ListType.Normal;

		private List<Profile> profilesToFind = new List<Profile>();
		private List<Group> groupsToFind = new List<Group>();
		private List<Profile> profilesNotToFind = new List<Profile>();

		public InviteView (Group groupObject, Event eventObject, Organization orgObject, WhatToShow whatToShow)
		{
			InitializeComponent ();

			Dictionary<Profile, string> profilesNotToInvite = new Dictionary<Profile, string> { };

			profilesToFind = App.userProfile.Friends;
			ListType = ListsAndButtons.ListType.Invite;

			if (whatToShow.Equals(WhatToShow.PeopleToInviteToEvent)) {
				profilesNotToFind = eventObject.Attendees;
				foreach (Profile p in eventObject.InvitedProfiles) {
					if (!profilesNotToFind.Exists(pro => pro.ProfileId == p.ProfileId)) { profilesNotToFind.Add(p); }
				}
				groupsToFind = App.userProfile.Groups;
			}
			else if (whatToShow.Equals(WhatToShow.PeopleToInviteToGroup)) {
				profilesNotToFind = groupObject.Members;
			}
			else if (whatToShow.Equals(WhatToShow.PeopleToInviteToOrganization)) {
				profilesNotToFind = orgObject.Members;
			}

			for (int i = 0; i < profilesNotToFind.Count; i++) {
				profilesNotToInvite.Add (profilesNotToFind [i], profilesNotToFind [i].ProfileId);
			}

			for (int i = profilesToFind.Count - 1; i > -1; i--) {
				if (profilesNotToInvite.ContainsValue (profilesToFind [i].ProfileId)) {
					profilesToFind.Remove (profilesToFind [i]);
				}
			}

			if(profilesToFind.Count > 0) listMaker.createList (profileGrid, profilesToFind, null, null, eventObject, groupObject, orgObject);


			App.setOptionsGrid(optionGrid, new List<string> { "Profiles", "Groups" }, new List<VisualElement> { profileGrid, groupGrid }, null, null);
		}

		public enum WhatToShow {
			PeopleToInviteToEvent,
			PeopleToInviteToGroup,
			PeopleToInviteToOrganization,
		}
	}
}

