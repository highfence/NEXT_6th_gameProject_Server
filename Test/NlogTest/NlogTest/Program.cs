using System;
using NLog;
using NLog.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

/*
 * 처음 Nlog를 사용하려고 했는데 로그를 출력하고 프로그램이 종료됨,
 * 그래서 .net core 2 콘솔 앱에서의 방법으로 다시 시도함. https://github.com/NLog/NLog.Extensions.Logging/wiki/Getting-started-with-.NET-Core-2---Console-application
 * 
 * 0. 프로젝트를 생성한다.
 * 1. NLog.Extensions.Logging, Microsoft.Extensions.DependencyInjection를 추가해 주고 그리고 가능하다면  Nlog를 업데이트 해준다. 
 *    세부적인 과정은 도구의 패키지 매니져에서 
 *    
 *    Install-Package NLog.Extensions.Logging -Version 1.0.0-rtm-rc2,
 *    
 *    Install-Package Microsoft.Extensions.DependencyInjection -Version 2.0.0
 *    
 *    을 입력해서 NLog.Extensions.Logging, Microsoft.Extensions.DependencyInjection를 추가해준다.
 *     
 * 2. nlog.config 파일을 만든다. root of your project에 만들어야 한다.
 *    처음에는 Program.cs 파일의 위치에 만들었는데 loggerFactory.ConfigureNLog("nlog.config") 부분에서 파일이 없다는 예외가 발생해서
 *    확인한 결과 solutionFolder\NlogTest\bin\Debug\netcoreapp2.0 에서 nlog.config파일을 찾고 있어서 일단 넣어 좋고 만약 릴리즈 버전으로 실행하면
 *    어떻게 해야 할지 알아봐야 한다. https://github.com/NLog/NLog/wiki/Configuration-file#configuration-file-locations
 *    
 *    파일의 내용은 예제의 내용을 그대로 사용했다. targets나 rules를좀더 봐야 할것 같다.
 *    
 * 3. 왜인지는 모르겠지만 Nlog 만을 사용하는게 아니라 ms의 익스텐션을 사용한다. BuildDi() 에서 로그를 기록해 주는 시스템이랑 내가 만든 클래스를 연결을 해주고
 *    그 다음 실제 로그를 출력한 내가 만든 클래스를 할당한다. 그래서 이 시스템을 사용할 커스텀 로거클래스를 하나 만든 후 그것을 각각의 클래스가 갖고 로그를 출력하는 방식으로 
 *    사용해야 할것같다. 아니면 아래 한것처럼 사용할 클래스를 모두 등록해준다. 아니면 다른 더 효율적인 방법이 있을 수도 있다. 일단 Nlog의 깃헙 페이지의 튜토리얼에 나온 방법이다. 
 *    
 * 4.  _logger.LogDebug(20, "Doing hard work! {Action}", name); 라고 하면
 *     2017/10/28 15:12:19.477|DEBUG|Doing hard work! Action1 |NlogTest.Runner|Action=Action1, EventId_Id=20, EventId_Name=, EventId=20 이렇게 출력된다.
 *     EventId는 왜 존재 하는지 잘모르겠고 어떻게 관리해야 하는지도 애매하다.검색해본결과 별고 쓰지 않는다고 한다.
 */
namespace NlogTest
{
    class Program
    {
        private static IServiceProvider BuildDi()
        {
            var services = new ServiceCollection();

            //Runner is the custom class
            services.AddTransient<Runner>();
            services.AddTransient<MyClass>();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            var serviceProvider = services.BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            //configure NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            loggerFactory.ConfigureNLog("nlog.config");

            return serviceProvider;
        }

        static void Main(string[] args)
        {
            var servicesProvider = BuildDi();
            var runner = servicesProvider.GetRequiredService<Runner>();
            var test = servicesProvider.GetRequiredService<MyClass>();

            test.TestLog();
            runner.DoAction("Action1");

            Console.WriteLine("Press ANY key to exit");
            Console.ReadLine();
        }
    }

}
