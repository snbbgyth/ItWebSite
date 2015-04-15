using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItWebSite.Core.DbModel;

namespace ItWebSite.Core.IDAL
{
    public interface ICnblogsBlogDal : IDataOperationActivity<CnblogsBlog>
    {
        int DeleteByBlogFromUrl(string url);
    }
}
