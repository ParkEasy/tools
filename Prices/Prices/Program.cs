using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Prices
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			String pricesRaw = File.ReadAllText("prices.json");
			List<String> prices = JsonConvert.DeserializeObject<List<String>> (pricesRaw);

			List<PriceModel> models = new List<PriceModel> ();
			foreach (String price in prices) 
			{
				PriceModel model = PriceParser.Parse (price);
				if (model != null)
					models.Add (model);
			}

			Console.WriteLine ("{0} of {1} parsed.", models.Count, prices.Count);
		}
	}
}
