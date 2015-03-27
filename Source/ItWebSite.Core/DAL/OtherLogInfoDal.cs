using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;

namespace ItWebSite.Core.DAL
{
    public class OtherLogInfoDal : DataOperationActivityBase<OtherLogInfo>, IOtherLogInfo
    {
        public static OtherLogInfoDal Current
        {
            get { return new OtherLogInfoDal(); }
        }
    }
}
