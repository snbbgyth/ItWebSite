using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading.Tasks;
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
            ExecuteHandler.Execute();
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
        public static string BlogUrl = ConfigurationManager.AppSettings["BlogUrl"];

        public static string NewsUrl = ConfigurationManager.AppSettings["NewsUrl"];

        public static string CrawType = ConfigurationManager.AppSettings["CrawType"];

        public static CrawlerType GetCrawlerType()
        {
            CrawlerType crawlerType;
            CrawlerType.TryParse(CrawType, out crawlerType);
            return crawlerType;
        }
    }

    public class ExecuteHandler
    {
        public static  void Execute()
        {
            var crawlerType = Helper.GetCrawlerType();
            if (crawlerType == CrawlerType.CnBlogs)
            {
                RunBlogCrawler();
            }
            if (crawlerType == CrawlerType.CsdnNews)
            {
                RunNewsCrawler();
            }
            if (crawlerType == CrawlerType.All)
            {
                RunBlogCrawler();
                RunNewsCrawler();
            }
        }

        private static void RunBlogCrawler()
        {
            Task.Factory.StartNew(() => HandleFactory.GetBlogCrawler().Crawler(Helper.BlogUrl));
        }

        private static void RunNewsCrawler()
        {
            Task.Factory.StartNew(() => HandleFactory.GetNewsCrawler().Crawler(Helper.NewsUrl));
        }
    }
}
