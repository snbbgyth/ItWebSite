using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading.Tasks;
using ItWebSite.Crawler;
using ItWebSite.Crawler.BLL;
using ItWebSite.Crawler.DAL;

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
        public static string CnBlogUrl = ConfigurationManager.AppSettings["CnBlogUrl"];

        public static string CsdnNewsUrl = ConfigurationManager.AppSettings["CsdnNewsUrl"];

        public static string News51CtoUrl = ConfigurationManager.AppSettings["News51CtoUrl"];

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
            if (crawlerType == CrawlerType.News51Cto)
            {
                Run51CtoCrawler();
            }
            if (crawlerType == CrawlerType.All)
            {
                RunBlogCrawler();
                RunNewsCrawler();
                Run51CtoCrawler();
            }
        }

        private static void Run51CtoCrawler()
        {
            Task.Factory.StartNew(() => HandleFactory.Get51CtoNewsCrawler().Crawler(Helper.News51CtoUrl));
        }

        private static void RunBlogCrawler()
        {
            Task.Factory.StartNew(() => HandleFactory.GetBlogCrawler().Crawler(Helper.CnBlogUrl));
        }

        private static void RunNewsCrawler()
        {
            Task.Factory.StartNew(() => HandleFactory.GetNewsCrawler().Crawler(Helper.CsdnNewsUrl));
        }
    }
}
