using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
    public class HandlerCnBlogs : HandleBase
    {
        private static IContainer _container;

        private static IBlogContentTypeDal _blogContentTypeDal;

        private static string _blogContentTypeName = ConfigurationManager.AppSettings["CnblogsType"];

        static HandlerCnBlogs()
        {
            _blogContentTypeDal = Helper.Resolve<IBlogContentTypeDal>();
        }

        private static DateTime GetCreateTime(HtmlDocument document)
        {
            var createTime = document.GetElementbyId("post-date");
            DateTime result;
            if (createTime != null && DateTime.TryParse(createTime.InnerText, out result))
                return result;
            return DateTime.Now;
        }

        public override bool SaveContent(CrawledPage crawledPage)
        {
            try
            {
                var document = new HtmlDocument();
                document.LoadHtml(crawledPage.Content.Text);
                var title = document.GetElementbyId("cb_post_title_url");
                var body = document.GetElementbyId("cnblogs_post_body");
                if (title == null || body == null || string.IsNullOrEmpty(crawledPage.Uri.ToString()))
                    return false;
                if (_isSaveLocalFile)
                    SaveFile(title.InnerText, body.InnerHtml);
                SaveBlogContent(title.InnerText, body.InnerHtml, crawledPage.Uri.ToString(), GetCreateTime(document));
                return true;
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(typeof(HandlerCnBlogs), MethodBase.GetCurrentMethod().Name, ex);
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
            var entity = new BlogContent
            {
                BlogContentTypeId = blogContentTypeId,
                Content = body,
                Creater = "snbbdx@sina.com",
                LastModifier = "snbbdx@sina.com",
                CreateDate = createTime,
                LastModifyDate = createTime,
                DisplayOrder = 1,
                Title = title,
                BlogFrom = "博客园",
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
