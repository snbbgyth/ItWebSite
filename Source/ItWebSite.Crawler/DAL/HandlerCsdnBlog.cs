using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using Autofac;
using HtmlAgilityPack;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Core.QueueDAL;
using ItWebSite.Crawler.Help;

namespace ItWebSite.Crawler.DAL
{
    public class HandlerCsdnBlog : HandlerBase
    {
        private static IContainer _container;

        private static IBlogContentTypeDal _blogContentTypeDal;

        private static string _blogContentTypeName = ConfigurationManager.AppSettings["CsdnBlogType"];

        static HandlerCsdnBlog()
        {
            _blogContentTypeDal = Helper.Resolve<IBlogContentTypeDal>();
        }

        private static DateTime GetCreateTime(HtmlDocument document)
        {
            var createTime = document.DocumentNode.SelectNodes("//span").SingleOrDefault(t => t.Attributes.Any(s => s.Name == "class" && s.Value == "link_postdate")); 
            DateTime result;
            if (createTime != null && DateTime.TryParse(createTime.InnerText, out result))
                return result;
            return DateTime.Now;
        }

        public override bool SaveContent(CrawledPage crawledPage)
        {
            try
            {
                if (!crawledPage.Uri.ToString().Contains("http://blog.csdn.net/")) return false;
                var document = new HtmlDocument();
                document.LoadHtml(crawledPage.Content.Text);
                var title = document.DocumentNode.SelectNodes("//span").SingleOrDefault(t => t.Attributes.Any(s => s.Name == "class" && s.Value == "link_title"));
                var body = document.GetElementbyId("article_content");
                var createTime = GetCreateTime(document);
                if (title == null||title.FirstChild==null || body == null || string.IsNullOrEmpty(crawledPage.Uri.ToString()))
                    return false;
                if (_isSaveLocalFile)
                    SaveFile(title.FirstChild.InnerText, body.InnerHtml);
                SaveBlogContent(title.FirstChild.InnerText,   body.InnerHtml, crawledPage.Uri.ToString(), createTime);
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
                LogInfoQueue.Instance.Insert(typeof(HandlerCnBlogs), MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        private void SaveBlogContent(string title, string body, string sourceUrl, DateTime createTime)
        {
            var blogContentTypeId = GetBlogContentTypeId(_blogContentTypeName);
            var entity = new CsdnBlog
            {
                BlogContentTypeId = blogContentTypeId,
                Content = body,
                Creater = "snbbdx@sina.com",
                LastModifier = "snbbdx@sina.com",
                CreateDate = createTime,
                LastModifyDate = createTime,
                DisplayOrder = 1,
                Title = title,
                BlogFrom = "CSDN博客",
                BlogFromUrl = sourceUrl
            };
            HandlerQueue.Instance.Add(entity);
        }

        private static int? blogContentTypeId = null;

        private static object _syncTypeId = new object();

        private int GetBlogContentTypeId(string typeName)
        {
            lock (_syncTypeId)
            {
                if (blogContentTypeId == null)
                {
                    blogContentTypeId = GetBlogContentTypeIdFromDb(typeName);
                }
                return (int)blogContentTypeId;
            }
        }

        private int GetBlogContentTypeIdFromDb(string typeName)
        {
            var entityList = _blogContentTypeDal.QueryByFun(t => t.Name == typeName);
            if (entityList.Any())
            {
                return entityList.First().Id;
            }
            return _blogContentTypeDal.Insert(new BlogContentType
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
