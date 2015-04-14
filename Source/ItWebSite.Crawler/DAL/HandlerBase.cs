using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Abot.Crawler;
using Abot.Poco;
using Autofac;
using HtmlAgilityPack;
using ItWebSite.Core.BLL;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Core.QueueDAL;
using ItWebSite.Crawler.IDAL;

namespace ItWebSite.Crawler.DAL
{
    public abstract class HandlerBase : ICrawler
    {

        public static bool _isSaveLocalFile;

        static HandlerBase()
        {
            _isSaveLocalFile = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSaveLocalFile"]);
        }

        public void Crawler(string url)
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                PrintDisclaimer();
                Uri uriToCrawl = GetSiteToCrawl(url);
                IWebCrawler crawler;
                //Uncomment only one of the following to see that instance in action
                crawler = GetDefaultWebCrawler();
                //crawler = GetManuallyConfiguredWebCrawler();
                //crawler = GetCustomBehaviorUsingLambdaWebCrawler();
                //Subscribe to any of these asynchronous events, there are also sychronous versions of each.
                //This is where you process data about specific events of the crawl
                crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
                crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
                crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
                crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;
                //Start the crawl
                //This is a synchronous call
                CrawlResult result = crawler.Crawl(uriToCrawl);
                //Now go view the log.txt file that is in the same directory as this executable. It has
                //all the statements that you were trying to read in the console window :).
                //Not enough data being logged? Change the app.config file's log4net log level from "INFO" TO "DEBUG"
                //PrintDisclaimer();
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(typeof(HandlerCsdnNews), MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        private static IWebCrawler GetDefaultWebCrawler()
        {
            return new PoliteWebCrawler();
        }

        private static IWebCrawler GetCustomBehaviorUsingLambdaWebCrawler()
        {
            IWebCrawler crawler = GetDefaultWebCrawler();

            //Register a lambda expression that will make Abot not crawl any url that has the word "ghost" in it.
            //For example http://a.com/ghost, would not get crawled if the link were found during the crawl.
            //If you set the log4net log level to "DEBUG" you will see a log message when any page is not allowed to be crawled.
            //NOTE: This is lambda is run after the regular ICrawlDecsionMaker.ShouldCrawlPage method is run.
            crawler.ShouldCrawlPage((pageToCrawl, crawlContext) =>
            {
                if (pageToCrawl.Uri.AbsoluteUri.Contains("ghost"))
                    return new CrawlDecision { Allow = false, Reason = "Scared of ghosts" };
                return new CrawlDecision { Allow = true };
            });

            //Register a lambda expression that will tell Abot to not download the page content for any page after 5th.
            //Abot will still make the http request but will not read the raw content from the stream
            //NOTE: This lambda is run after the regular ICrawlDecsionMaker.ShouldDownloadPageContent method is run
            crawler.ShouldDownloadPageContent((crawledPage, crawlContext) =>
            {
                if (crawlContext.CrawledCount >= 5)
                    return new CrawlDecision { Allow = false, Reason = "We already downloaded the raw page content for 5 pages" };

                return new CrawlDecision { Allow = true };
            });

            //Register a lambda expression that will tell Abot to not crawl links on any page that is not internal to the root uri.
            //NOTE: This lambda is run after the regular ICrawlDecsionMaker.ShouldCrawlPageLinks method is run
            crawler.ShouldCrawlPageLinks((crawledPage, crawlContext) =>
            {
                if (!crawledPage.IsInternal)
                    return new CrawlDecision { Allow = false, Reason = "We dont crawl links of external pages" };

                return new CrawlDecision { Allow = true };
            });

            return crawler;
        }

        private static Uri GetSiteToCrawl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ApplicationException("Site url to crawl is as a required parameter");
            return new Uri(url);
        }

        private static void PrintDisclaimer()
        {
            PrintAttentionText("The demo is configured to only crawl a total of 10 pages and will wait 1 second in between http requests. This is to avoid getting you blocked by your isp or the sites you are trying to crawl. You can change these values in the app.config or Abot.Console.exe.config file.");
        }

        private static void PrintAttentionText(string text)
        {
            ConsoleColor originalColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(text);
            System.Console.ForegroundColor = originalColor;
        }

        void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            //Process data
        }

        void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            //Process data
            SaveContent(e.CrawledPage);
         
        }

        void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            //Process data
        }

        void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            //Process data
        }

        private DateTime ConvertToDateTime(string value)
        {
            DateTime result;
            if (DateTime.TryParse(value, out result))
                return result;
            return DateTime.MinValue;
        }

        public abstract bool SaveContent(CrawledPage crawledPage);

        public string SavePath()
        {
            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);
            return basePath;
        }
    }
}
