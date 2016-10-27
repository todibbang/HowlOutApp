using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class CarouselList : ContentView
	{
		List<VisualElement> visualElements = new List<VisualElement>();
		List<Action> actions = new List<Action>();
		public int lastCarouselView = 0;

		public CarouselList(List<VisualElement> ve, List<string> titles)
		{
			InitializeComponent();
			visualElements = ve;
			carousel.ItemsSource = visualElements;


			foreach (VisualElement v in visualElements)
			{
				actions.Add(() => { carousel.Position = visualElements.FindIndex(p => p == v); } );
			}

			App.coreView.otherFunctions.setOptionsGrid(optionGrid, titles, null, actions, carousel);

			carousel.PositionSelected += (sender, e) =>
			{
				lastCarouselView = carousel.Position;
			};
		}

		public async Task setLastCarousel()
		{
			carousel.Position = 0;
			await Task.Delay(20);
			carousel.Position = lastCarouselView;
		}
	}
}
