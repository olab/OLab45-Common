using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;

namespace OLabWebAPI.Utils
{
    public class ConcurrentList<T>
    {
        private readonly IList<T> _items = new List<T>();
        private readonly ILogger _logger;
        private static readonly Mutex mutex = new Mutex();

        public ConcurrentList(ILogger logger)
        {
            _logger = logger;
        }

        public void Lock()
        {
            mutex.WaitOne();
        }

        public void Unlock()
        {
            mutex.ReleaseMutex();
        }

        public IList<T> Items
        {
            get { return _items; }
        }

        public int Count
        {
            get
            {
                try
                {
                    Lock();
                    return _items.Count;
                }
                finally
                {
                    Unlock();
                }

            }

        }

        public void Remove(int index)
        {

            try
            {
                Lock();
                _items.RemoveAt(index);
            }
            finally
            {
                Unlock();
            }
        }

        public void Remove(T item)
        {

            try
            {
                Lock();
                _items.Remove(item);
            }
            finally
            {
                Unlock();
            }

        }

        public int Add(T item)
        {

            try
            {
                Lock();

                var count = 0;
                _items.Add(item);

                count = _items.Count;
                return count;
            }
            finally
            {
                Unlock();
            }

        }

        public void Clear()
        {

            try
            {
                Lock();

                _items.Clear();
            }
            finally
            {
                Unlock();
            }
        }

        public T this[int index]
        {
            get { return GetAt(index); }
        }

        public T GetAt(int index)
        {

            try
            {
                Lock();

                T item;
                item = _items[index];

                return item;

            }
            finally
            {
                Unlock();
            }
        }

    }
}