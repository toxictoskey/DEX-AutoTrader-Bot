using TradingBot.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Timers; 
using System.ComponentModel;

namespace TradingBot
{
	public class API
	{
		/// <summary>
		/// The Bot takes a currency pair, checks previous trades for the pair
		/// Only the opposite of the last trade can be executed (maximising stock quantity technique)
		/// If the price has changed according to the desired percentage, a trade is executed. 
		/// </summary>

		// Do not change from here
		public static string baseUrl { get; set; } = "https://www.binance.com/api/";

		/// <summary>
		/// Test Connectivity to Binance Rest API
		/// </summary>
		/// <returns>Boolean value (True/False) based on success</returns>
		public static bool Ping()
		{
			string apiRequestUrl = $"{baseUrl}v1/ping";

			string response = Helper.webRequest(apiRequestUrl, "GET", null);
			if (response == "{}")
				return true;
			else
				return false;
		}

		/// <summary>
		/// Test Connectivity to Binance Rest API and get current server time
		/// </summary>
		/// <returns>String of current time in milliseconds</returns>
		public static string Time()
		{
			string apiRequestUrl = $"{baseUrl}v1/time";

			var response = Helper.webRequest(apiRequestUrl, "GET", null);
			return response;
		}


		/// <summary>
		/// Get prices for all symbols
		/// </summary>
		/// <returns>Current price for all symbols</returns>
		public static SymbolPrice[] GetAllPrices()
		{
			string apiRequestUrl = $"{baseUrl}v1/ticker/allPrices";

			var response = Helper.webRequest(apiRequestUrl, "GET", null);
			var parsedResponse = JsonConvert.DeserializeObject<SymbolPrice[]>(response);

			return parsedResponse;
		}


		/// <summary>
		/// Get the current price for the given symbol
		/// </summary>
		/// <param name="symbol">Asset symbol e.g.ETHBTC</param>
		/// <returns>The price. Double.</returns>
		public static double GetOnePrice(string symbol)
		{
			var symbolPrices = GetAllPrices();

			SymbolPrice symbolPrice = (from sym in symbolPrices where sym.Symbol == symbol select sym).FirstOrDefault();
			if (symbolPrice == null)
			{
				throw new ApplicationException($"No symbol, {symbol}, exists");
			}

			return symbolPrice.Price;
		}


		/// <summary>
		/// Get AccountInformation including all Asset Balances
		/// </summary>
		/// <param name="recvWindow">Interval (in milliseconds) in which the request must be processed within a certain number of milliseconds or be rejected by the server. Defaults to 5000 milliseconds</param>
		/// <returns>AccountInformation object including current asset balances</returns>
		public static AccountInformation GetAccountInformation()
		{
			string apiRequestUrl = $"{baseUrl}v3/account";

			string query = $"";
			query = $"{query}&timestamp={Helper.getTimeStamp()}";

			var signature = Helper.getSignature(Globals.SecretKey, query);
			query += "&signature=" + signature;

			apiRequestUrl += "?" + query;

			var response = Helper.webRequest(apiRequestUrl, "GET", Globals.ApiKey);

			var parsedResponse = JsonConvert.DeserializeObject<AccountInformation>(response);
			return parsedResponse;
		}

		/// <summary>
		/// Get your Open Orders for the given symbol
		/// </summary>
		/// <param name="symbol">Asset symbol e.g.ETHBTC</param>
		/// <returns>A list of Order objects with the order data</returns>
		public static Order[] GetOpenOrders(string symbol)
		{
			string apiRequestUrl = $"{baseUrl}v3/openOrders";

			string query = $"symbol={symbol}";

			query = $"{query}&timestamp={Helper.getTimeStamp()}";

			var signature = Helper.getSignature(Globals.SecretKey, query);

			query += "&signature=" + signature;

			apiRequestUrl += "?" + query;

			var response = Helper.webRequest(apiRequestUrl, "GET", Globals.ApiKey);
			var parsedResponse = JsonConvert.DeserializeObject<Order[]>(response);
			return parsedResponse;
		}

		/// <summary>
		/// Get trades for a specific account and symbol
		/// </summary>
		/// <param name="symbol">Asset symbol e.g.ETHBTC</param>
		/// <returns>A list of Trades</returns>
		public static Trades[] GetMyTrades(string symbol)
		{
			string apiRequestUrl = $"{baseUrl}v3/myTrades";

			string query = $"symbol={symbol}";

			query = $"{query}&timestamp={Helper.getTimeStamp()}";

			var signature = Helper.getSignature(Globals.SecretKey, query);
			query += "&signature=" + signature;

			apiRequestUrl += "?" + query;

			var response = Helper.webRequest(apiRequestUrl, "GET", Globals.ApiKey);
			var parsedResponse = JsonConvert.DeserializeObject<Trades[]>(response);
			return parsedResponse;
		}

