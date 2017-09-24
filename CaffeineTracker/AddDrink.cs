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
using System.Text.RegularExpressions;

namespace CaffeineTracker
{
	[Activity(Label = "Select a drink", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, NoHistory = true, 
		Theme = "@android:style/Theme.DeviceDefault.DialogWhenLarge")]
	public class AddDrink : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.AddDrinkDialog);

			var data = Intent.GetStringArrayExtra("data");
			if (data.Length == 0)
			{
				SetResult(Result.Canceled);
				Finish();
				return;
			}
			var drinks = LoadDrinks();
			var items = data.Select(_ => drinks.First(x => x.Name == _)).ToArray();

			void SubmitDialog(int index)
			{
				var d = items[index];
				var intent = new Intent(this, typeof(MainActivity));
				intent.PutExtra("data", Drink.Serialize(d));
				SetResult(Result.Ok, intent);
				Finish();
			}

			if (data.Length == 1) SubmitDialog(0);

			var la = FindViewById<ListView>(Resource.Id.listView1);
			la.Adapter = new LVAdapter(this, items);

			la.ItemClick += (s, e) => SubmitDialog(e.Position);
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