using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using ImageCircle.Forms.Plugin.iOS;

namespace HowlOut.iOS
{
	public class Application
	{
		static void Main(string[] args)
		{
			Xamarin.FormsMaps.Init();
			ImageCircleRenderer.Init();
			UIApplication.Main(args, null, "AppDelegate");
		}
	}
}
