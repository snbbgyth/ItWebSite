using System;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace ItWebSite.Web.Help
{
    public class NullJsonResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            var response = context.HttpContext.Response;
            response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;
            this.Data = null;
            var serializedObject = JsonConvert.SerializeObject(Data, Formatting.Indented);
            response.Write(serializedObject);
        }
    }
}