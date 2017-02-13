using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HowlOut
{
	public partial class ContentPageFrame : ContentPage
	{
		public ContentPageFrame(Grid ve)
		{
			InitializeComponent();
			NavigationPage.SetHasNavigationBar(this, false);
			mainView.Content = ve;
			UpperBar ub = new UpperBar();
			if (ub == null) return;
			topBarLayout.Children.Add(ub);
			ub.showBackButton(true);
		}

		public ContentPageFrame(ViewModelInterface cv, bool returnView)
		{
			InitializeComponent();
			NavigationPage.SetHasNavigationBar(this, false);
			mainView.Content = cv.getContentView();
			lateSetup(cv, returnView);
		}

		public async void lateSetup(ViewModelInterface cv, bool returnView)
		{
			try
			{
				UpperBar ub = await cv.getUpperBar();
				if (ub == null) return;
				topBarLayout.Children.Add(ub);
				if (returnView)
				{
					ub.showBackButton(true);
				}
			}
			catch (Exception exc) {}

		}
	}
}
