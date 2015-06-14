using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Prices
{
	public static class PriceParser
	{
		// GETHOURANDDAYPRICE
		// extracts a format like "5,00 € / 15,00 €"
		private static PriceModel GetHourAndDayPrice(string input)
		{
			PriceModel model = new PriceModel();

			// split in half
			string[] halfs = input.Split ('/');

			string pricePerHourStr = halfs [0]
				.Replace ("€", "")
				.Replace ("EUR", "")
				.Replace (",", ".")
				.Trim ();

			string pricePerDayStr = halfs [1]
				.Replace ("€", "")
				.Replace ("EUR", "")
				.Replace (",", ".")
				.Trim ();

			// check for format "2,50 € / Tag"
			if (halfs [1].ToLower ().Contains ("tag")) 
			{
				model.FullDay = Convert.ToDouble (pricePerHourStr);
			} 
			else 
			{
				model.PerHour = new PerHourModel (1, Convert.ToDouble (pricePerHourStr));
				model.FullDay = Convert.ToDouble (pricePerDayStr);
			}

			return model;
		}

		// STRIPALLALPHAS
		// removes all alpha characters from a string
		private static double StripAllAlphas(string input)
		{
			string value = Regex.Replace(input.Replace(",", "."), "[A-Za-züöäÖÄÜß€/]", "").Trim();


			return Convert.ToDouble (value);
		}

		// PARSE
		// parses a whole set of price inputs
		public static PriceModel Parse(string input)
		{
			PriceModel model = new PriceModel ();

			if (string.IsNullOrEmpty (input) || 
				string.IsNullOrWhiteSpace (input) || 
				input.Length <= 1) 
			{
				return null;
			}

			// check for "5,00€" or "1,50€ pro Stunde"
			if (input.Count (f => f == '€') == 1 && !input.Contains ("/") && input.Length < 20) 
			{
				model.PerHour = new PerHourModel (1, StripAllAlphas(input));
		
				return model;
			}

			// check for format "1,70 € / 15,00 €"
			if (input.Contains ("/") && input.Length < 20) 
			{
				return GetHourAndDayPrice (input);
			}

			// check for tiered and special hours
			else 
			{
				// check for "2€ für Nichtkunden; 1€ für Kunden"
				if (input.Contains (";")) 
				{
					string[] semihalfs = input.Split (';');
					foreach (string half in semihalfs) 
					{
						if (half.IndexOf ("€") >= 0) 
						{
							model.PerHour = new PerHourModel (1, StripAllAlphas(half));
							return model;
						}
					}

				}

				// is this a multi liner?
				if (input.Contains("\n")) 
				{
					string[] lines = input.Split ('\n');

					// check for "5 € / 15 €" in first line
					if (lines.First ().Contains ("/") && 
						lines.First ().Count (f => f == '€') == 2 && 
						lines.First ().Length < 20) 
					{
						return GetHourAndDayPrice (lines.First());
					}

				} 
				else 
				{
					// check for "je stunde zahlen sie 1,00 Euro das Tagesmaximum liegt bei 20 Euro"
					if(input.Contains("Stunde") && input.Contains("Tages"))
					{
						int idx;
						if (input.Contains (", ")) 
						{
							idx = input.IndexOf (", ");
						} 
						else 
						{
							idx = input.IndexOf ("Tages");
						}

						string hour = input.Substring (0, idx).Replace(", ", "");
						string day = input.Substring (idx, input.Length - idx).Replace(", ", "");

						model.PerHour = new PerHourModel (1, StripAllAlphas (hour));
						model.FullDay = StripAllAlphas (day);
						return model;
					}
				}
			}

			return null;
		}
	}
}

