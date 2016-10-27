using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace HowlOut
{
	public class EventCategory
	{
		public string Name { get; set; }

		public EventCategory(Category category)
		{
			
		}

		public static void ManageCategories(Grid grid, List<EventType> list, bool limit)
		{
			int row = 0;
			int column = 1;
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			foreach (EventType en in Enum.GetValues(typeof(EventType)))
			{
				if (column == 9)
				{
					grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
					column = 1;
					row++;
				}

				bool selected = false;
				if (list.Contains(en)) { selected = true; }
				EventCategoryButton b = new EventCategoryButton(en, selected, 60, list);
				//typeButtons.Add(b);
				grid.Children.Add(b, column, row);
				b.button.Clicked += (sender, e) =>
				{
					if (list.Contains(en))
					{
						list.Remove(en);
					}
					else if(!limit || (limit && list.Count < 3)){
						list.Add(en);
						System.Diagnostics.Debug.WriteLine("Added 1");
					}
				};
				column += 2;
			}
		}

		public enum Category
		{
			Party,
			Sport,
			Culture,
			Movie,
			Music,
			Politics,
			Cafe,
			Food,
			Hobby,
			Animals,
			Study,
			Games,
			Fitness,
			Charity,
			Work,
			Other
		}
	}
}
