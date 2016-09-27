using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class UpperBar : ContentView
	{

		public UpperBar ()
		{
			InitializeComponent ();

			var backImage = new TapGestureRecognizer();
			backImage.Tapped += async (sender, e) => 
			{
				await backBtn.ScaleTo(0.7, 50, Easing.Linear);
				await Task.Delay(60);
				await backBtn.ScaleTo(1, 50, Easing.Linear);
				App.coreView.returnToPreviousView();
			};				
			backBtn.GestureRecognizers.Add(backImage); 

			var updateImage = new TapGestureRecognizer();
			updateImage.Tapped += async (sender, e) =>
			{
				await createNewGroupBtn.ScaleTo(0.7, 50, Easing.Linear);
				await Task.Delay(60);
				await createNewGroupBtn.ScaleTo(1, 50, Easing.Linear);
				App.coreView.setContentViewWithQueue (new CreateGroup(new Group()), "Create Group");
			};				
			createNewGroupBtn.GestureRecognizers.Add(updateImage);

			var createImage = new TapGestureRecognizer();
			createImage.Tapped += async (sender, e) =>
			{
				await filterSearchBtn.ScaleTo(0.7, 50, Easing.Linear);
				await Task.Delay(60);
				await filterSearchBtn.ScaleTo(1, 50, Easing.Linear);
				App.coreView.setContentViewWithQueue(new FilterSearch(App.userProfile.SearchReference), "FilterSearch");
			};
			filterSearchBtn.GestureRecognizers.Add(createImage);


		}

		public SearchBar getSearchBar()
		{
			return searchBar;
		}

		public Button[] getOptionButtons()
		{
			return new Button [] {optionOne, optionTwo};
		}

		public void hideAll()
		{
			showSearchBar(false);
			showCreateNewGroupButton(false);
			showFilterSearchButton(false);
			showBackButton(false);
			showOptionGrid(false, "", "");

			howlOut.IsVisible = true;
		}

		public void showSearchBar(bool show)
		{
			searchBar.IsVisible = show;
			howlOut.IsVisible = !show;
			searchBar.Text = "";
		}

		public void showCreateNewGroupButton(bool show)
		{
			createNewGroupBtn.IsVisible = show;
		}

		public void showFilterSearchButton(bool show)
		{
			filterSearchBtn.IsVisible = show;
		}

		public void setNavigationLabel(string label)
		{
			//navigationLabel.Text = label;
		}

		public void showBackButton(bool active)
		{
			backBtn.IsVisible = active;
		}

		public void showOptionGrid(bool show, string optionOneText, string optionTwoText)
		{
			optionGrid.IsVisible = show;
			optionGridBackground.IsVisible = show;
			optionOne.Text = optionOneText;
			optionTwo.Text = optionTwoText;
		}
	}
}

