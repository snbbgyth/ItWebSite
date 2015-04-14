using ItWebSite.Crawler.DAL;
using ItWebSite.Crawler.IDAL;

namespace ItWebSite.Crawler.BLL
{
    public  class HandleFactory
    {
        private  static ICrawler _blogCrawler=new HandlerCnBlogs();

        private  static ICrawler _csdnNewsCrawler=new HandlerCsdnNews();

        private static ICrawler _51CtonewsCrawler = new Handler51CtoNews();

        private static ICrawler _iteyeBlogCrawler = new HandlerCsdnBlog();

        public static ICrawler GetCrawler(CrawlerType crawlerType)
        {
            if (crawlerType == CrawlerType.CsdnNews)
                return _csdnNewsCrawler;
            if (crawlerType == CrawlerType.CnBlogs)
                return _blogCrawler;
            if (crawlerType == CrawlerType.News51Cto)
                return _51CtonewsCrawler;
            if (crawlerType == CrawlerType.CsdnBlog)
                return _iteyeBlogCrawler;
            return _blogCrawler;
        }

        public static ICrawler GetBlogCrawler()
        {
            return _blogCrawler;
        }

        public static ICrawler GetCsdnNewsCrawler()
        {
            return _csdnNewsCrawler;
        }

        public static ICrawler Get51CtoNewsCrawler()
        {
            return _51CtonewsCrawler;
        }

        public static ICrawler GetCsdnBlogCrawler()
        {
            return _iteyeBlogCrawler;
        }
    }

    public enum CrawlerType
    {
        All=1,
        CsdnNews,
        CnBlogs,
        News51Cto,
        CsdnBlog
    }
}
