using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	class HSLV : BaseAdapter<Drink>
	{
		DetailedDrink[] _drinks;
		Activity _context;

		public HSLV(Activity context, DetailedDrink[] drinks) : base()
		{
			_context = context;
			_drinks = drinks;
		}

		public override long GetItemId(int position) => position;

		public override Java.Lang.Object GetItem(int position) => position;

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var drink = _drinks[position];
			var view = convertView;
			if (view is null) view = _context.LayoutInflater.Inflate(Resource.Layout.HomeScreenLV, parent, false);
			view.FindViewById<TextView>(Resource.Id.Title).Text = _drinks[position].Name;
			view.FindViewById<TextView>(Resource.Id.Text1).Text = Math.Round(_drinks[position].ComputeCaffeine, 2) + " mg / " + _drinks[position].Caffeine + " mg";
			
			return view;
		}

		public override Drink this[int position] => _drinks[position];
		public override int Count => _drinks.Length;
	}
}
