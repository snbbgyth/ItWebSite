using System;
using System.Configuration;
using System.ServiceProcess;
using ItWebSite.Crawler;

namespace ItWebSiteCrawlerService
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Environment.UserInteractive)//Windows Service
            {
                RunAsService();
                return;
            }

            ///使用命令行启动
            if (args != null && args.Length > 0)
            {
                if (args[0].Equals("-i", StringComparison.OrdinalIgnoreCase))
                    CrawlerServiceManage.InstallMe();
                else if (args[0].Equals("-u", StringComparison.OrdinalIgnoreCase))
                    CrawlerServiceManage.UninstallMe();
                else if (args[0].Equals("-c", StringComparison.OrdinalIgnoreCase))
                    RunAsConsole();
                else
                    Console.WriteLine(args[0]);
            }
            else
                RunAsConsole();
        }

        public static void RunAsConsole()
        {
            HandleFactory.GetCrawler(Helper.GetCrawlerType()).Crawler(Helper.Url);
            Console.WriteLine("Input Q to exit.");
            while (string.Compare(Console.ReadLine(), ConsoleKey.Q.ToString(), StringComparison.OrdinalIgnoreCase) != 0)
            {
            }
        }

        /// <summary>
        /// 以服务的形式来运行
        /// </summary>
        static void RunAsService()
        {
            ServiceBase[] serviceToRun = new ServiceBase[]
            {
                new CrawlerServerService(), 
            };
            ServiceBase.Run(serviceToRun);
        }
    }

    public class Helper
    {
        public static string Url = ConfigurationManager.AppSettings["Url"];

        public static string CrawType = ConfigurationManager.AppSettings["CrawType"];

        public static CrawlerType GetCrawlerType()
        {
            CrawlerType crawlerType;
            CrawlerType.TryParse(Helper.CrawType, out crawlerType);
            return crawlerType;
        }
    }
}
