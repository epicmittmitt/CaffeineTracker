using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Security;
using Camera = Android.Hardware.Camera;

namespace CaffeineTracker
{
    [Activity(Label = "Activity1")]
    public class Activity1 : Activity,TextureView.ISurfaceTextureListener
    {
        private Camera _camera;
        private TextureView _textureView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _textureView = new TextureView(this);
            _textureView.SurfaceTextureListener = this;

            SetContentView(_textureView);
        }

        public void OnSurfaceTextureAvailable(Android.Graphics.SurfaceTexture surface, int w, int h)
        {
            _camera = Camera.Open();
            
            _textureView.LayoutParameters = new FrameLayout.LayoutParams(h, w);

            try
            {
                _camera.SetPreviewTexture(surface);
                _camera.StartPreview();
            }
            catch (Java.IO.IOException ex)
            {
                Console.WriteLine(ex.Message);
                
            }

            _textureView.Rotation = 90.0f;
            _textureView.TranslationX = -300f;

        }

        public bool OnSurfaceTextureDestroyed(Android.Graphics.SurfaceTexture surface)
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