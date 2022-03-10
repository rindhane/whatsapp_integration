// See https://aka.ms/new-console-template for more information
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace proxyArrangement
{
    public class browserAutomation {
        public static void Main (string[] args )
        {  
            System.Console.WriteLine("Hello, World!");
            //InternetExplorerSession client = new InternetExplorerSession();
            ChromiumSession client = new ChromiumSession();

            client.startAction();
        }
        
    }

    public class InternetExplorerSession {

        public InternetExplorerSession(){

        }
        public void startAction()
        {
            new DriverManager().SetUpDriver(new InternetExplorerConfig());
            var options = new InternetExplorerOptions();
            var driver = new InternetExplorerDriver(options);
            driver.Quit();
        }

    }
    public class ChromiumSession {

        public ChromiumSession(){

        }
        public void startAction()
        {
            //new DriverManager().SetUpDriver(new ChromeConfig());
            var options = new ChromeOptions();
            options.BinaryLocation= @"C:\portapps\ungoogled-chromium-portable\app\chrome.exe";
            var driver = new ChromeDriver(@"C:\WebDriver\bin", options);
            driver.Navigate().GoToUrl("https://www.mahindra.com");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);
            Task.Delay(TimeSpan.FromSeconds(30));
            driver.Quit();
        }
    }

    public class ProxyActiveService:BackgroundService {

        private readonly ChromiumSession _client;


        public ProxyActiveService(ChromiumSession client)
        {
            _client = client;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _client.startAction();
                    System.Console.WriteLine($"Proxy was approached: {DateTime.Now.ToString()}");
                }catch (System.Exception e){
                    System.Console.WriteLine("ProxyError:"+e.Message);
                }
                //_logger.writeNotification(response);
                //System.Console.WriteLine(response);
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(System.TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
     }   
}