using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Security;
using Camera = Android.Hardware.Camera;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;

namespace CaffeineTracker
{
	[Activity(Label = "Add an Image", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class Activity1 : Activity, TextureView.ISurfaceTextureListener, Camera.IShutterCallback, Camera.IPictureCallback
	{
		private Camera _camera;
		private TextureView _textureView;
		private bool manualFocus = false;
		private bool stopImaging;

		protected override void OnCreate(Bundle bundle)
		{
			RequestWindowFeature(WindowFeatures.NoTitle);
			base.OnCreate(bundle);
			Window.AddFlags(WindowManagerFlags.Fullscreen);
			SetContentView(Resource.Layout.Activity1);
			_textureView = FindViewById<TextureView>(Resource.Id.textureView1);
			_textureView.SurfaceTextureListener = this;

			var button = FindViewById<Button>(Resource.Id.backButton);
			button.Click += delegate
			{
				stopImaging = true;
				_camera.TakePicture(this, this, this);
			};

			_textureView.Click += delegate
			{
				_camera.CancelAutoFocus();
				Camera.Parameters param = _camera.GetParameters();
				if (!manualFocus && param.SupportedFocusModes.Contains(Camera.Parameters.FocusModeFixed))
				{
					param.FocusMode = Camera.Parameters.FocusModeFixed;
				}
				manualFocus = !manualFocus;
				_camera.SetParameters(param);
				Toast.MakeText(this, "Manual Focus " + (manualFocus ? "Enabled" : "Disabled"), ToastLength.Short).Show();
			};
		}

		public void OnSurfaceTextureAvailable(SurfaceTexture surface, int w, int h)
		{
			if (stopImaging) return;
			_camera = Camera.Open();
			Camera.Parameters param = _camera.GetParameters();
			if (!manualFocus && param.SupportedFocusModes.Contains(Camera.Parameters.FocusModeContinuousPicture))
			{
				param.FocusMode = Camera.Parameters.FocusModeContinuousPicture;
			}
			var dimensions = 480 * 768;
			var size = param.SupportedPictureSizes.Where(_ => _.Width * _.Height < dimensions).OrderByDescending(_ => _.Width * _.Height < dimensions).First();
			param.SetPictureSize(size.Width, size.Height);

			_camera.SetParameters(param);
			_camera.SetPreviewTexture(_textureView.SurfaceTexture);
			_camera.StartPreview();
			_textureView.Rotation = 90.0f;
			_textureView.ScaleX = (float)h / w;
			_textureView.ScaleY = (float)w / h;
		}

		public bool OnSurfaceTextureDestroyed(SurfaceTexture surface) => true;

		public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height) { }

		public void OnSurfaceTextureUpdated(SurfaceTexture surface) { }

		private async void ParseResponse(byte[] image)
		{
			var vw = new VisionWrapper(MainActivity.Key);
			var requests = vw.BuildRequest(vw.ToBase64(image));
			var responses = new[] { await vw.GetResponse(requests[0]), await vw.GetResponse(requests[1]) };
			var response = string.Join("\n", responses);
			var buzz = Regex.Matches(response, "\"description\": \"([A-Z a-z'\\(\\)0-9\\.\\-\\+]+)\"", RegexOptions.Singleline).Cast<Match>().Select(_ => _.Groups[1].Value).ToArray();
			var drinks = LoadDrinks();
			var matches = new List<Drink>();
			var i = 0;
			foreach (var b in buzz)
			{
				matches.AddRange(drinks.Where(_ => _.Name.ToLower().Contains(b.ToLower())));
			}
			d.Dismiss();
			var intent = new Intent(this, typeof(AddDrink));
			intent.PutExtra("data", matches.Distinct().Take(25).Select(_ => _.Name).ToArray());
			StartActivityForResult(intent, 1);
		}

		protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == Result.Canceled)
			{
				var ad = new AlertDialog.Builder(this);
				ad.SetMessage("The beverage you photographed could not be identified.\n\nTry another angle or better lighting.");
				ad.SetTitle("Unknown Beverage");
				ad.SetPositiveButton("Okay", delegate
				{
					SetResult(Result.Canceled, data);
					Finish();
				});
				ad.SetCancelable(false);
				ad.Create().Show();
				return;
			}
			SetResult(Result.Ok, data);
			Finish();
		}

		internal IEnumerable<Drink> LoadDrinks()
		{
			var reader = new StreamReader(Assets.Open("Drinks.csv"));
			var drinks = new List<Drink>();
			while (!reader.EndOfStream) yield return Drink.Deserialize(reader.ReadLine().Split('~'));
		}

		ProgressDialog d;
		public void OnShutter() { }
		public void OnPictureTaken(byte[] data, Camera camera)
		{
			if (data is null) { return; }
			else
			{
				d = new ProgressDialog(this);
				d.SetCancelable(false);
				d.SetMessage("Looking for caffinated beverages ...");
				d.Indeterminate = true;
				d.SetCanceledOnTouchOutside(false);
				d.SetProgressStyle(ProgressDialogStyle.Spinner);
				d.Create();
				d.Show();
				new Thread(() => ParseResponse(data)).Start();
				_camera.Release();
			}
		}
	}
}
