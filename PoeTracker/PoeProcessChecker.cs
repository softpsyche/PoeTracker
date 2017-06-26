using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;

namespace PoeTracker
{
	internal class PoeProcessChecker
	{
		private Settings Settings { get; set; }

		public PoeProcessChecker()
		{
			Settings = new Settings();
		}

		public bool IsPathOfExileExeRunning()
		{
			var poeFound = false;
			var runningProcessNames = Process.GetProcesses().Select(a => a.ProcessName).ToList();
			var acceptedProcessNames = Settings.PoeAcceptedProcessNamesCsv.ToList();

			poeFound = acceptedProcessNames.Any(a => runningProcessNames.Any(b => b.Equals(a, StringComparison.InvariantCultureIgnoreCase)));

			return poeFound;
		}
	}
}
