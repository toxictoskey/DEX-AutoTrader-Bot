using TradingBot.Models;
using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;


namespace TradingBot
{
	public static class Helper
	{
		/// <summary>
		/// Takes date and returns timestamp.
		/// </summary>
		/// <returns>timestamp in milliseconds</returns>
		/// <param name="date">Current date.</param>
		/// Only works for adding timestamp to Url if timestamp entered is UTC
		/// Otherwise, recvWindow is required for other timestamps, less than UTC only!
		public static long DateToTimestamp(DateTime date)
		{
			var timeSt = (long)(date - new DateTime(1970, 1, 1)).TotalMilliseconds;
			return timeSt;
		}

		/// <summary>
		/// Gets the current time stamp for UTC
		/// </summary>
		/// <returns>Returns timestamp in milliseconds</returns>
		public static long getTimeStamp()
		{
			var timeSt = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
			return timeSt;
		}

		/// <summary>
		/// Test Connectivity to Binance Rest API and get current server time object
		/// </summary>
		/// <returns>Object of current time in milliseconds</returns>
		/// Should not be used for timestamp parameter in url, delay can be too long.
		public static ServerTime getServerTimeObject()
		{
			string apiRequestUrl = "https://www.binance.com/api/v1/time";

			string response = webRequest(apiRequestUrl, "GET", null);
			var serverTime = JsonConvert.DeserializeObject<ServerTime>(response);
			return serverTime;
		}

		/// <summary>
		/// Get current server time
		/// </summary>
		/// <returns>Long of current time in milliseconds</returns>
		public static long getServerTime()
		{
			var serverTime = getServerTimeObject();
			return serverTime.Time;
		}


		/// <summary>
		/// Converts bytes to string.
		/// </summary>
		/// <returns>String</returns>
		/// <param name="buff">Variable that holds bytes</param>
		public static string ByteToString(byte[] buff)   //from StackOverflow
		{
			string str = "";
			for (int i = 0; i < buff.Length; i++)
				str += buff[i].ToString("X2");
			return str;
		}


		public static string addReceiveWindow(string query)
		{
			query = $"{query}&recvWindow={5}";
			return query;
		}

		/// <summary>
		/// Create HttpWebRequest with Signature if needed
		/// </summary>
		/// <param name="url">Url to be requested. String.</param>
		/// <param name="method">Type of HTTP request. Eg; GET/POST. String.</param>
		/// <param name="ApiKey">Public Key to be added to header if signed. String.</param>
		/// <returns>HTTP Request Responce</returns>
		public static string webRequest(string requestUrl, string method, string ApiKey)
		{
			try
			{
				var request = (HttpWebRequest)WebRequest.Create(requestUrl);
				request.Method = method;
				request.Timeout = 5000;  //very long response time from Singapore. Change in Boston accordingly.
				if (ApiKey != null)
				{
					request.Headers.Add("X-MBX-APIKEY", ApiKey);
				}

				var webResponse = (HttpWebResponse)request.GetResponse();
				if (webResponse.StatusCode != HttpStatusCode.OK)
				{
					throw new Exception($"Did not return OK 200. Returned: {webResponse.StatusCode}");
				}

				var encoding = ASCIIEncoding.ASCII;
				string responseText = null;

				using (var reader = new System.IO.StreamReader(webResponse.GetResponseStream(), encoding))
				{
					responseText = reader.ReadToEnd();
				}

				return responseText;
			}
			catch (WebException webEx)
			{
				if (webEx.Response != null)
				{
					Encoding encoding = ASCIIEncoding.ASCII;
					using (var reader = new System.IO.StreamReader(webEx.Response.GetResponseStream(), encoding))
					{
						string responseText = reader.ReadToEnd();
						throw new Exception(responseText);
					}
				}
				throw;
			}
			catch
			{
				return "Error";
			}
		}

		/// <summary>
		/// Create Signature   
		/// </summary>
		/// <param name="SecretKey">Secret Key for HMAC signature. String.</param>
		/// <param name="query">Text to be signed. String.</param>
		/// <returns>Signature</returns>
		public static string getSignature(string SecretKey, string query)
		{
			Encoding encoding = Encoding.UTF8;
			var keyByte = encoding.GetBytes(SecretKey);
			using (var hmacsha256 = new HMACSHA256(keyByte))
			{
				hmacsha256.ComputeHash(encoding.GetBytes(query));
				return ByteToString(hmacsha256.Hash);
			}

		}

	}
}
