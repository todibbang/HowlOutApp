using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class CreateView : ContentView, ViewModelInterface
	{
		public Task<UpperBar> getUpperBar() { return null; }
		public ContentView getContentView() { return this; }
		public void reloadView() { }

		public ContentView CreateLayout() { return createLayout; }
		CreateEvent createEvent = new CreateEvent(new Event(), true);
		CreateGroup createGroup = new CreateGroup(new Group(), true);
		int eventOrGroup = 0;

		public CreateView()
		{
			InitializeComponent();

			upperEventBtn.Clicked += (sender, e) =>
			{
				createLayout.Content = null;
				createLayout.Content = createEvent;
				createLayout.IsVisible = true;
				upperEventBtn.BackgroundColor = App.HowlOut;
				upperGroupBtn.BackgroundColor = Color.Transparent;
				centerLayout.IsVisible = false;
				eventOrGroup = 1;
				eventLayout.IsVisible = true;
				groupLayout.IsVisible = false;
			};
			centerEventBtn.Clicked += (sender, e) =>
			{
				//App.coreView.setContentViewWithQueue(new CreateEvent(new Event(), true));

				createLayout.Content = null;
				createLayout.Content = createEvent;
				createLayout.IsVisible = true;
				upperEventBtn.BackgroundColor = App.HowlOut;
				upperGroupBtn.BackgroundColor = Color.Transparent;
				centerLayout.IsVisible = false;
				eventOrGroup = 1;
				eventLayout.IsVisible = true;
				groupLayout.IsVisible = false;
			};

			upperGroupBtn.Clicked += (sender, e) =>
			{
				createLayout.Content = null;
				createLayout.Content = createGroup;
				createLayout.IsVisible = true;
				upperGroupBtn.BackgroundColor = App.HowlOut;
				upperEventBtn.BackgroundColor = Color.Transparent;
				centerLayout.IsVisible = false;
				eventOrGroup = 2;
				eventLayout.IsVisible = false;
				groupLayout.IsVisible = true;
			};
			centerGroupBtn.Clicked += (sender, e) =>
			{
				//App.coreView.setContentViewWithQueue(new CreateGroup(new Group(), true));

				createLayout.Content = null;
				createLayout.Content = createGroup;
				createLayout.IsVisible = true;
				upperGroupBtn.BackgroundColor = App.HowlOut;
				upperEventBtn.BackgroundColor = Color.Transparent;
				centerLayout.IsVisible = false;
				eventOrGroup = 2;
				eventLayout.IsVisible = false;
				groupLayout.IsVisible = true;
			};

			var bckClick = new TapGestureRecognizer();
			bckClick.Tapped += (sender, e) =>
			{
				createEvent.otherViews.IsVisible = false;
				eventBackButton.IsVisible = false;
			};
			eventBackButton.GestureRecognizers.Add(bckClick);

			bckClick = new TapGestureRecognizer();
			bckClick.Tapped += (sender, e) =>
			{
				createGroup.otherViews.IsVisible = false;
				groupBackButton.IsVisible = false;
			};
			groupBackButton.GestureRecognizers.Add(bckClick);

		}

		public bool returnCreateView()
		{



			return true;
		}

		public void displayBackButton(bool show, int i)
		{
			if (i == 1) eventBackButton.IsVisible = show;
			if (i == 2) groupBackButton.IsVisible = show;
		}
	}
}
