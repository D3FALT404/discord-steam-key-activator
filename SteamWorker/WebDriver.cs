using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Text.Json;
using OpenQA.Selenium;

namespace SteamWorker
{
    public class WebDriver : IDisposable, ISteamReedemer
    {
        private IWebDriver webDriver;

        private FirefoxProfile firefoxProfile;

        public WebDriver()
        {
            ChromeDriverInitize();
        }

        protected void ChromeDriverInitize()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            try
            {
                webDriver = new ChromeDriver(/*"/usr/lib/chromium-browser", */options);
                addCookies();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void RedeemGame(string key)
        {
            Console.WriteLine(key);
            GoToUrl($"https://store.steampowered.com/account/registerkey?key={key}");
            Click("//*[@id=\"accept_ssa\"]");
            Click("//*[@id=\"register_btn\"]/span");
            Dispose();
        }

        private void addCookies()
        {
            GoToUrl("https://store.steampowered.com/");
            var cookiesText = File.ReadAllText("cookies.json");
            Cookie[] cookies = JsonSerializer.Deserialize<Cookie[]>(cookiesText);
            foreach (var item in cookies)
            {
                webDriver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie(item.name, item.value));
            }
        }

        public void GoToUrl(string url)
        {
            webDriver.Navigate().GoToUrl(url);
        }

        public string[] GetHrefs(string xpath)
        {
            ReadOnlyCollection<IWebElement> elements = getElements(xpath);
            string[] hrefs = new string[elements.Count];
            for (int i = 0; i < elements.Count; i++)
            {
                hrefs[i] = getArtibbute(elements[i]);
            }
            return hrefs;
        }

        public double[] GetPrices(string xpath)
        {
            string[] pricesNotConverted = GetTexts(xpath);
            double[] prices = new double[pricesNotConverted.Length];
            for (int i = 0; i < prices.Length; i++)
            {
                prices[i] = double.Parse(pricesNotConverted[i].Replace(" zł", "").Replace(',', '.'), CultureInfo.InvariantCulture);
            }
            return prices;
        }

        public string[] GetTexts(string xpath)
        {
            ReadOnlyCollection<IWebElement> elements = getElements(xpath);
            string[] texts = new string[elements.Count];
            for (int i = 0; i < elements.Count; i++)
            {
                texts[i] = elements[i].Text;
            }
            return texts;
        }

        public void ClickElementsIfExist(string xpath)
        {
            try
            {
                clickElements(xpath);
            }
            catch (NoSuchElementException)
            {
                return;
            }
        }

        private void clickElements(string xpath)
        {
            ReadOnlyCollection<IWebElement> elements = getElements(xpath);
            foreach (var element in elements)
            {
                element.Click();
            }
        }

        public void Click(string xpath)
        {
            getElement(xpath).Click();
        }

        private ReadOnlyCollection<IWebElement> getElements(string xpath)
        {
            return webDriver.FindElements(By.XPath(xpath));
        }

        public string GetText(string xpath)
        {
            try
            {
                return getElement(xpath).Text;
            }
            catch (NoSuchElementException)
            {
                return "error";
            }
        }

        public string GetHref(string xpath)
        {
            return getArtibbute(getElement(xpath));
        }

        private string getArtibbute(IWebElement webElement)
        {
            return webElement.GetAttribute("href");
        }

        public void ClearAndType(string xpath, string text)
        {
            IWebElement textBlock = getElement(xpath);
            textBlock.Clear();
            textBlock.SendKeys(text);
        }

        public void TypeAndPressEnter(string xpath, string text)
        {
            typeInElement(xpath, text + Keys.Enter);
        }

        public void Type(string xpath, string text)
        {
            typeInElement(xpath, text);
        }

        private IWebElement typeInElement(string xpath, string text)
        {
            var element = getElement(xpath);
            element.SendKeys(text);
            return element;
        }

        private IWebElement getElement(string xpath)
        {
            return webDriver.FindElement(By.XPath(xpath));
        }

        public bool CheckIfDriverIsOnWebiste(string url)
        {
            return webDriver.Url == url;
        }

        public void Dispose()
        {
            webDriver?.Dispose();
        }
    }

    public class Cookie
    {
        public string domain { get; set; }
        public float expirationDate { get; set; }
        public bool hostOnly { get; set; }
        public bool httpOnly { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string sameSite { get; set; }
        public bool secure { get; set; }
        public bool session { get; set; }
        public string storeId { get; set; }
        public string value { get; set; }
        public int id { get; set; }
    }
}