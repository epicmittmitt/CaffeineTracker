using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Runtime;
using Android.Graphics;
using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Environment = Android.OS.Environment;
using Android.Views;
using System.Threading.Tasks;
using System.Timers;
using Android.Bluetooth;

namespace CaffeineTracker {
    [Activity(Label = "Caffeine Tracker", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity {
        public static string Key => Encoding.ASCII.GetString(Convert.FromBase64String("QUl6YVN5QXJ2WUk3cjh1SHhwMTh3enlkeU4wX1YyMHI3TEpTR0FJ"));
		public static string HistoryPath => /*System.IO.Path.Combine(System.Environment.CurrentDirectory, "/sdcard/Documents/history.csv"*/ System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "history.csv");


		protected override void OnCreate(Bundle savedInstanceState) {
            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.AddFlags(WindowManagerFlags.ForceNotFullscreen);
            SetContentView(Resource.Layout.Main);
            base.OnCreate(savedInstanceState);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar1);
            SetActionBar(toolbar);
            ActionBar.Title = "Caffeine Tracker";

			lv = FindViewById<ListView>(Resource.Id.listView4);
			UpdateListView();

			timer = new Timer { AutoReset = true, Interval = 10000 };
			timer.Elapsed += (s, e) => UpdateListView();
			StartTimer();
        }

        private Timer timer;

        protected override void OnDestroy() {
            timer.Stop();
            timer.Dispose();
            base.OnDestroy();
        }

        protected override void OnPause() {
            timer.Stop();
            base.OnPause();
        }

        protected override void OnRestart() {
            StartTimer();
            base.OnRestart();
        }

        protected override void OnResume() {
            StartTimer();
            base.OnResume();
        }

        private void StartTimer() => timer.Start();

        public override bool OnCreateOptionsMenu(IMenu menu) {
            MenuInflater.Inflate(Resource.Menu.Menu1, menu);
            var openCamera = menu.FindItem(Resource.Id.openCamera);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item) {
            if (item.TitleFormatted.ToString() == "Add an Image") {
                StartActivityForResult(typeof(Activity1), 0);
            } else {
				File.Delete(HistoryPath);
            }
            return base.OnOptionsItemSelected(item);
        }

		ListView lv;

        private void UpdateListView() {
			var drinks = Read().ToArray();
            lv.Adapter = new HSLV(this, drinks);
			//lv.ItemClick += (s, e) => { };
			//lv.ItemLongClick += (s, e) => { };
		}

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data) {
                if (resultCode == Result.Ok) {
					var _d = Drink.Deserialize(data.GetStringArrayExtra("data"));
                    var d = DetailedDrink.ToDetailedDrink(_d);
                    Write(new[] { d });
                    UpdateListView();
				}
            base.OnActivityResult(requestCode, resultCode, data);
        }

        internal IEnumerable<DetailedDrink> Read() {
			if (!File.Exists(HistoryPath)) File.WriteAllText(HistoryPath, string.Empty);
			var file = File.OpenRead(HistoryPath);
			var raw = new StreamReader(file);
			while (!raw.EndOfStream) yield return DetailedDrink.Deserialize(raw.ReadLine().Split('~'));
			raw.Close();
			raw.Dispose();
			file.Flush();
			file.Close();
			file.Dispose();
		}

        internal void Write(DetailedDrink[] drinks) {
            var _output = Read().ToList();
            _output.AddRange(drinks);
			var file = File.OpenWrite(HistoryPath);
			var raw = new StreamWriter(file);
			foreach (var output in _output) raw.WriteLine(string.Join("~", DetailedDrink.Serialize(output)));
			raw.Flush();
			raw.Close();
			raw.Dispose();
			file.Close();
			file.Dispose();
		}

        public static int GetDamerauLevenshteinDistance(string s, string t) {
            var bounds = new { Height = s.Length + 1, Width = t.Length + 1 };
            var matrix = new int[bounds.Height, bounds.Width];
            for (var height = 0; height < bounds.Height; height++) {
                matrix[height, 0] = height;
            }
            for (var width = 0; width < bounds.Width; width++) {
                matrix[0, width] = width;
            }
            for (var height = 1; height < bounds.Height; height++) {
                for (var width = 1; width < bounds.Width; width++) {
                    var cost = (s[height - 1] == t[width - 1]) ? 0 : 1;
                    var insertion = matrix[height, width - 1] + 1;
                    var deletion = matrix[height - 1, width] + 1;
                    var substitution = matrix[height - 1, width - 1] + cost;
                    var distance = Math.Min(insertion, Math.Min(deletion, substitution));
                    if (height > 1 && width > 1 && s[height - 1] == t[width - 2] && s[height - 2] == t[width - 1]) {
                        distance = Math.Min(distance, matrix[height - 2, width - 2] + cost);
                    }
                    matrix[height, width] = distance;
                }
            }
            return matrix[bounds.Height - 1, bounds.Width - 1];
        }
    }
}
