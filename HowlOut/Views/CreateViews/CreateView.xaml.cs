using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class CreateView : ContentView
	{
		public CreateEvent createEvent = new CreateEvent(new Event(), true);
		public CreateGroup createGroup = new CreateGroup(new Group(), true);
		public CreateOrganization createOrganization = new CreateOrganization(new Organization(), true);

		List<VisualElement> createViews = new List<VisualElement>();

		public int lastCarouselView = 0;

		public CreateView()
		{
			InitializeComponent();

			createViews.Add(createEvent);
			createViews.Add(createGroup);
			createViews.Add(createOrganization);
			createCarousel.ItemsSource = createViews;

			App.setOptionsGrid(optionGrid, new List<string> { "Event", "Group", "Organization" }, new List<VisualElement> { null, null, null }, new List<Action> { () => { createCarousel.Position = 0; }, () => { createCarousel.Position = 1; }, () => { createCarousel.Position = 2; } }, createCarousel);

			createCarousel.PositionSelected += (sender, e) =>
			{
				lastCarouselView = createCarousel.Position;
			};
		}

		public async Task setLastCarousel() {
			createCarousel.Position = 0;
			await Task.Delay(20);
			createCarousel.Position = lastCarouselView;
		}
	}
}
