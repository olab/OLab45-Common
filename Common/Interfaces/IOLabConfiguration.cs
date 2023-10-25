using Microsoft.Extensions.Configuration;
using OLab.Api.Utils;

namespace OLab.Common.Interfaces;
public interface IOLabConfiguration
{
  IConfiguration GetRawConfiguration();
  AppSettings GetAppSettings();
#nullable enable
  T? GetValue<T>(string key, bool optional = false);
  T? GetValue<T>(string section, string key, bool optional = false);
#nullable disable
}