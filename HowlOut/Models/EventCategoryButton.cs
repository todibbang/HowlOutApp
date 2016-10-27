using System;
using Xamarin.Forms;
using HowlOut;
using ImageCircle.Forms.Plugin.Abstractions;
using System.Collections.Generic;

namespace HowlOut
{
	public class EventCategoryButton : Grid
	{


		public bool selected = false;

		public Button button;

		List<EventType> eventCategoriesList;
		bool limit = false;


		public EventCategoryButton(EventType category, bool selected, int dims, List<EventType> list)
		{
			if (list != null)
			{
				limit = true;
				eventCategoriesList = list;
			}

			this.selected = selected;
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
				BorderRadius = dims/2,
				BorderWidth = 3,
				HeightRequest = dims,
				WidthRequest = dims,
				BackgroundColor = Color.Transparent,
			};

			select(button, selected); 

			button.Clicked += (sender, e) =>
			{
				selected = !selected;

				if (limit && eventCategoriesList.Count == 3 && selected)
				{
					selected = false;
				}
				select(button, selected);
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

		private void select(Button b, bool sele)
		{
			if (sele )
			{
				b.BorderColor = App.HowlOut;
				System.Diagnostics.Debug.WriteLine("Added 2");
			}
			else {
				b.BorderColor = Color.Transparent;
			}
		}
	}
}
