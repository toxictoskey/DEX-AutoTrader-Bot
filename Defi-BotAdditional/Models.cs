using System.Collections.Generic;

namespace TradingBot.Models
{
	public class ServerTime
	{
		public long Time { get; set; }
	}

	public class SymbolPrice
	{
		public string Symbol { get; set; }
		public double Price { get; set; }
	}

	public class SymbolTicker
	{
		public string Symbol { get; set; }
		public double BidPrice { get; set; }
		public double BidQty { get; set; }
		public double AskPrice { get; set; }
		public double AskQty { get; set; }
	}

	public class AccountInformation
	{
		public double MakerCommission { get; set; }
		public double TakerCommission { get; set; }
		public double BuyerCommission { get; set; }
		public double SellerCommission { get; set; }
		public bool CanTrade { get; set; }
		public bool CanWithdraw { get; set; }
		public bool CanDeposit { get; set; }
		public List<AssetBalance> Balances { get; set; }
	}

	public class AssetBalance
	{
		public string Asset { get; set; }
		public double Free { get; set; }
		public double Locked { get; set; }
	}

	public class Trades
	{
		public long Id { get; set; }
		public long OrderId { get; set; }
		public double Price { get; set; }
		public double Qty { get; set; }
		public string Commission { get; set; }
		public string commissionAsset { get; set; }
		public long Time { get; set; }
		public bool isBuyer { get; set; }
		public bool isMaker { get; set; }
		public bool isBestMatch { get; set; }
	}

	public class Order
	{
		public string Symbol { get; set; }
		public long OrderId { get; set; }
		public string ClientOrderid { get; set; }
		public double Price { get; set; }
		public double OrigQty { get; set; }
		public string ExecutedQty { get; set; }
		public OrderStatuses Status { get; set; }
		public TimesInForce TimeInForce { get; set; }
		public OrderTypes Type { get; set; }
		public OrderSides Side { get; set; }
		public double StopPrice { get; set; }
		public double IcebergQty { get; set; }
		public long Time { get; set; }
	}


	public enum OrderStatuses
	{
		NEW,
		PARTIALLY_FILLED,
		FILLED,
		CANCELED,
		PENDING_CANCEL,
		REJECTED,
		EXPIRED,
	}
	public enum OrderTypes
	{
		LIMIT,
		MARKET,
		STOP_LOSS,
		STOP_LOSS_LIMIT,
		TAKE_PROFIT,
		TAKE_PROFIT_LIMIT,
		LIMIT_MAKER
	}
	public enum OrderSides
	{
		BUY,
		SELL
	}
	public enum TimesInForce
	{
		GTC,
		IOC,
		FOK
	}
}
