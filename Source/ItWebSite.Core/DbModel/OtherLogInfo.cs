using ItWebSite.Core.Model;

namespace ItWebSite.Core.DbModel
{
    public class OtherLogInfo:BaseTable
    {
        public virtual string ClientName { get; set; }
        public virtual string UniqueName { get; set; }
        public virtual string TableName { get; set; }
        public virtual string ClassName { get; set; }
        public virtual string MethodName { get; set; }
        public virtual string LogType { get; set; }
        public virtual string Message { get; set; }
        public virtual string StackTrace { get; set; }
    }
}
