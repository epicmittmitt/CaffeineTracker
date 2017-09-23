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
using System.Runtime.Serialization;

namespace CaffeineTracker
{
	class Drink
	{
		public string Name { get; set; }
		public double Size { get; set; }
		public double Caffeine { get; set; }

		internal static string[] Serialize(Drink d) => new[] { d.Name, d.Size.ToString(), d.Caffeine.ToString() };
		internal static Drink Deserialize(string[] s) => new Drink { Name = s[0], Size = double.Parse(s[1]), Caffeine = double.Parse(s[2]) };
	}
}
