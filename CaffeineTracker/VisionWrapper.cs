using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace CaffeineTracker {
    class VisionWrapper {
        private string _apiKey;
        private WebClient _client;

        private const string Endpoint = "https://vision.googleapis.com/v1/images:annotate?key=";

        public VisionWrapper(string key) {
            _apiKey = key;
            _client = new WebClient();
        }

        public async Task<string> GetResponse(Image image) => await _client.UploadStringTaskAsync(Endpoint + _apiKey, BuildRequest(ToBase64(image)));

        private string BuildRequest(string base64) => "{\"requests\":[{\"image\":{\"content\":\"" + base64
                                                      + "\"},\"features\":[{\"type\":\"LOGO_DETECTION\"}]}]}";

        private string ToBase64(Image image) {
            var bytes = image.ToArray<byte>();
            return Convert.ToBase64String(bytes);
        }
    }
}
