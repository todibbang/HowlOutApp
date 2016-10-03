using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public partial class UpperBar : ContentView
	{

		ScrollView scrollView;

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
				App.coreView.setContentViewWithQueue (new CreateGroup(null), "Create WolfPack", null);
			};				
			createNewGroupBtn.GestureRecognizers.Add(updateImage);

			var createImage = new TapGestureRecognizer();
			createImage.Tapped += async (sender, e) =>
			{
				await filterSearchBtn.ScaleTo(0.7, 50, Easing.Linear);
				await Task.Delay(60);
				await filterSearchBtn.ScaleTo(1, 50, Easing.Linear);
				App.coreView.setContentViewWithQueue(new FilterSearch(App.userProfile.SearchReference), "FilterSearch", null);
			};
			filterSearchBtn.GestureRecognizers.Add(createImage);

			navigationLabel.Clicked += (sender, e) =>
			{
				if (scrollView == null)
				{
					System.Diagnostics.Debug.WriteLine("ScrollView is null ");
				}

				if (scrollView != null)
				{
					scrollView.ScrollToAsync(scrollView.X, 0, true);
				}
			};

		}

		public void hideAll()
		{
			showCreateNewGroupButton(false);
			showFilterSearchButton(false);
			showBackButton(false);
			scrollView = null;
		}

		public void showCreateNewGroupButton(bool show)
		{
			createNewGroupBtn.IsVisible = show;
		}

		public void showFilterSearchButton(bool show)
		{
			filterSearchBtn.IsVisible = show;
		}

		public void setNavigationLabel(string label, ScrollView s)
		{
			navigationLabel.Text = label;
			scrollView = s;

			System.Diagnostics.Debug.WriteLine("label " + label );
			if (s == null)
			{
				System.Diagnostics.Debug.WriteLine("ScrollView is null ");
			}
		}

		public void showBackButton(bool active)
		{
			backBtn.IsVisible = active;
		}
	}
}

