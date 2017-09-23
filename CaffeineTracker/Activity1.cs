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
	public class Activity1 : Activity, TextureView.ISurfaceTextureListener, Camera.IAutoFocusCallback
	{
		private Camera _camera;
		private TextureView _textureView;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			_textureView = new TextureView(this)
			{
				SurfaceTextureListener = this
			};

			SetContentView(_textureView);
		}

		public void OnSurfaceTextureAvailable(SurfaceTexture surface, int w, int h)
		{
			_camera = Camera.Open();
			_camera.AutoFocus(this);
			_textureView.LayoutParameters = new FrameLayout.LayoutParams(w, w);
			_camera.SetPreviewTexture(surface);
			_camera.StartPreview();
			_textureView.Rotation = 90.0f;
			_textureView.ScaleX = (float)h / w;
		}

		public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
		{
			_camera.CancelAutoFocus();
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

		void Camera.IAutoFocusCallback.OnAutoFocus(bool success, Camera camera) { }
	}
}