using System;
using System.Reflection;
using ItWebSite.Core.DAL;
using ItWebSite.Core.DbModel;
using ItWebSite.Core.Help;

namespace ItWebSite.Core.QueueDAL
{
    public class LogInfoQueue : BaseQueueDal<OtherLogInfo>
    {
        private static readonly object SyncObj = new object();

        private static LogInfoQueue _instance;

        public static LogInfoQueue Instance
        {
            get
            {
                lock (SyncObj)
                {
                    if (_instance == null)
                        _instance = new LogInfoQueue();
                }
                return _instance;
            }
        }

        private LogInfoQueue()
            : base()
        {

        }

        public override void OnNotify(OtherLogInfo entity)
        {
            try
            {
                OtherLogInfoDal.Current.SaveOrUpdate(entity);
            }
            catch (Exception ex)
            {
                Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        #region OtherLogInfo(Error,Warning,Information)

        #region Error

        public void Insert(Type type, string methodName, Exception exception)
        {
            var entity = new OtherLogInfo
            {
                CreateDate = DateTime.Now,
                ClassName = type.Name,
                MethodName = methodName,
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                LogType = LogMessageType.Error.ToString()
            };
            Add(entity);
        }

        public void Insert(Type type, string methodName, Exception exception, string clientName)
        {
            var entity = new OtherLogInfo
            {
                CreateDate = DateTime.Now,
                ClassName = type.Name,
                MethodName = methodName,
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                ClientName = clientName,
                LogType = LogMessageType.Error.ToString()
            };
            Add(entity);
        }

        public void Insert(Type type, string methodName, Exception exception, string clientName, string uniqueName,
            string tableName)
        {
            var entity = new OtherLogInfo
            {
                CreateDate = DateTime.Now,
                ClassName = type.Name,
                MethodName = methodName,
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                ClientName = clientName,
                UniqueName = uniqueName,
                TableName = tableName,
                LogType = LogMessageType.Error.ToString()
            };
            Add(entity);
        }

        public void Insert(Type type, string methodName, string message, string stackTrace)
        {
            var entity = new OtherLogInfo
            {
                CreateDate = DateTime.Now,
                ClassName = type.Name,
                MethodName = methodName,
                Message = message,
                StackTrace = stackTrace,
                LogType = LogMessageType.Error.ToString()
            };
            Add(entity);
        }

        public void Insert(Type type, string methodName, string message, string stackTrace, string clientName)
        {
            var entity = new OtherLogInfo
            {
                CreateDate = DateTime.Now,
                ClassName = type.Name,
                MethodName = methodName,
                ClientName = clientName,
                Message = message,
                StackTrace = stackTrace,
                LogType = LogMessageType.Error.ToString()
            };
            Add(entity);
        }

        public void Insert(Type type, string methodName, string message, string stackTrace, LogMessageType logType)
        {
            var entity = new OtherLogInfo
            {
                CreateDate = DateTime.Now,
                ClassName = type.Name,
                MethodName = methodName,
                Message = message,
                StackTrace = stackTrace,
                LogType = logType.ToString()
            };
            Add(entity);
        }

        #endregion

        public void Insert(Type type, string methodName, string message, string clientName, string uniqueName, string tableName, LogMessageType logType = LogMessageType.Warning)
        {
            var entity = new OtherLogInfo
            {
                CreateDate = DateTime.Now,
                ClassName = type.Name,
                MethodName = methodName,
                Message = message,
                ClientName = clientName,
                UniqueName = uniqueName,
                TableName = tableName,
                LogType = logType.ToString()
            };
            Add(entity);
        }

        #endregion

    }
}
