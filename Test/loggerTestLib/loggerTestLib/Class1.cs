using System;
using NLog;
namespace loggerTestLib
{
    public class Class1
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
