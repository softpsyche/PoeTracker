using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace PoeTracker
{
	internal class PoeTradeClient
	{
		private static HttpClient HttpClient = new HttpClient();
		public PoeTradeSettings Settings { get; set; }
		private static readonly string OnlineStringIndicator = "you are online for the next";
		private string UserUrl => $"{Settings.BaseUrl}{Settings.ControlId}";

		public PoeTradeStatus CheckStatus()
		{
			
			using (var response = HttpClient.GetAsync(UserUrl).Result)
			{
				return ReadPoeTradeStatus(response);
			}
		}		
		public PoeTradeStatus Prolongate()
		{
			using (var response = HttpClient.PostAsync(UserUrl, null).Result)
			{
				return ReadPoeTradeStatus(response);
			}
		}

		private PoeTradeStatus ReadPoeTradeStatus(HttpResponseMessage response)
		{
			response.EnsureSuccessStatusCode();

			var status = new PoeTradeStatus();
			var stringContent = response.Content.ReadAsStringAsync().Result.ToLowerInvariant();

			status.IsOnline = IsOnline(stringContent);
			if (status.IsOnline) status.OnlineSecondsRemaining = GetOnlineSecondsRemaining(stringContent);

			return status;
		}

		private bool IsOnline(string htmlResponse)
		{
			return htmlResponse.Contains(OnlineStringIndicator);
		}
		private double GetOnlineSecondsRemaining(string htmlResponse)
		{
			var startIndex = htmlResponse.IndexOf(OnlineStringIndicator);
			var endIndex = htmlResponse.IndexOf("seconds.");

			var segment = htmlResponse.Substring(startIndex, endIndex - startIndex);
			var secondsPart = segment.Replace(OnlineStringIndicator, "").Trim();
			return Convert.ToDouble(secondsPart);
		}

		
	}
}
