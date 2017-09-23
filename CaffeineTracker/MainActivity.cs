using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Runtime;
using Android.Graphics;
using Java.IO;
using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Environment = Android.OS.Environment;
using Android.Views;

namespace CaffeineTracker
{
	[Activity(Label = "Caffeine Tracker", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
		private string Key => Encoding.ASCII.GetString(Convert.FromBase64String("QUl6YVN5QXJ2WUk3cjh1SHhwMTh3enlkeU4wX1YyMHI3TEpTR0FJ"));

		protected override void OnCreate(Bundle savedInstanceState)
		{
			RequestWindowFeature(WindowFeatures.NoTitle);
			base.OnCreate(savedInstanceState);
			Window.AddFlags(WindowManagerFlags.ForceNotFullscreen);
			SetContentView(Resource.Layout.Main);

			var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar1);
			SetActionBar(toolbar);
			ActionBar.Title = "Caffeine Tracker";
		}

		internal Drink[] LoadDrinks()
		{
			var _csv = Assets.Open("Drinks.csv");
			var reader = new StreamReader(_csv);
			var csv = reader.ReadToEnd().Split('\n');
			var drinks = new List<Drink>();
			foreach (var line in csv)
			{
				var a = line.Split('~');
				drinks.Add(Drink.Deserialize(a));
			}
			return drinks.ToArray();
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.Menu1, menu);
			var openCamera = menu.FindItem(Resource.Id.openCamera);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			if (item.TitleFormatted.ToString() == "Add an Image")
			{
				StartActivityForResult(typeof(Activity1), 0);
			}
			return base.OnOptionsItemSelected(item);
		}

		private async void ParseResponse(byte[] image)
		{
			var vw = new VisionWrapper(Key);
			var requests = vw.BuildRequest(vw.ToBase64(image));
			var responses = new[] { await vw.GetResponse(requests[0]), await vw.GetResponse(requests[1]) };
			var response = string.Join("\n", responses);
			var buzz = Regex.Matches(response, "\"description\": \"(.*?)\"", RegexOptions.Singleline).Cast<Match>().Select(_ => _.Groups[1].Value).ToArray();
			var oldMatches = LoadDrinks();
			foreach (var b in buzz)
			{
				var newMatches = oldMatches.Where(_ => _.Name.Contains(b)).ToArray();
				if (newMatches.Length == 0) break;
				oldMatches = newMatches;
				if (newMatches.Length <= 3) break;
			}
			var intent = new Intent(this, typeof(AddDrink));
			intent.PutExtra("data", oldMatches.Select(_ => _.Name).Take(10).ToArray());
			StartActivityForResult(intent, 1);
		}

		protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
		{
			if (resultCode == Result.Ok)
			{
				if (requestCode == 0) ParseResponse(data.GetByteArrayExtra("image"));
				else if (requestCode == 1) ;
			}
			base.OnActivityResult(requestCode, resultCode, data);
		}

		public static int GetDamerauLevenshteinDistance(string s, string t)
		{
			var bounds = new { Height = s.Length + 1, Width = t.Length + 1 };
			var matrix = new int[bounds.Height, bounds.Width];
			for (var height = 0; height < bounds.Height; height++)
			{
				matrix[height, 0] = height;
			}
			for (var width = 0; width < bounds.Width; width++)
			{
				matrix[0, width] = width;
			}
			for (var height = 1; height < bounds.Height; height++)
			{
				for (var width = 1; width < bounds.Width; width++)
				{
					var cost = (s[height - 1] == t[width - 1]) ? 0 : 1;
					var insertion = matrix[height, width - 1] + 1;
					var deletion = matrix[height - 1, width] + 1;
					var substitution = matrix[height - 1, width - 1] + cost;
					var distance = Math.Min(insertion, Math.Min(deletion, substitution));
					if (height > 1 && width > 1 && s[height - 1] == t[width - 2] && s[height - 2] == t[width - 1])
					{
						distance = Math.Min(distance, matrix[height - 2, width - 2] + cost);
					}
					matrix[height, width] = distance;
				}
			}
			return matrix[bounds.Height - 1, bounds.Width - 1];
		}
	}
}
