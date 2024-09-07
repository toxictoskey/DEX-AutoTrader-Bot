import base64
import time
import hmac
import hashlib


def get_timestamp():
    return int(time.time() * 1000)


def sign_request(api_secret, request_path, body=''):
    message = request_path + str(get_timestamp()) + body
    hmac_key = base64.b64decode(api_secret)
    signature = hmac.new(hmac_key, message.encode('utf-8'), hashlib.sha256)
    return base64.b64encode(signature.digest())


def format_currency(amount):
    return "{:,.2f}".format(amount)


def log(message):
    print(f"[{time.strftime('%Y-%m-%d %H:%M:%S')}] {message}")
