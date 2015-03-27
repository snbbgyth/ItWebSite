using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.IDAL;
using ItWebSite.Core.QueueDAL;

namespace ItWebSite.Web.DAL.Manage
{
    public class CommentManage
    {
        private static  ICommentDal _commentDal;

        static CommentManage()
        {
            _commentDal = DependencyResolver.Current.GetService<ICommentDal>();
        }

        public static IEnumerable<Comment> LastComments(int count)
        {
            try
            {
                return _commentDal.QueryLast(count);
            }
            catch (Exception ex)
            {
                LogInfoQueue.Instance.Insert(typeof(CommentManage), MethodBase.GetCurrentMethod().Name, ex);
                return new List<Comment>();
            }
         
        }
    }
}