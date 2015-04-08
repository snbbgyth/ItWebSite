using ItWebSite.Core.DbModel;

namespace ItWebSite.Core.IDAL
{
    public interface INewsDal : IDataOperationActivity<News>
    {
        int DeleteByNewsFromUrl(string url);
    }
}
