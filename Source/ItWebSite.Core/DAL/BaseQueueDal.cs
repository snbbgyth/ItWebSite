using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using ItWebSite.Core.QueueDAL;

namespace ItWebSite.Core.DAL
{
    public abstract class BaseQueueDal<T> : IDisposable where T : new()
    {
        /// <summary>
        /// Wait enqueue wirte log message semaphore will release
        /// </summary>
        private Semaphore _semaphore;

        protected BaseQueueDal()
        {
            _messageList = new ConcurrentQueue<T>();
            _semaphore = new Semaphore(0, int.MaxValue);
            var thread = new Thread(Work) { IsBackground = true, Priority = ThreadPriority.Highest };
            thread.Start();
        }

        private bool _isDispose;

        private void Work(object obj)
        {
            while (true)
            {
                if (_isDispose) break;
                try
                {
                    var entity = Get();
                    if (entity == null)
                        WaitHandle.WaitAny(new WaitHandle[] { _semaphore }, 10000, false);
                    else
                    {
                        OnNotify(entity);
                    }
                }
                catch (Exception ex)
                {
                    LogInfoQueue.Instance.Insert(GetType(), MethodBase.GetCurrentMethod().Name, ex);
                }
            }
        }

        private ConcurrentQueue<T> _messageList;

        private readonly object _syncMessage = new object();

        public virtual void Add(T entity)
        {
            if (entity == null) return;
            lock (_syncMessage)
            {
                _messageList.Enqueue(entity);
                _semaphore.Release();
            }
        }

        public void Add(List<T> entityList)
        {
            entityList.ForEach(Add);
        }

        public abstract void OnNotify(T entity);

        public T Get()
        {
            T entity;
            _messageList.TryDequeue(out entity);
            return entity;
        }

        public bool IsExist(Func<T, bool> func)
        {
            return _messageList.Any(func);
        }

        public void Dispose()
        {
            _messageList = new ConcurrentQueue<T>();
            _isDispose = true;
        }
    }
}
