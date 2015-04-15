using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ItWebSite.Core.DbModel;

namespace ItWebSite.Web.Models
{
    public class CnblogsBlogViewModel
    {
        public CnblogsBlogViewModel()
        {
            CnblogsComments = new List<CnblogsComment>();
        }

        public CnblogsBlog CnblogsBlog { get; set; }

        public IEnumerable<CnblogsComment> CnblogsComments { get; set; } 
    }
}