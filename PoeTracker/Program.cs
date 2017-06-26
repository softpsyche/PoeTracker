using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PoeTracker
{
	public class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				var driver = new ConsoleDriver();
				driver.Run();


			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				var secCount = 10;

				while (secCount > 0)
				{
					Console.WriteLine($"Unhandled error, shutting down in {secCount} seconds...");
					secCount--;
					Thread.Sleep(1000);
				}
			}
		}
	}


}
