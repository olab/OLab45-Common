using OLab.Api.Model;
using OLab.Api.Utils;
using System.Collections.Generic;

namespace OLab.Data.Interface
{
  public interface IFileStorageModule
  {
    void AttachUrls(AppSettings appSettings, IList<SystemFiles> items);
  }
}
