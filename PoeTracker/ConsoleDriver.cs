using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PoeTracker
{
	internal class ConsoleDriver
	{
		private PoeTradeClient Client { get; set; }
		private PoeProcessChecker ProcessChecker { get; set; }
		private Settings Settings { get; set; }

		public ConsoleDriver()
		{
			Client = new PoeTradeClient();
			ProcessChecker = new PoeProcessChecker();
			Settings = new Settings();
		}

		public void Run()
		{
			var settings = TryLoadSettings();
			if (settings == null) throw new Exception("No settings provided/found");

			Console.WriteLine($"Settings found: '{settings.BaseUrl}{settings.ControlId}'");
			Console.WriteLine($"(if these settings are wrong, delete the file '{PoeControlUrlFilePath()}' and restart this application)");
			Client.Settings = settings;

			Console.WriteLine();

			while (true)
			{
				if (Settings.CheckForPoeRunningProcess)
				{
					Console.WriteLine("Checking Poe process status...");
					var poeRunning = ProcessChecker.IsPathOfExileExeRunning();

					if (poeRunning == false)
					{
						var secondsToSleep = Settings.CheckForPoeRunningProcessIntervalInSeconds;
						Console.WriteLine($"Poe process is not running. Going to sleep and checking in again in {secondsToSleep} seconds.");
						Thread.Sleep(secondsToSleep * 1000);
						continue;
					}
					else
					{
						Console.WriteLine("Poe process is running!");
					}
					Console.WriteLine();
				}

				Console.WriteLine("Checking Poe Trade status...");
				var status = Client.CheckStatus();


				if ((status.IsOnline == false) || (status.OnlineSecondsRemaining <= 5))
				{
					Console.WriteLine("Status is OFFLINE or about to expire...");
					Console.WriteLine();

					Console.WriteLine("Prolongating...");
					status = Client.Prolongate();
					Console.WriteLine($"Status prolongated for {status.OnlineSecondsRemaining} seconds");

					Console.WriteLine();
				}
				else
				{
					Console.WriteLine("Status is ONLINE");
					Console.WriteLine();
				}

				if (status.OnlineSecondsRemaining > 0)
				{
					var timeToSleep = Convert.ToInt32((status.OnlineSecondsRemaining - 5) * 1000);
					Console.WriteLine($"Sleeping for {timeToSleep} seconds until next prolongation...");
					Console.WriteLine();
					Thread.Sleep(timeToSleep);
				}
			}
		}

		private PoeTradeSettings TryLoadSettings()
		{
			Console.WriteLine($"Looking for settings in path '{PoeControlUrlFilePath()}' ...");
			var settings = TryFindPoeTradeSettings();

			if (settings == null)
			{
				Console.WriteLine(@"Please enter your poe trade id (i.e. http://control.poe.xyz.is/YOURID)");
				var tradeId = Console.ReadLine();

				if (tradeId != null)
				{
					settings = new PoeTradeSettings()
					{
						BaseUrl = @"http://control.poe.xyz.is/",
						ControlId = tradeId
					};
					SavePoeTradeSettings(settings);
				}
			}

			return settings;
		}

		public string PoeControlUrlFilePath() => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"PoeTradeSettings.json");
		private PoeTradeSettings TryFindPoeTradeSettings()
		{
			if (File.Exists(PoeControlUrlFilePath()))
				return JsonConvert.DeserializeObject<PoeTradeSettings>(File.ReadAllText(PoeControlUrlFilePath()));

			return null;
		}
		private void SavePoeTradeSettings(PoeTradeSettings tradeId)
		{
			File.WriteAllText(PoeControlUrlFilePath(), JsonConvert.SerializeObject(tradeId, Formatting.Indented));
		}
	}
}
