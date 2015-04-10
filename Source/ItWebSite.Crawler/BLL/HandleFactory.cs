using ItWebSite.Crawler.DAL;
using ItWebSite.Crawler.IDAL;

namespace ItWebSite.Crawler.BLL
{
    public  class HandleFactory
    {
        private  static ICrawler _blogCrawler=new HandlerCnBlogs();

        private  static ICrawler _newsCrawler=new HandleCsdnNews();

        private static ICrawler _51CtonewsCrawler = new Handle51CtoNews();

        public static ICrawler GetCrawler(CrawlerType crawlerType)
        {
            if (crawlerType == CrawlerType.CsdnNews)
                return _newsCrawler;
            if (crawlerType == CrawlerType.CnBlogs)
                return _blogCrawler;
            if (crawlerType == CrawlerType.News51Cto)
                return _51CtonewsCrawler;
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

        public static ICrawler Get51CtoNewsCrawler()
        {
            return _51CtonewsCrawler;
        }
    }

    public enum CrawlerType
    {
        All=1,
        CsdnNews,
        CnBlogs,
        News51Cto
       
    }
}
