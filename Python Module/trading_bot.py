import time
from .strategies import MeanReversionStrategy
from ..api.exchange_api import ExchangeAPI

class TradingBot:
    def __init__(self, settings):
        self.settings = settings
        self.strategy = MeanReversionStrategy(settings)
        self.wallets = {}
        self.api = ExchangeAPI(settings["api_key"], settings["api_secret"])

    def add_wallet(self, currency, amount):
        self.wallets[currency] = amount

    def update_wallets(self):
        balances = self.api.get_balance()
        for currency, amount in balances.items():
            self.wallets[currency] = amount

    def place_order(self, pair, side, price, quantity):
        order = self.api.place_order(pair, side, price, quantity)
        print(f"Placed {side} order for {quantity} {pair} at {price}")
        return order

    def cancel_order(self, order_id):
        self.api.cancel_order(order_id)
        print(f"Canceled order {order_id}")

    def start(self):
        print("Starting trading bot...")
        while True:
            self.update_wallets()
            for pair in self.settings["trading_pairs"]:
                base_currency, quote_currency = pair.split("/")
                if base_currency in self.wallets and self.wallets[base_currency] > 0:
                    action = self.strategy.decide(pair, self.wallets[base_currency])
                    if action == "sell":
                        price = self.api.get_price(pair)
                        self.place_order(pair, "sell", price, self.wallets[base_currency])
                elif quote_currency in self.wallets and self.wallets[quote_currency] > 0:
                    action = self.strategy.decide(pair, self.wallets[quote_currency])
                    if action == "buy":
                        price = self.api.get_price(pair)
                        self.place_order(pair, "buy", price, self.wallets[quote_currency])
            time.sleep(60)
