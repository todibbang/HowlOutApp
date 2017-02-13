using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace HowlOut
{
	public interface ViewModelInterface
	{
		Task<UpperBar> getUpperBar();
		ContentView getContentView();
		void reloadView();
	}
}
