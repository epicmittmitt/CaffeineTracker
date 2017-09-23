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
	public class Activity1 : Activity, TextureView.ISurfaceTextureListener
	{
		private Camera _camera;
		private TextureView _textureView;

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
				
			};
		}

		public void OnSurfaceTextureAvailable(SurfaceTexture surface, int w, int h)
		{
			_camera = Camera.Open();
			Camera.Parameters param = _camera.GetParameters();
			if (param.SupportedFocusModes.Contains(Camera.Parameters.FocusModeContinuousVideo))
			{
				param.FocusMode = Camera.Parameters.FocusModeContinuousVideo;
			}
			
			_camera.SetParameters(param);
			_camera.SetPreviewTexture(_textureView.SurfaceTexture);
			_camera.StartPreview();
			_textureView.Rotation = 90.0f;
			_textureView.ScaleX = (float)h / w;
			_textureView.ScaleY = (float)w / h;
		}

		public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
		{
			_camera.StopPreview();
			_camera.Release();

			return true;

		}

		public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
		{

		}

		public void OnSurfaceTextureUpdated(SurfaceTexture surface)
		{

		}
	}
}