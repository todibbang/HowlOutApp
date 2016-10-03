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
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			//Xamarin.Insights.Initialize (global::HowlOut.iOS.XamarinInsights.ApiKey);
			Xamarin.FormsMaps.Init();
			ImageCircleRenderer.Init ();
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}
