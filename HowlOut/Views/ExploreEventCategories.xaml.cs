using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class ExploreEventCategories : ContentView
	{
		public ContentView content
		{
			get { return this; }
			set { this.content = value; }
		}

		List<string> partyBanners = new List<string>{"banner_blade.png", "banner_controller.png", "banner_donut.png", "banner_fodbold.png", "banner_golf.png", "banner_gravid.png", "banner_hjort.png",
			"banner_kaffe.png", "banner_marked.png", "banner_publikum.png", "banner_scrabble.png", "banner_skak.png", "banner_skater.png", "banner_skovsoe.png", "banner_surf.png"};

		public ExploreEventCategories()
		{
			InitializeComponent();
			List<EventCategoryListItem> CategoryItems = new List<EventCategoryListItem>();
			for (int i = 0; i < partyBanners.Count; i++)
			{
				CategoryItems.Add(new EventCategoryListItem() {ImageSource = partyBanners[i], Category = (EventCategory.Category) i });
			}

			categoryList.ItemsSource = CategoryItems;
			categoryList.ItemSelected += OnListItemSelected;

			EventListView eventListView = new EventListView(0);

			eventList.Children.Add(eventListView);

			searchBar.TextChanged += async (sender, e) =>
			{
				if (string.IsNullOrWhiteSpace(searchBar.Text))
				{
					categoryList.IsVisible = true;
					categoryList.ItemsSource = CategoryItems;
					await Task.Delay(10);
					DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
				}
				else {
					categoryList.IsVisible = false;
					eventListView.UpdateList(true, searchBar.Text);
				}
			};
			categoryList.Focused += async (sender, e) =>
			{
				await Task.Delay(10);
				DependencyService.Get<ForceCloseKeyboard>().CloseKeyboard();
			};
		}

		public async void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("hmm");

			try
			{
				categoryList.ScrollTo(categoryList.SelectedItem, ScrollToPosition.Center, true);
				EventCategoryListItem selectedCategory = categoryList.SelectedItem as EventCategoryListItem;
				categoryImage.Source = selectedCategory.ImageSource;
				await Task.Delay(500);
				categoryImage.IsVisible = true;
				categoryImage.FadeTo(0, 300, null);
				await categoryImage.ScaleTo(2, 150, null);
				categoryList.IsVisible = false;
				await categoryImage.ScaleTo(4, 150, null);
				categoryImage.Scale = 1;
				categoryImage.Opacity = 1;
				categoryImage.IsVisible = false;
				searchBar.Text = "#" + selectedCategory.Category;
				categoryList.SelectedItem = null;
			}
			catch (Exception ex)
			{

			}


		}
	}
}
