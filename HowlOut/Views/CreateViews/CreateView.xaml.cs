using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HowlOut
{
	public partial class CreateView : ContentView
	{
		public CreateEvent createEvent = new CreateEvent(new Event(), true);
		public CreateGroup createGroup = new CreateGroup(new Group(), true);
		public CreateGroup createOrganization = new CreateGroup(new Group(), true);

		public CreateView()
		{
			InitializeComponent();

			App.setOptionsGrid(optionGrid, new List<string> { "WolfPack", "Group", "Organization" }, new List<VisualElement> { createEventView, createGroupView, createOrganization }, new List<Action> { null, null, null });
			createEventView.Content = createEvent;
			createGroupView.Content = createGroup;
			createOrganizationView.Content = createOrganization;
		}
	}
}
