using System;
using NLog;
using loggerTestLib;
namespace nlogTest
{
    class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            _logger.Trace("trace message");
            _logger.Debug("debug message");
            _logger.Info("info message");
            _logger.Warn("warn message");
            _logger.Error("error message");
            _logger.Fatal("fatal message");

            LoggerTest.testFunc();

            Class1.testFunc();
        }
    }

    class LoggerTest
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static void testFunc()
        {
            _logger.Trace("trace message");
            _logger.Debug("debug message");
            _logger.Info("info message");
            _logger.Warn("warn message");
            _logger.Error("error message");
            _logger.Fatal("fatal message");
        }
    }
}
