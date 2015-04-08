﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abot.Crawler;
using Abot.Poco;
using Autofac;
using HtmlAgilityPack;
using ItWebSite.Core.BLL;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Core.QueueDAL;

namespace ItWebSite.Crawler
{
    public class HandleNews:ICrawler
    {
        private static IContainer _container;

        private static INewsDal _newsDal;
        private static INewsTypeDal _newsTypeDal;

        private static string _newsTypeName = ConfigurationManager.AppSettings["NewsTypeName"];

        private static bool _isSaveLocalFile;

        static HandleNews()
        {
            _container = BuildContainer();
            _newsDal = _container.Resolve<INewsDal>();
            _newsTypeDal = _container.Resolve<INewsTypeDal>();
            _isSaveLocalFile = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSaveLocalFile"]);
        }

        public static   T Resolve<T>()
        {
            try
            {
                return _container.Resolve<T>();
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }


        public   void Crawler(string url)
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

                PrintDisclaimer();

            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(typeof(HandleNews), MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            // FunnelWeb Database
            builder.RegisterModule(new CoreModule());
            return builder.Build();
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

        static void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            //Process data
        }

        static void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            //Process data
            if (e.CrawledPage.Uri.ToString().Contains("http://www.csdn.net/article"))
            SaveContent(e.CrawledPage);
        }

        static void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            //Process data
        }

        static void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            //Process data
        }

        static bool SaveContent(CrawledPage crawledPage)
        {
            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(crawledPage.Content.Text);
                var title = document.DocumentNode.SelectSingleNode("//title");
                var body = document.DocumentNode.SelectSingleNode("//body");
                var summary =body.SelectNodes("//div").SingleOrDefault(t => t.Attributes.Any(s => s.Name == "class" && s.Value == "summary"));

                body =body.SelectNodes("//div").SingleOrDefault(t => t.Attributes.Any(s => s.Name == "class" && s.Value == "con news_content"));
                if (title == null || body == null || string.IsNullOrEmpty(crawledPage.Uri.ToString()))
                    return false;
                if (_isSaveLocalFile)
                    SaveFile(title.InnerText, body.InnerHtml);
                SaveNews(title.InnerText,summary==null?string.Empty:summary.InnerHtml, body.InnerHtml, crawledPage.Uri.ToString());
                return true;
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(typeof(HandleNews), MethodBase.GetCurrentMethod().Name, ex);
                return false;
            }
        }

        private static void SaveFile(string title, string body)
        {
            var filePath = Path.Combine(SavePath(), title + ".html");
            try
            {
                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    writer.Write(body);
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(typeof(HandleNews), MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        private static void SaveNews(string title,string summary, string body, string sourceUrl)
        {
            var typeId = GetNewsTypeId(_newsTypeName);
            var entity = new News
            {
                NewsTypeId = typeId,
                Content = body,
                Creater = "snbbdx@sina.com",
                LastModifier = "snbbdx@sina.com",
                CreateDate = DateTime.Now,
                LastModifyDate = DateTime.Now,
                DisplayOrder = 1,
                Title = title,
                NewsFrom = "CSDN",
                NewsFromUrl = sourceUrl,
                Summary = summary
            };
            HandlerQueue.Instance.Add(entity);
        }

        private static int? _newsTypeId = null;

        private static object _syncTypeId = new object();

        private static int GetNewsTypeId(string typeName)
        {
            lock (_syncTypeId)
            {
                if (_newsTypeId == null)
                {
                    _newsTypeId = GetNewsTypeIdFromDb(typeName);
                }
                return (int)_newsTypeId;
            }
        }

        private static int GetNewsTypeIdFromDb(string typeName)
        {
            var entityList = _newsTypeDal.QueryByFun(t => t.Name == typeName);
            if (entityList.Any())
            {
                return entityList.First().Id;
            }
            return _newsTypeDal.Insert(new NewsType()
            {
                Creater = "snbbdx@sina.com",
                LastModifier = "snbbdx@sina.com",
                CreateDate = DateTime.Now,
                LastModifyDate = DateTime.Now,
                Name = typeName
            });
        }

        

        private static string NoHtml(string Htmlstring)
        {
            //删除脚本   
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            //Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();

            return Htmlstring;
        }

        static string SavePath()
        {
            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);
            return basePath;
        }
    }
}