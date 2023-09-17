using Microsoft.Extensions.Options;
using OLab.Api.Utils;

namespace OLab.Common.Interfaces;
public interface IOLabConfiguration
{
  AppSettings GetAppSettings();
  //IConfiguration GetConfiguration();
#nullable enable
  T? GetValue<T>(string key, bool optional = false);
  T? GetValue<T>(string section, string key, bool optional = false);
#nullable disable
}