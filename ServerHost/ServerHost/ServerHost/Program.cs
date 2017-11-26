using System;
using NLog;
namespace ServerHost
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            logger.Trace("test Trace");

            logger.Debug("test Debug");

            logger.Info("test Info");

            logger.Error("test Error");

            logger.Fatal("test Fatal");
        }
    }
}
