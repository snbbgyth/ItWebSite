using ItWebSite.Core.DbModel;

namespace ItWebSite.Core.IDAL
{
    public interface INewsCsdnDal : IDataOperationActivity<NewsCsdn>
    {
        int DeleteByNewsFromUrl(string url);
    }
}
