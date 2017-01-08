using System;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Net.Http;
using ModernHttpClient;
using System.Collections.Generic;
using Plugin.Media;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace HowlOut
{
	public class UtilityManager
	{
		HttpClient httpClient;

		public UtilityManager ()
		{
			httpClient = new HttpClient(new NativeMessageHandler());
		}

		public async void updateLastKnownPosition()
		{
			if (CrossGeolocator.Current.IsGeolocationEnabled && CrossGeolocator.Current.IsGeolocationAvailable)
			{
				try
				{
					var locator = CrossGeolocator.Current;
					locator.DesiredAccuracy = 50;
					var position = await locator.GetPositionAsync(timeoutMilliseconds: 10000);
					App.lastKnownPosition = new Position(position.Latitude, position.Longitude);
					System.Diagnostics.Debug.WriteLine("Last Known Position: " + App.lastKnownPosition.Latitude + "" + App.lastKnownPosition.Longitude);
				}
				catch (Exception ex)
				{

				}
			}
			else {
				App.lastKnownPosition = new Position(55.679802, 12.585466);
			}
		}

		public async void setMapForEvent(Position pos, ExtMap map, StackLayout mapLayout)
		{
			map.MoveToRegion (
				MapSpan.FromCenterAndRadius (
					new Position (pos.Latitude, pos.Longitude), Distance.FromKilometers (1.2)));
			mapLayout.Children.Add(map);
		}

		public async void setPin(Position pos, ExtMap map, String label, String address)
		{
			map.Pins.Clear ();
			var pin = new Pin
			{
				Type = PinType.Place,
				Position = new Position(pos.Latitude,pos.Longitude),
				Label = label,
				Address = address,
			};
			map.Pins.Add (pin);
		}

		public string distance(Position position1, Position position2) {

			double lat1 = position1.Latitude;
			double lon1 = position1.Longitude;
			double lat2 = position2.Latitude;
			double lon2 = position2.Longitude;
			double theta = lon1 - lon2;
			double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
			dist = Math.Acos(dist);
			dist = rad2deg(dist);
			dist = dist * 60 * 1.1515;
			//if (unit == 'K') {
			dist = dist * 1.609344;
			//} else if (unit == 'N') {
			//	dist = dist * 0.8684;
			//}

			/*
			string distance = "";
			if (dist < 1)
				distance = "less than 1 km away";
			else {
				int Dist = (int)dist;
				distance = Dist + " km away";
			}
			*/		


			return (((int)dist) + "");
		}

		private double deg2rad(double deg) {
			return (deg * Math.PI / 180.0);
		}

		private double rad2deg(double rad) {
			return (rad / Math.PI * 180.0);
		}

		public string getTime(DateTime time)
		{
			string hour = time.Hour.ToString();
			if (hour.Length == 1) {
				hour = "0" + time.Hour.ToString ();
			}

			string minute = time.Minute.ToString();
			if(minute.Length == 1) minute = "0" + time.Minute.ToString();

			string newTime = hour + ":" + minute;

			return newTime;
		}

		public List<string> setTime(DateTime time) {
			var theTimeNow = DateTime.Now;
			var timeBetween = time - theTimeNow;
			string number;
			string describer;

			if (timeBetween.TotalDays < 1) {
				number = timeBetween.Hours + "";
				describer = "hour";
			} else if (timeBetween.TotalDays < 7) {
				number = (time.TimeOfDay + "").Substring(0, 5);
				describer = (time.DayOfWeek + "").Substring(0, 3);
			} else {
				number = time.Day + "";
				describer = time.ToString("MMMM") + "";
			}

			return new List<string> () { number, describer };
		}

		/*
		public async Task<List<Plugin.Media.Abstractions.MediaFile>>  TakePicture()
		{
			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				return null;
			}
			List<Plugin.Media.Abstractions.MediaFile> files = new List<Plugin.Media.Abstractions.MediaFile>();

			var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
			{

				Directory = "Sample",
				Name = "test.jpg",
				CompressionQuality = 0,
				PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small
			});



			files.Add(file);

			//Plugin.Media.Abstractions.StoreCameraMediaOptions g = new Plugin.Media.Abstractions.StoreCameraMediaOptions();
			//g.PhotoSize = Plugin.Media.Abstractions.PhotoSize.

			if (file == null)
				return null;

			//s = file.GetStream();

			return files;

		}*/

		public async Task<List<byte[]>> TakePicture(Image imgSrc)
		{
			if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				return null;
			}
			var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
			{
				Directory = "Sample",
				Name = "test.jpg",
				CompressionQuality = 0,
				PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small
			});

			if (file == null)
				return null;

			using (var memoryStream = new MemoryStream())
			{
				file.GetStream().CopyTo(memoryStream);

				List<byte[]> mStream = new List<byte[]>() {
					DependencyService.Get<ImageResizer>().ResizeImage(memoryStream.ToArray(), 50),
					DependencyService.Get<ImageResizer>().ResizeImage(memoryStream.ToArray(), 100),
					DependencyService.Get<ImageResizer>().ResizeImage(memoryStream.ToArray(), 300)
				};
				if (imgSrc != null) imgSrc.Source = ImageSource.FromStream(() => new MemoryStream(mStream[2]));

				await Task.Delay(10);
				file.Dispose();
				return mStream;
			}
		}

		public async Task<List<byte[]>> PictureFromAlbum(Image imgSrc)
		{
			if (!CrossMedia.Current.IsPickPhotoSupported)
			{
				return null;
			}
			var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
			{
				CompressionQuality = 60,
				PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
			});

			if (file == null)
				return null;
			
			using (var memoryStream = new MemoryStream())
			{
				file.GetStream().CopyTo(memoryStream);

				List<byte[]>  mStream = new List<byte[]>() {
					DependencyService.Get<ImageResizer>().ResizeImage(memoryStream.ToArray(), 50),
					DependencyService.Get<ImageResizer>().ResizeImage(memoryStream.ToArray(), 100),
					DependencyService.Get<ImageResizer>().ResizeImage(memoryStream.ToArray(), 300)
				};
				if (imgSrc != null) imgSrc.Source = ImageSource.FromStream(() => new MemoryStream(mStream[2]));
				await Task.Delay(10);
				file.Dispose();
				return mStream;

				//new MemoryStream();
			}
		}
		/*
		public async Task<Plugin.Media.Abstractions.MediaFile> PictureFromAlbum()
		{
			if (!CrossMedia.Current.IsPickPhotoSupported)
			{
				//DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
				return null;
			}
			var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
			{
				CompressionQuality = 60,
				PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,

			});



			if (file == null)
				return null;

			return file;
		} */

		public async Task<String> UploadImageToStorage(Stream imageStream, string imageName)
		{
			// Retrieve storage account from connection string.
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=howloutstorage;AccountKey=gY2mb3lGUcOWPEHVpNBJhqXNrJYxgOemQKoJUD4lD17czV5RSDfV6b0ot/lwj/2fVmoYEWXx5W711iWmk7BiNg==");

			// Create the blob client.
			CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

			// Retrieve reference to a previously created container.
			CloudBlobContainer container = blobClient.GetContainerReference("howlout");

			// Create the container if it doesn't already exist.
			await container.CreateIfNotExistsAsync();

			// Retrieve reference to a blob named "myblob".
			CloudBlockBlob blockBlob = container.GetBlockBlobReference(imageName);

			// Set type of blob time image
			blockBlob.Properties.ContentType = "image/jpg";
			await blockBlob.UploadFromStreamAsync(imageStream);
			// Return blob uri
			return blockBlob.Uri.ToString();
		}
	}
}

