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
using System.Text.RegularExpressions;

namespace CaffeineTracker
{
	class VisionWrapper
	{
		private string _apiKey;
		private WebClient _client;

		private const string Endpoint = "https://vision.googleapis.com/v1/images:annotate?key=";

		public VisionWrapper(string key)
		{
			_apiKey = key;
			_client = new WebClient();
		}

		public async Task<string> GetResponse(string request) => await _client.UploadStringTaskAsync(Endpoint + _apiKey, request);

		public string[] BuildRequest(string base64) => new[] {
			"{\"requests\":[{\"image\":{\"content\":\"" + base64 + "\"},\"features\":[{\"type\":\"LOGO_DETECTION\"}]}]}",
			"{\"requests\":[{\"image\":{\"content\":\"" + base64 + "\"},\"features\":[{\"type\":\"TEXT_DETECTION\"}]}]}"
		};

		public string ToBase64(byte[] image) => Convert.ToBase64String(image);
	}
}
