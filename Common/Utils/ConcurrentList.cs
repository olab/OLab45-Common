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
                int count = 0;
                mutex.WaitOne();
                count = _items.Count;
                mutex.ReleaseMutex();
                return count;
            }

        }

        public int Add(T item)
        {
            int count = 0;

            mutex.WaitOne();
            _items.Add(item);
            count = _items.Count;
            mutex.ReleaseMutex();

            return count;
        }

        public T this[int index]
        {
            get { return GetAt(index); }
        }

        public T GetAt(int index)
        {
            T item;
            mutex.WaitOne();
            item = _items[index];
            mutex.ReleaseMutex();

            return item;
        }

    }
}