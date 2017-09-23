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

		public async Task<string[]> GetResponse(byte[] image)
		{
			var responses = new string[2];
			var reqs = BuildRequest(ToBase64(image));
			for (var i = 0; i < responses.Length; i++)
			{
				responses[i] = await _client.UploadStringTaskAsync(Endpoint + _apiKey, reqs[i]);
			}
			return responses;
		}

		private string[] BuildRequest(string base64)
		{
			var req = new[] {
				"{\"requests\":[{\"image\":{\"content\":\"" + base64 + "\"},\"features\":[{\"type\":\"LOGO_DETECTION\"}]}]}",
				"{\"requests\":[{\"image\":{\"content\":\"" + base64 + "\"},\"features\":[{\"type\":\"TEXT_DETECTION\"}]}]}"
			};
			return req;
		}

		private string ToBase64(Image image)
		{
			var bytes = image.ToArray<byte>();
			return Convert.ToBase64String(bytes);
		}
		private string ToBase64(byte[] image)
		{
			return Convert.ToBase64String(image);
		}
	}
}
