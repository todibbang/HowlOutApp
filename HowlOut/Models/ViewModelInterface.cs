using System;
using Xamarin.Forms;

namespace HowlOut
{
	public interface ViewModelInterface
	{
		void viewInFocus(UpperBar bar);
		ContentView getContentView();
		void viewExitFocus();
		void reloadView();
	}
}
