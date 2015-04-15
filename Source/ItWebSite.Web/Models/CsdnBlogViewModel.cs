using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ItWebSite.Core.DbModel;

namespace ItWebSite.Web.Models
{
    public class CsdnBlogViewModel
    {
        public CsdnBlogViewModel()
        {
            CsdnblogComments = new List<CsdnBlogComment>();
        }

        public CsdnBlog CsdnBlog { get; set; }

        public IEnumerable<CsdnBlogComment> CsdnblogComments { get; set; } 
    }
}