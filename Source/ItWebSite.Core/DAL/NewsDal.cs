﻿using System;
using System.Reflection;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Core.QueueDAL;

namespace ItWebSite.Core.DAL
{
    public class NewsDal : DataOperationActivityBase<News>, INewsDal
    {
        public virtual int DeleteByNewsFromUrl(string url)
        {
            int reslut = 0;

            try
            {
                using (var session = FluentNHibernateDal.Instance.GetSession())
                {
                    session.Delete(string.Format("from {0} where NewsFromUrl = '{1}'", typeof(News).Name, url));
                    session.Flush();
                }
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }
            return reslut;
        }

    }
}