		/// <summary>
		/// Gets the last trade.
		/// </summary>
		/// <returns>The last trade. Trades object</returns>
		/// <param name="symbol">Symbol.</param>
		public static Trades GetLastTrade(string symbol)
		{
			var parsedResponse = GetMyTrades(symbol);

			if (parsedResponse.Length != 0)
				return parsedResponse[parsedResponse.Length - 1];
			else
				return null;
		}

		/// <summary>
		/// Places an order
		/// </summary>
		/// <returns>The order object</returns>
		/// <param name="symbol">Symbol of currencies to be traded, eg BCCETH</param>
		/// <param name="side">Order Side, BUY or SELL</param>
		/// <param name="type">Order Type, see Set.OrderTypes </param>
		/// <param name="timeInForce">Time order will be active for.</param>
		/// <param name="quantity">Amount to be traded</param>
		/// <param name="price">Price to be bought at.</param>

		public static Order PlaceOrder(string symbol, OrderSides side, OrderTypes type, TimesInForce timeInForce, double quantity, double price)
		{
			string apiRequestUrl = "";

			if (Globals.testCase == true)
				apiRequestUrl = $"{baseUrl}v3/order/test";
			else
				apiRequestUrl = $"{baseUrl}v3/order";


			string query = $"symbol={symbol}&side={side}&type={type}&timeInForce={timeInForce}&quantity={quantity}&price={price}";

			query = $"{query}&timestamp={Helper.getTimeStamp()}";

			var signature = Helper.getSignature(Globals.SecretKey, query);
			query += "&signature=" + signature;

			apiRequestUrl += "?" + query;
			var response = Helper.webRequest(apiRequestUrl, "POST", Globals.ApiKey);

			var parsedResponse = JsonConvert.DeserializeObject<Order>(response);
			return parsedResponse;
		}

		/// <summary>
		/// Places a market order. (Order at the current market price, needs no price or timeInForce params).
		/// </summary>
		/// <returns>The order object</returns>
		/// <param name="symbol">Symbol of currencies to be traded, eg BCCETH.</param>
		/// <param name="side">Order Side, BUY or SELL.</param>
		/// <param name="quantity">Amount to be traded.</param>

		public static Order PlaceMarketOrder(string symbol, OrderSides side, double quantity)
		{
			string apiRequestUrl = ""; 

			if (Globals.testCase == true)
				apiRequestUrl = $"{baseUrl}v3/order/test";
			else
				apiRequestUrl = $"{baseUrl}v3/order";

			string query = $"symbol={symbol}&side={side}&type={OrderTypes.MARKET}&quantity={quantity}";
			query = $"{query}&timestamp={Helper.getTimeStamp()}";

			var signature = Helper.getSignature(Globals.SecretKey, query);
			query += "&signature=" + signature;

			apiRequestUrl += "?" + query;
			var response = Helper.webRequest(apiRequestUrl, "POST", Globals.ApiKey);

			var parsedResponse = JsonConvert.DeserializeObject<Order>(response);
			return parsedResponse;
		}


		/***********

		//Ping
		var pingTest = Ping();

		//Time
		var timeTest = Time();

		///Get your account Info
		//Returned as object, see Set.cs to parse through it
		var accountInfo = GetAccountInformation();

		///Get prices of all available currency pairs
		//Returned as list of objects, see Set.cs or GetOnePrice() function to parse through it
		var allPrices = GetAllPrices();

		///Get prices of a specific currency pair/symbol
		var onePrice = GetOnePrice("BCCETH");

		///Get all open orders on your account
		var openOrders = GetOpenOrders("BCCETH");

		///Get your trade history related to a specific currency pair/symbol
		//Returned as list of objects, see Set.cs or GetLastTrade() function to parse through it
		var trades = GetMyTrades("XRPETH");

		///Get your last trade of a specific currency pair/symbol
		//Returned as object, see Set.cs to parse through it
		var lastTrade = GetLastTrade("XRPETH");

		///Place any kind of acccepted order, set to "test" phase as not in use.
		//var order = PlaceOrder("BCCETH", OrderSides.SELL, OrderTypes.MARKET, TimesInForce.GTC, 0.01, 2.09);

		///Place a Market order
		var marketOrder = PlaceMarketOrder("BCCETH", OrderSides.SELL, 0.01);

		**********/

	}
}
