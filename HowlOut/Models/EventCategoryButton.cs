using System;
using Xamarin.Forms;
using HowlOut;
using ImageCircle.Forms.Plugin.Abstractions;
using System.Collections.Generic;

namespace HowlOut
{
	public class EventCategoryButton : Grid
	{


		public int selectedState = 0;

		public Button button;

		List<EventType> eventCategoriesList;
		bool limit = false;


		public EventCategoryButton(EventType category, int selectedState, int dims, List<EventType> list, bool lim)
		{

			eventCategoriesList = list;
			limit = lim;

			this.BackgroundColor = Color.FromHex("#30ffffff");
			Padding = 0;
			RowSpacing = 0;

			RowDefinitions.Add(new RowDefinition { Height = dims });
			RowDefinitions.Add(new RowDefinition { Height = dims*0.3 });

			CircleImage i = new CircleImage()
			{
				Source = "category_" + category.ToString().ToLower() + "_ic.jpg",
				Aspect = Aspect.AspectFill,
				HeightRequest = dims,
				WidthRequest = dims
			};

			Children.Add(i, 0, 0);

			button = new Button()
			{
				BorderRadius = dims / 2,
				BorderWidth = 0,
				HeightRequest = dims,
				WidthRequest = dims,
				FontSize = 35,
				TextColor = App.NormalTextColor,
				//BackgroundColor = Color.Transparent,
			};

			select(button, selectedState); 

			button.Clicked += (sender, e) =>
			{
				selectedState++;





				if (limit)
				{
					if (selectedState > 1) selectedState = 0;

					if (eventCategoriesList.Count == 2 && selectedState == 1)
					{
						selectedState = 0;
						//App.coreView.displayAlertMessage("Max 2 Event Categories", "An event can have 2 categories max", "OK");
					}
				}
				else {
					
					if (selectedState > 1) selectedState = 0;

				}



				select(button, selectedState);
			};

			Children.Add(button, 0, 0);

			Label l = new Label()
			{
				FontSize = 12,
				Text = category.ToString(),
				HorizontalTextAlignment = TextAlignment.Center,
			};
			Children.Add(l, 0, 1);
		}

		private void select(Button b, int sele)
		{
			if (sele == 0)
			{
				b.BackgroundColor = Color.FromHex("#dcffffff");
				b.Text = "";
			}
			else if (sele == 1)
			{
				b.BackgroundColor = Color.Transparent;
				b.Text = "";
			}
			else if (sele == 2) {
				b.BackgroundColor = Color.FromHex("#acefc4c4");
				b.Text = "X";
			}
		}
	}
}
