using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Threading.Tasks;
using System;
using OpenQA.Selenium.Interactions;

namespace ServerControlSelenium{
    public class ServerBrowser {
        public static async Task Main(string[] args)
        {
            string url = "https://afsvdi.mahindra.com";
            ChromiumSession client = new ChromiumSession(url);
            int alpha=1;
            while(true)
            {
                client.MoveMouserforActiveness(alpha);
                await Task.Delay(TimeSpan.FromSeconds(30));
                alpha = alpha * -1;
            }                        
        }
    }

    public class ChromiumSession {
        public ChromeDriver driver;

        public ChromiumSession(string URL)
        {
            var options = new ChromeOptions();
            options.BinaryLocation= @"C:\portapps\ungoogled-chromium-portable\app\chrome.exe";
            driver = new ChromeDriver(@"C:\WebDriver\bin", options);
            driver.Navigate().GoToUrl(URL);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);
        }
        public void MoveMouserforActiveness(int alpha)
        {
            string handle =driver.WindowHandles[0];
            driver.SwitchTo().Window(handle);
            // Set x and y offset positions of element
            int xOffset = alpha * 100;
            int yOffset = alpha* 100;
            Actions actionProvider = new Actions(driver);
            // Performs mouse move action onto the offset position
            actionProvider.MoveByOffset(xOffset, yOffset).Build().Perform();
            actionProvider.Click();
            IAction keydown = actionProvider.SendKeys(Keys.Tab).Build();
            keydown.Perform();

                Console.WriteLine(TimeSpan.FromSeconds(1800).ToString());

        }

        public void closeSession(){
            driver.Quit();
        }
    }


}


