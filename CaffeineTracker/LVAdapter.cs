﻿using System;
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
	class LVAdapter : BaseAdapter<Drink>
	{
		Drink[] _drinks;
		Activity _context;

		public LVAdapter(Activity context, Drink[] drinks) : base() {
			_context = context;
			_drinks = drinks;
		}

		public override long GetItemId(int position) => position;

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var view = convertView;
			var drink = _drinks[position];
			if (view is null) view = _context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem2, parent, false);
			view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = System.Net.WebUtility.UrlDecode(drink.Name);	
			view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = drink.Caffeine + " mg";
			return view;
		}

		public override int Count => _drinks.Length;

		public override Drink this[int position] => _drinks[position];
	}
}