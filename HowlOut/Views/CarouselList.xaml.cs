using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class CarouselList : ContentView, ViewModelInterface
	{
		List<VisualElement> visualElements = new List<VisualElement>();
		List<Action> actions = new List<Action>();
		public int lastCarouselView = 0;
		public int veryLastCarouselView = 0;
		ViewType viewType;


		public CarouselList(List<VisualElement> ve, List<string> titles, ViewType type)
		{
			InitializeComponent();
			visualElements = ve;
			viewType = type;
			carousel.ItemsSource = visualElements;


			foreach (VisualElement v in visualElements)
			{
				actions.Add(() => { 
					carousel.Position = visualElements.FindIndex(p => p == v); 
				});
			}

			App.coreView.otherFunctions.setOptionsGrid(optionGrid, titles, null, actions, carousel);

			carousel.PositionSelected += (sender, e) =>
			{
				lastCarouselView = carousel.Position;
				DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			};
		}

		public void viewInFocus(UpperBar bar)
		{
			if (viewType == ViewType.Conversations)
			{
				bar.setNavigationLabel("Conversations", null);
			}
			else if (viewType == ViewType.SearchEvents)
			{
				bar.setNavigationLabel("Explore Events", null);
				/*
				bar.setRightButton("ic_settings.png").Clicked += (sender, e) =>
				{
					if (App.userProfile != null && App.userProfile.SearchPreference != null)
						App.coreView.setContentViewWithQueue(new FilterSearch(App.userProfile.SearchPreference));
				};*/
			}
			else if (viewType == ViewType.JoinedEvents)
			{
				bar.setNavigationLabel("Joined Events", null);
			}
			else if (viewType == ViewType.Create)
			{
				bar.setNavigationLabel("Create", null);
			}
			else if (viewType == ViewType.Home)
			{
				bar.setNavigationLabel("Home", null);
			}
			setCarousel(veryLastCarouselView);
		}

		public void viewExitFocus()
		{
			veryLastCarouselView = lastCarouselView;
			carousel.Position = 0;
		}

		public ContentView getContentView() { return this; }

		public async Task setCarousel(int i)
		{
			carousel.Position = 0;
			await Task.Delay(2);
			if (i <= visualElements.Count - 1)
			{
				carousel.Position = i;
			}
		}

		public enum ViewType
		{
			Conversations,
			Create,
			JoinedEvents,
			SearchEvents,
			Other,
			Home
		}
	}
}
