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

				int selected = 0;
				if (list.Contains(en)) { selected = 1; }
				EventCategoryButton b = new EventCategoryButton(en, selected, 60, list, limit);
				//typeButtons.Add(b);
				grid.Children.Add(b, column, row);
				b.button.Clicked += (sender, e) =>
				{
					selected++;


					if (limit)
					{
						if (selected > 1) selected = 0;

						if (list.Count == 3 && selected == 1)
						{
							selected = 0;
						}
					}
					else {

						if (selected > 2) selected = 0;

					}




					if (selected == 0)
					{
						list.Remove(en);
						System.Diagnostics.Debug.WriteLine("removed");
					} else if (selected == 1)
					{
						list.Add(en);
						System.Diagnostics.Debug.WriteLine("added");
					} else if (selected == 2)
					{
						list.Add(en);
						System.Diagnostics.Debug.WriteLine("blocked");
					}

					/*

					if (list.Contains(en))
					{
						list.Remove(en);
					}
					else if(!limit || (limit && list.Count < 3)){
						list.Add(en);
						System.Diagnostics.Debug.WriteLine("Added 1");
					}
					*/
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
