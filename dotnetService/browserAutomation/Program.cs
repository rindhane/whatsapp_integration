// See https://aka.ms/new-console-template for more information
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
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
            //InternetExplorerSession clientIE=new InternetExplorerSession();
            //clientIE.startAction();
            client.closeSession();
            //clientIE.closeSession();
        }
        
    }

    public class InternetExplorerSession {
        public InternetExplorerDriver driver ;


        public InternetExplorerSession(){
            //new DriverManager().SetUpDriver(new InternetExplorerConfig());
            var options = new InternetExplorerOptions();
            options.IgnoreZoomLevel=true;
            driver = new InternetExplorerDriver(@"C:\Webdriver\IE\bin",options);
            driver.Navigate().GoToUrl("https://www.mahindrarise.com");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);   
        }
        public void startAction()
        {
          driver.Navigate().Refresh();
        }
        public void closeSession()
        {
            driver.Quit();
            driver.Dispose();
        }

    }
    public class ChromiumSession {
        public ChromeDriver driver ;

        public ChromiumSession()
        {
            //new DriverManager().SetUpDriver(new ChromeConfig());
        }
        public void startAction()
        {
            var options = new ChromeOptions();
            options.BinaryLocation= @"C:\portapps\ungoogled-chromium-portable\app\chrome.exe";
            driver = new ChromeDriver(@"C:\WebDriver\bin", options);
            driver.Navigate().GoToUrl("https://www.mahindra.com");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(1000);
            //driver.Navigate().Refresh();
        }
        public void closeSession()
        {
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
                    await Task.Delay(System.TimeSpan.FromSeconds(5), stoppingToken);
                    _client.closeSession();
                    System.Console.WriteLine($"Proxy was approached: {DateTime.Now.ToString()}");
                }catch (System.Exception e){
                    System.Console.WriteLine("ProxyError:"+e.Message);
                }
                //_logger.writeNotification(response);
                //System.Console.WriteLine(response);
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(System.TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
     }   
}