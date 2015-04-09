using ItWebSite.Crawler.IDAL;

namespace ItWebSite.Crawler.DAL
{
    public  class HandleFactory
    {
        private  static ICrawler _blogCrawler=new HandlerBlog();

        private  static ICrawler _newsCrawler=new HandleNews();

        public static ICrawler GetCrawler(CrawlerType crawlerType)
        {
            if (crawlerType == CrawlerType.CsdnNews)
                return _newsCrawler;
            return _blogCrawler;
        }

        public static ICrawler GetBlogCrawler()
        {
            return _blogCrawler;
        }

        public static ICrawler GetNewsCrawler()
        {
            return _newsCrawler;
        }
    }

    public enum CrawlerType
    {
        CsdnNews=1,
        CnBlogs,
        All
    }
}
