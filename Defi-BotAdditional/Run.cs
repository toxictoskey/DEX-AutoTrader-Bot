using TradingBot.Models;
using System;
using System.Timers;



namespace TradingBot
{
	public class Run
	{
		static System.Threading.ManualResetEvent timerFired = new System.Threading.ManualResetEvent(false);

		public static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			Console.WriteLine("Timer Fired.");
			Algorithm.Bot();
		}


		public static void Main(string[] args)
		{
			//Start Bot the first time, before timer
			Algorithm.Bot();

			//Start Timer
			Timer timer = new Timer();
			timer.Interval = Globals.timerInterval * 60 * 1000; // converts ms to minutes
			timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
			timer.Enabled = true;

			timerFired.WaitOne();  //https://stackoverflow.com/questions/34958759/timer-does-not-fire-before-application-ends-in-c-sharp?rq=1
		}
	}
}