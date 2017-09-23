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

namespace CaffeineTracker
{
	class Drink
	{
		public string Name { get; set; }
		public double Size { get; set; }
		public double Caffeine { get; set; }

		public static Drink Parse(string[] attributes) => new Drink
		{
			Name = attributes[0],
			Size = double.Parse(attributes[1]),
			Caffeine = double.Parse(attributes[2])
		};
	}
}