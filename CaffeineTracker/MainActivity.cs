using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Runtime;
using Android.Graphics;
using Java.IO;
using System.Text;
using System;

namespace CaffeineTracker
{
	[Activity(Label = "CaffeineTracker", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
		private string Key => Encoding.ASCII.GetString(Convert.FromBase64String("QUl6YVN5QXJ2WUk3cjh1SHhwMTh3enlkeU4wX1YyMHI3TEpTR0FJ"));

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);


			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			var button = FindViewById<Button>(Resource.Id.myButton);
			button.Click += delegate
			{
				StartActivityForResult(typeof(Activity1), 0);
			};
		}

		private async void ParseResponse(byte[] image)
		{
			var vw = new VisionWrapper(Key);
			foreach (var res in await vw.GetResponse(image)) System.Console.WriteLine(res);
		}

		protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
		{
			if (resultCode == Result.Ok)
			{
				ParseResponse(data.GetByteArrayExtra("image"));
			}
			base.OnActivityResult(requestCode, resultCode, data);
		}
	}
}

