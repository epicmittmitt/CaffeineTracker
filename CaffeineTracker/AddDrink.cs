using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace CaffeineTracker
{
	[Activity(Label = "Add Drink", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, NoHistory = true)]
	public class AddDrink : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.AddDrinkDialog);

			var data = Intent.GetStringArrayExtra("data");
			var drinks = LoadDrinks();
			var items = data.Select(_ => drinks.First(x => x.Name == _)).ToArray();
			var la = FindViewById<ListView>(Resource.Id.listView1);
			la.Adapter = new LVAdapter(this, items);

			la.ItemClick += (s, e) =>
			{
				var d = items[e.Position];
				var intent = new Intent(this, typeof(MainActivity));
				intent.PutExtra("data", Drink.Serialize(d));
				SetResult(Result.Ok, intent);
				Finish();
			};
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
	}
}