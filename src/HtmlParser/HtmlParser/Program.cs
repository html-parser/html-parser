using System;
using System.IO;

namespace HtmlParser
{
	class Program
	{		
		static string InputFileName = "input.html";
		
		public static void Main(string[] args)
		{
			string htmlText = File.ReadAllText(Environment.CurrentDirectory + Path.DirectorySeparatorChar + InputFileName);
			Console.WriteLine("Input Html text:");
			Console.WriteLine();
			Console.WriteLine(htmlText);
			
			HtmlPage html = null;
			try
			{
				html = Serialize.FromHtml(htmlText);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Following error occured while parsing input HTML:");
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}
			
			if (html != null)
			{
				Console.WriteLine("HTML was parsed successfully");
			}
			else
			{
				Console.WriteLine("Failed to parse HTML");
			}
			
			Console.WriteLine();
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}