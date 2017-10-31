using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using NLog.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace NlogTest
{
    public class MyClass
    {
        private readonly ILogger<MyClass> _logger;

        public MyClass(ILogger<MyClass> logger)
        {
            _logger = logger;
        }

        public void TestLog()
        {
            _logger.LogDebug("test log is static buildDi func needed?");
        }

    }
}
