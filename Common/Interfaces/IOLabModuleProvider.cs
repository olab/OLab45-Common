using OLab.Api.Common;

namespace OLab.Common.Interfaces
{
  public interface IOLabModuleProvider<T> where T : class
  {
    T GetModule(string name);
  }
}