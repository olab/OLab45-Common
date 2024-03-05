using DocumentFormat.OpenXml.EMMA;
using OLab.Common.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Common.Utils;
public class SemaphoreManager
{
  private SemaphoreSlim _dictSemaphore = new SemaphoreSlim(1, 1);

  private IDictionary<string, SemaphoreSlim> _semaphores =
    new ConcurrentDictionary<string, SemaphoreSlim>();

  public IOLabLogger Logger;

  public SemaphoreManager(IOLabLogger logger)
  {
    Logger = logger;
  }

  /// <summary>
  /// Creates and waits on a named semaphore 
  /// </summary>
  /// <param name="key">Semaphore name</param>
  /// <param name="cancellation">CancellationToken</param>
  public async Task WaitAsync(
    string key,
    CancellationToken cancellation)
  {
    if (!_semaphores.ContainsKey(key))
    {
      // if named semaphore doesn't exist already, create it
      // then lock it
      try
      {
        await SemaphoreLogger.WaitAsync(
          Logger,
          $"SemaphoreManager WaitAsync",
          _dictSemaphore);

        _semaphores.Add(key, new SemaphoreSlim(1, 1));

      }
      catch (ArgumentException)
      {
        Logger.LogWarning($"semaphore '{key}' already exists");
      }
      finally
      {
        SemaphoreLogger.Release(
          Logger,
          $"SemaphoreManager WaitAsync",
          _dictSemaphore);
      }

    }

    await SemaphoreLogger.WaitAsync(
      Logger,
      key,
      _semaphores[key],
      cancellation);
  }

  public void Release(string key)
  {
    if (_semaphores.ContainsKey(key))
      SemaphoreLogger.Release(
        Logger,
        $"SemaphoreManager WaitAsync",
        _semaphores[key]);
    else
      Logger.LogWarning($"semaphore key '{key}' not found");

  }

}
