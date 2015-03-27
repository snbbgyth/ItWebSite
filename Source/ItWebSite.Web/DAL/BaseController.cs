using System;
using System.Web.Mvc;
using ItWebSite.Core.Model;
using ItWebSite.Web.DAL.MySql;

namespace ItWebSite.Web.DAL
{
    [MyAuthorize]
    public class BaseController:Controller
    {
        public T InitInsert<T>(T entity) where T : BaseTable
        {
            entity.Creater = User.Identity.Name;
            entity.LastModifier = User.Identity.Name;
            entity.CreateDate = DateTime.Now;
            entity.LastModifyDate = DateTime.Now;
            return entity;
        }

        public T InitModify<T>(T entity) where T : BaseTable
        {
            entity.LastModifier = User.Identity.Name;
            entity.LastModifyDate = DateTime.Now;
            return entity;
        }
    }
}