using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItWebSite.Core.DbModel;

namespace ItWebSite.Core.IDAL
{
    public interface INews51CtoDal : IDataOperationActivity<News51Cto>
    {
        int DeleteByNewsFromUrl(string url);
    }
}
