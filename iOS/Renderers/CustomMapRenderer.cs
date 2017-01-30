using System;
using Xamarin.Forms;
using HowlOut;
using HowlOut.iOS.CustomRenderers;
using Xamarin.Forms.Maps.iOS;
using UIKit;
using MapKit;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Platform.iOS;
using System.Collections.Generic;
using CoreGraphics;
using ImageCircle;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace HowlOut.iOS.CustomRenderers
{
	public class CustomMapRenderer : MapRenderer
	{
		UIView customPinView;
		List<CustomPin> customPins;

		protected override void OnElementChanged(ElementChangedEventArgs<View> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				var nativeMap = Control as MKMapView;
				nativeMap.GetViewForAnnotation = null;
				nativeMap.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
				nativeMap.DidSelectAnnotationView -= OnDidSelectAnnotationView;
				nativeMap.DidDeselectAnnotationView -= OnDidDeselectAnnotationView;
			}

			if (e.NewElement != null)
			{
				var formsMap = (CustomMap)e.NewElement;
				var nativeMap = Control as MKMapView;
				customPins = formsMap.CustomPins;

				nativeMap.GetViewForAnnotation = GetViewForAnnotation;
				nativeMap.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
				nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;
				nativeMap.DidDeselectAnnotationView += OnDidDeselectAnnotationView;
			}
		}



		MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
		{
			MKAnnotationView annotationView = null;

			if (annotation is MKUserLocation)
				return null;

			var anno = annotation as MKPointAnnotation;
			var customPin = GetCustomPin(anno);
			if (customPin == null)
			{
				throw new Exception("Custom pin not found");
			}

			annotationView = mapView.DequeueReusableAnnotation(customPin.Id);
			if (annotationView == null)
			{
				annotationView = new CustomMKAnnotationView(annotation, customPin.Id);
				//annotationView.Image = UIImage.FromFile("ic_add.png");
				annotationView.Image = UIImage.FromFile("pin.png");
				//annotationView.


				annotationView.CalloutOffset = new CGPoint(0, 0);

				//annotationView.LeftCalloutAccessoryView = new UIImageView(UIImage.FromFile("default_icon.png"));
				//annotationView.DetailCalloutAccessoryView = new UIImageView(UIImage.FromFile("default_icon.png"));

				annotationView.RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure);
				((CustomMKAnnotationView)annotationView).Id = customPin.Id;
				((CustomMKAnnotationView)annotationView).Url = customPin.Url;
				((CustomMKAnnotationView)annotationView).eve = customPin.eve;
			}
			annotationView.CanShowCallout = true;

			return annotationView;
		} 

		void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
		{
			var customView = e.View as CustomMKAnnotationView;
			customPinView = new UIView();

			if (customView.Id == "Xamarin")
			{
				
				customPinView.Frame = new CGRect(120, 400, 200, 84);
				var image = new UIImageView(new CGRect(0, 0, 200, 84));
				image.Image = UIImage.FromFile(customView.eve.ImageSource);
				customPinView.AddSubview(image);
				customPinView.Center = new CGPoint(0, -(e.View.Frame.Height + 75));
				e.View.AddSubview(customPinView);

			}
		}

		void OnCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e)
		{
			var customView = e.View as CustomMKAnnotationView;
			if (!string.IsNullOrWhiteSpace(customView.Url))
			{
				//UIApplication.SharedApplication.OpenUrl(new Foundation.NSUrl(customView.Url));
				App.coreView.GoToSelectedEvent(customView.Url);
			}
		}

		void OnDidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e)
		{
			if (!e.View.Selected)
			{
				customPinView.RemoveFromSuperview();
				customPinView.Dispose();
				customPinView = null;
			}
		}

		CustomPin GetCustomPin(MKPointAnnotation annotation)
		{
			var position = new Position(annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
			foreach (var pin in customPins)
			{
				if (pin.Pin.Position == position)
				{
					return pin;
				}
			}
			return null;
		}
    }
}
