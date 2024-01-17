using OLab.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace OLab.Common.Utils;
public static class SemaphoreLogger
{
  public static async Task WaitAsync(
    IOLabLogger logger,
    string key,
    SemaphoreSlim semaphore)
  {
    logger.LogInformation($"key {key} requesting lock. remaining threads = {semaphore.CurrentCount}");
    await semaphore.WaitAsync();
    logger.LogInformation($"key {key} lock granted. remaining threads = {semaphore.CurrentCount}");
  }

  public static void Release(
    IOLabLogger logger,
    string key,
    SemaphoreSlim semaphore)
  {
    semaphore.Release();
    logger.LogInformation($"key {key} released lock. remaining threads = {semaphore.CurrentCount}");
  }
}
