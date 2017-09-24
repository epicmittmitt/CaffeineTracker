﻿using System;
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
			if (view is null) view = _context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem2, parent, false);
			view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = _drinks[position].Name;
			view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = _drinks[position].ComputeCaffeine + " / " + _drinks[position].Caffeine + TimeSpan.FromHours(_drinks[position].ComputeTime).ToString();
			return view;
		}

		public override Drink this[int position] => _drinks[position];
		public override int Count => /*_drinks is null ? 0 :*/ _drinks.Length;
	}
}
