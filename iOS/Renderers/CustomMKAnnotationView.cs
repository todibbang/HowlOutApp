using MapKit;

namespace HowlOut.iOS.CustomRenderers
{
	public class CustomMKAnnotationView : MKAnnotationView
	{
		public string Id { get; set; }

		public string Url { get; set; }

		public Event eve { get; set; }

		public CustomMKAnnotationView(IMKAnnotation annotation, string id)
			: base(annotation, id)
		{
		}
	}
}
