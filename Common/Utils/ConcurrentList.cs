using OLab.Common.Interfaces;
using System.Collections.Generic;
using System.Threading;

namespace OLab.Api.Utils
{
    public class ConcurrentList<T>
  {
    private readonly IList<T> _items = new List<T>();
    private readonly IOLabLogger _logger;
    private static readonly Mutex mutex = new Mutex();

    public ConcurrentList(IOLabLogger logger)
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

    /// <summary>
    /// Remove item from list by index
    /// </summary>
    /// <param name="index">Index to remove</param>
    public void SafeRemove(int index)
    {

      try
      {
        Lock();
        Remove(index);
      }
      finally
      {
        Unlock();
      }
    }

    /// <summary>
    /// Remove item from (assumed locked) list by index
    /// </summary>
    /// <param name="index">Index to remove</param>
    public void Remove(int index)
    {
      _items.RemoveAt(index);
    }

    /// <summary>
    /// Remove item from list
    /// </summary>
    /// <param name="item">objectto remove</param>
    public void SafeRemove(T item)
    {

      try
      {
        Lock();
        Remove(item);
      }
      finally
      {
        Unlock();
      }

    }

    /// <summary>
    /// Remove item from (assumed locked) list
    /// </summary>
    /// <param name="item">objectto remove</param>
    public void Remove(T item)
    {
      _items.Remove(item);
    }

    /// <summary>
    /// Adds item to list
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Index of item just added</returns>
    public int SafeAdd(T item)
    {

      try
      {
        Lock();
        return Add(item);
      }
      finally
      {
        Unlock();
      }

    }


    /// <summary>
    /// Adds item to (assumed locked) list
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Index of item just added</returns>
    public int Add(T item)
    {
      var count = 0;
      _items.Add(item);

      count = _items.Count;
      return count - 1;

    }

    /// <summary>
    /// Clears list
    /// </summary>
    public void SafeClear()
    {

      try
      {
        Lock();
        Clear();
      }
      finally
      {
        Unlock();
      }
    }

    /// <summary>
    /// Clears (assumed locked) list
    /// </summary>
    public void Clear()
    {
      _items.Clear();
    }

    public T this[int index]
    {
      get { return SafeGetAt(index); }
    }

    /// <summary>
    /// Gets item from list by index
    /// </summary>
    /// <param name="index">Index to get</param> 
    public T SafeGetAt(int index)
    {

      try
      {
        Lock();
        return GetAt(index);
      }
      finally
      {
        Unlock();
      }
    }

    /// <summary>
    /// Gets item from (assumed locked) list by index
    /// </summary>
    /// <param name="index">Index to get</param>    
    public T GetAt(int index)
    {
      T item;
      item = _items[index];

      return item;
    }
  }
}