using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamWorker
{
    internal class WebDriverFactory : GameClaimerFactory
    {
        public override ISteamReedemer CreateSteamRedemer()
        {
            return new WebDriver();
        }
    }
}