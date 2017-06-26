using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeTracker
{
	public class Settings
	{
		public bool CheckForPoeRunningProcess => ReadAppSetting<bool>(nameof(CheckForPoeRunningProcess));
		public int CheckForPoeRunningProcessIntervalInSeconds => ReadAppSetting<int>(nameof(CheckForPoeRunningProcessIntervalInSeconds));
		public string[] PoeAcceptedProcessNamesCsv => ReadAppSetting(nameof(PoeAcceptedProcessNamesCsv)).Split(",".ToCharArray());

		private string ReadAppSetting(string settingName)
		{
			return ReadAppSetting<string>(settingName);
		}
		private T ReadAppSetting<T>(string settingName)
		{
			var setting = ConfigurationManager.AppSettings[settingName];
			return (T)Convert.ChangeType(setting, typeof(T));
		}
	}
}
