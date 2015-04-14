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
using ItWebSite.Crawler.Help;
using ItWebSite.Crawler.IDAL;

namespace ItWebSite.Crawler.DAL
{
    public class Handler51CtoNews : HandlerBase
    {
 
        private static INewsTypeDal _newsTypeDal;
        private static string _newsTypeName = ConfigurationManager.AppSettings["News51CtoType"];

        public  Handler51CtoNews()
        {
            _newsTypeDal = Helper.Resolve<INewsTypeDal>();
        }
 
        private static DateTime GetCreateTime(HtmlDocument document)
        {
            var node = document.DocumentNode.SelectNodes("//div").SingleOrDefault(t => t.Attributes.Any(s => s.Name == "class" && s.Value == "msg"));
            var childNode = node.SelectSingleNode("//div");
            if (childNode != null&&childNode.InnerText.Length>16)
                return ConvertToDateTime(node.InnerText);
            return DateTime.Now;
        }

        private static DateTime ConvertToDateTime(string value)
        {

            DateTime result;
            if (DateTime.TryParse(value.Substring(0,16), out result))
                return result;
            return DateTime.MinValue;
        }

        public override bool SaveContent(CrawledPage crawledPage)
        {
            try
            {
                if (!crawledPage.Uri.ToString().Contains("51cto.com/art")) return false;
                var document = new HtmlDocument();
                document.LoadHtml(crawledPage.Content.Text);
                var title = document.DocumentNode.SelectSingleNode("//h1") ;
                var body = document.GetElementbyId("content");
                var createTime = GetCreateTime(document);
                var summary =document.DocumentNode.SelectNodes("//p").SingleOrDefault(t => t.Attributes.Any(s => s.Name == "class" && s.Value == "f14 green"));
                if (title == null || body == null || string.IsNullOrEmpty(crawledPage.Uri.ToString()))
                    return false;
                if (_isSaveLocalFile)
                    SaveFile(title.InnerText, body.InnerHtml);
                SaveNews(title.InnerText, summary == null ? string.Empty : summary.InnerHtml, body.InnerHtml, crawledPage.Uri.ToString(), createTime);
                return true;
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(typeof(HandlerCsdnNews), MethodBase.GetCurrentMethod().Name, ex);
                return false;
            }
        }

        private void SaveFile(string title, string body)
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
                LogInfoQueue.Instance.Insert(typeof(HandlerCsdnNews), MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        private void SaveNews(string title, string summary, string body, string sourceUrl, DateTime createTime)
        {
            var typeId = GetNewsTypeId(_newsTypeName);
            var entity = new News51Cto
            {
                NewsTypeId = typeId,
                Content = body,
                Creater = "snbbdx@sina.com",
                LastModifier = "snbbdx@sina.com",
                CreateDate = createTime,
                LastModifyDate = createTime,
                DisplayOrder = 1,
                Title = title,
                NewsFrom = "51CTO",
                NewsFromUrl = sourceUrl,
                Summary = summary
            };
            HandlerQueue.Instance.Add(entity);
        }

        private static int? _newsTypeId = null;

        private static object _syncTypeId = new object();

        private int GetNewsTypeId(string typeName)
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

        private int GetNewsTypeIdFromDb(string typeName)
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

    }
}
