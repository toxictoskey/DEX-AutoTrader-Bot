
using TradingBot.Models;
using System;
using System.Linq;



namespace TradingBot
{
	public class Algorithm
	{


		public static void Bot()
		{
			string symbol = Globals.C1 + Globals.C2;

			//GetAccountInformation
			var accInfo = API.GetAccountInformation();

			//Check account funds for both currencies
			AssetBalance Currency1 = (from coin in accInfo.Balances where coin.Asset == Globals.C1 select coin).FirstOrDefault();
			var freeCurrency1 = Currency1.Free;

			AssetBalance Currency2 = (from coin in accInfo.Balances where coin.Asset == Globals.C2 select coin).FirstOrDefault();
			var freeCurrency2 = Currency2.Free;

			//Get Price of last BCCBTC trade
			var lastTrade = API.GetLastTrade(symbol);
			var lastTradePrice = 0.0;
			var lastTradeSide = OrderSides.SELL; //if lastTrade == null, this means buy Currency1


			//get price and OrderSide of last trade (if any)
			if (lastTrade != null)
			{
				lastTradePrice = lastTrade.Price;

				if (lastTrade.isBuyer == true)
					lastTradeSide = OrderSides.BUY;
				else
					lastTradeSide = OrderSides.SELL;
			}

			//Check current price
			var currentPrice = API.GetOnePrice(symbol);

			//Calculate actual price change percentage
			var priceChange = 100 * (currentPrice - lastTradePrice) / currentPrice;

			Console.WriteLine("Current Price is " + currentPrice);
			Console.WriteLine("Price Change is " + priceChange);

			//Create Order
			Order marketOrder = null;

			if (lastTradeSide == OrderSides.BUY && priceChange > Globals.percentageChange)
			{
				//if last order was buy, and price has increased
				//sell C1
				marketOrder = API.PlaceMarketOrder(symbol, OrderSides.SELL, Globals.quatityPerTrade);

			}
			else if (lastTradeSide == OrderSides.SELL && priceChange < -Globals.percentageChange)
			{
				//if last order was sell, and price has decreased
				//buy C1
				marketOrder = API.PlaceMarketOrder(symbol, OrderSides.BUY, Globals.quatityPerTrade);
			}


			//Statements
			if (marketOrder == null)
			{
				Console.WriteLine("No trade was made.");
				var actualLastTrade = API.GetLastTrade(symbol);
				if (actualLastTrade.isBuyer == true)
				{
					Console.WriteLine(actualLastTrade.Qty + " of " + Globals.C1 + " was previously bought for " + actualLastTrade.Price);
				}
				else if (actualLastTrade.isBuyer == false)
				{
					Console.WriteLine(actualLastTrade.Qty + " of " + Globals.C1 + " was previously sold for " + actualLastTrade.Price);
				}
			}
			else
			{
				var newLastTrade = API.GetLastTrade(symbol);
				if (marketOrder.Side == OrderSides.BUY)
				{
					Console.WriteLine(newLastTrade.Qty + " of " + Globals.C1 + " was bought for " + newLastTrade.Price);
				}
				else if (marketOrder.Side == OrderSides.SELL)
				{
					Console.WriteLine(newLastTrade.Qty + " of " + Globals.C1 + " was sold for " + newLastTrade.Price);
				}
			}

		}
	}
}