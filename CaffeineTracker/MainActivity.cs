using Android.App;
using Android.Widget;
using Android.OS;

namespace CaffeineTracker {
	[Activity(Label = "CaffeineTracker", MainLauncher = true)]
	public class MainActivity : Activity {
		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			var button = FindViewById<Button>(Resource.Id.myButton);
			button.Click += delegate {
				StartActivity(typeof(Activity1));
			};
		}
		 
	}
}

