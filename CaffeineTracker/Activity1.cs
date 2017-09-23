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

namespace CaffeineTracker
{
	[Activity(Label = "Activity1", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
	public class Activity1 : Activity, TextureView.ISurfaceTextureListener, Camera.IShutterCallback, Camera.IPictureCallback
	{
		private Camera _camera;
		private TextureView _textureView;
		//private bool readyToDispose = false;
		private bool stopImaging;

		protected override void OnCreate(Bundle bundle)
		{
			RequestWindowFeature(WindowFeatures.NoTitle);
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.Activity1);
			_textureView = FindViewById<TextureView>(Resource.Id.textureView1);
			_textureView.SurfaceTextureListener = this;

			var button = FindViewById<Button>(Resource.Id.backButton);
			button.Click += delegate
			{
				stopImaging = true;
				_camera.TakePicture(this, this, this);
			};
		}

		public void OnSurfaceTextureAvailable(SurfaceTexture surface, int w, int h)
		{
			if (stopImaging) return;
			_camera = Camera.Open();
			Camera.Parameters param = _camera.GetParameters();
			if (param.SupportedFocusModes.Contains(Camera.Parameters.FocusModeContinuousVideo))
			{
				param.FocusMode = Camera.Parameters.FocusModeContinuousVideo;
			}
			var dimensions = 480 * 720;
			var size = param.SupportedPictureSizes.Where(_ => _.Width * _.Height < dimensions).OrderByDescending(_ => _.Width * _.Height < dimensions).First();
			param.SetPictureSize(size.Width, size.Height);

			_camera.SetParameters(param);
			_camera.SetPreviewTexture(_textureView.SurfaceTexture);
			_camera.StartPreview();
			_textureView.Rotation = 90.0f;
			_textureView.ScaleX = (float)h / w;
			_textureView.ScaleY = (float)w / h;
		}

		public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
		{
			_camera.Release();
			return true;
		}

		public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
		{

		}

		public void OnSurfaceTextureUpdated(SurfaceTexture surface)
		{

		}

		public void OnShutter() { }
		public void OnPictureTaken(byte[] data, Camera camera)
		{
			if (data is null)
			{
				return;
			}
			else
			{
				var intent = new Intent(this, typeof(MainActivity));
				intent.PutExtra("image", data);
				SetResult(Result.Ok, intent);
				Finish();
			}
		}
	}
}