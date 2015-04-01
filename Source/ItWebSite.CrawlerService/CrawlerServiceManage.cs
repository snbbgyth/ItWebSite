using System;
using System.Configuration.Install;
using System.Reflection;
using ItWebSite.Crawler;

namespace ItWebSiteCrawlerService
{
   public class CrawlerServiceManage
    {
        private static readonly string _exePath = Assembly.GetExecutingAssembly().Location;

        public static bool InstallMe()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new string[]
                {
                    _exePath
                });
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool UninstallMe()
        {
            try
            {

                ManagedInstallerClass.InstallHelper(new string[]
                {
                    "/u",
                    _exePath
                });
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool Run(bool isConsole = true)
        {
            try
            {
                Handler.Crawler(Helper.Url);

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

  
    }
}
